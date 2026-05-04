using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameDesign4.Build.Contracts;
using GameDesign4.Command.Contracts;
using GameDesign4.Deployment.Contracts;
using GameDesign4.Infrastructure.Definitions;
using GameDesign4.Infrastructure.Runtime;
using GameDesign4.Infrastructure.Runtime.Logging;
using GameDesign4.Infrastructure.Utilities;
using GameDesign4.Inventory.Contracts;
using GameDesign4.Unit.Component;
using GameDesign4.Unit.Contracts.Command;
using GameDesign4.Unit.Definition;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using VContainer.Unity;

namespace GameDesign4.Deployment.Runtime
{
    /// <summary>
    /// 部署服务。
    /// 负责进入部署模式、根据目标点查找最近部署建筑，并出生单位后下发移动命令。
    /// </summary>
    public sealed class DeploymentService : IDeploymentService
    {
        private readonly IBuildingRegistry buildingRegistry;
        private readonly ICommandBus commandBus;
        private readonly IInventoryService inventoryService;
        private readonly IObjectResolver objectResolver;

        // 部署状态
        // 当前待部署单位。
        private UnitDef pendingUnitDef;
        // 当前是否存在有效部署请求。
        public bool HasPendingRequest => pendingUnitDef != null;

        /// <summary>
        /// 构造部署服务。
        /// </summary>
        public DeploymentService(
            IBuildingRegistry buildingRegistry,
            ICommandBus commandBus,
            IInventoryService inventoryService,
            IObjectResolver objectResolver)
        {
            this.buildingRegistry = buildingRegistry;
            this.commandBus = commandBus;
            this.inventoryService = inventoryService;
            this.objectResolver = objectResolver;
        }

        #region 部署入口
        /// <summary>
        /// 基于仓库中的可部署实体开始一次部署流程。
        /// </summary>
        public void StartDeployment(EntityDef deployableEntity)
        {
            UnitDef unitDef = deployableEntity as UnitDef;
            Guard.EnsureNotNull(unitDef, "非法实体定义，部署目标不是 UnitDef。");
            Guard.EnsureNotNull(unitDef.DeployBuilding, $"单位缺少部署建筑定义：{unitDef.DisplayName}");

            // 记录待部署单位
            pendingUnitDef = unitDef;
            GameLog.Log(GameLogModule.Deployment, $"开始部署请求：{unitDef.DisplayName}");
        }
        #endregion

        #region 输入处理
        /// <summary>
        /// 处理部署模式下的主操作输入。
        /// </summary>
        public InputHandleResult HandlePrimaryAction(Vector3 worldPosition, bool hasGroundHit, bool isOverUi)
        {
            if (HasPendingRequest == false)
            {
                return InputHandleResult.Continue;
            }

            // 部署模式激活后，左键优先由部署系统消费，避免透传到默认选择逻辑。
            if (hasGroundHit == false || isOverUi)
            {
                return InputHandleResult.Continue;
            }

            UnitDef unitDef = pendingUnitDef;
            if (unitDef == null || unitDef.DeployBuilding == null)
            {
                ClearPendingRequestInternal("部署失败，单位部署配置无效。");
                return InputHandleResult.Cancelled;
            }

            if (unitDef.PrefabReference == null || unitDef.PrefabReference.RuntimeKeyIsValid() == false)
            {
                ClearPendingRequestInternal($"单位缺少有效预制体，无法部署：{unitDef.DisplayName}");
                return InputHandleResult.Cancelled;
            }

            Transform spawnPoint;
            if (buildingRegistry.TryGetNearestDeploySpawnPoint(unitDef.DeployBuilding, worldPosition, out spawnPoint) == false)
            {
                GameLog.Warning(GameLogModule.Deployment, $"未找到可用部署建筑或出生点：{unitDef.DisplayName}");
                return InputHandleResult.Continue;
            }

            if (HasInventoryItem(unitDef.Id) == false)
            {
                GameLog.Warning(GameLogModule.Deployment, $"仓库数量不足，无法完成部署：{unitDef.DisplayName}");
                return InputHandleResult.Continue;
            }

            pendingUnitDef = null;
            TryDeployAsync(unitDef, worldPosition, spawnPoint).Forget();
            return InputHandleResult.Completed;
        }

        /// <summary>
        /// 处理部署模式下的次操作输入。
        /// </summary>
        public InputHandleResult HandleSecondaryAction()
        {
            if (HasPendingRequest == false)
            {
                return InputHandleResult.Continue;
            }

            ClearPendingRequestInternal("右键取消部署模式。");
            return InputHandleResult.Cancelled;
        }

        /// <summary>
        /// 处理部署模式下的取消输入。
        /// </summary>
        public InputHandleResult HandleCancelAction()
        {
            if (HasPendingRequest == false)
            {
                return InputHandleResult.Continue;
            }

            ClearPendingRequestInternal("取消部署模式。");
            return InputHandleResult.Cancelled;
        }
        #endregion

        #region 部署执行
        /// <summary>
        /// 异步执行一次单位部署。
        /// </summary>
        private async UniTaskVoid TryDeployAsync(UnitDef unitDef, Vector3 targetPosition, Transform spawnPoint)
        {
            UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> unitHandle =
                unitDef.PrefabReference.InstantiateAsync(spawnPoint.position, spawnPoint.rotation);

            await unitHandle.ToUniTask();
            GameObject unitInstance = unitHandle.Result;
            if (unitInstance == null)
            {
                GameLog.Warning(GameLogModule.Deployment, $"单位实例化失败：{unitDef.DisplayName}");
                return;
            }

            // 新出生单位需要先完成依赖注入，再允许接收移动命令。
            objectResolver.InjectGameObject(unitInstance);

            UnitEntity unitEntity = unitInstance.GetComponent<UnitEntity>();
            if (unitEntity == null)
            {
                Addressables.ReleaseInstance(unitInstance);
                GameLog.Warning(GameLogModule.Deployment, $"单位预制体缺少 UnitEntity 组件：{unitDef.DisplayName}");
                return;
            }

            if (inventoryService.RemoveItem(unitDef.Id, 1) == false)
            {
                Addressables.ReleaseInstance(unitInstance);
                GameLog.Warning(GameLogModule.Deployment, $"仓库数量不足，无法完成部署：{unitDef.DisplayName}");
                return;
            }

            commandBus.Publish(new UnitMoveCommand(unitEntity.UnitId, targetPosition));
            GameLog.Log(GameLogModule.Deployment, $"部署单位：{unitDef.DisplayName}");
        }
        #endregion

        #region 请求清理
        /// <summary>
        /// 判断仓库中是否存在指定单位。
        /// </summary>
        private bool HasInventoryItem(string itemId)
        {
            IReadOnlyList<string> itemIds = inventoryService.GetItemIds();
            for (int index = 0; index < itemIds.Count; index++)
            {
                if (itemIds[index] == itemId)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 清空当前部署请求。
        /// </summary>
        private void ClearPendingRequestInternal(string logMessage)
        {
            pendingUnitDef = null;

            if (string.IsNullOrWhiteSpace(logMessage) == false)
            {
                GameLog.Log(GameLogModule.Deployment, logMessage);
            }
        }
        #endregion
    }
}
