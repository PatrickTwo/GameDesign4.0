using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameDesign4.Build.Contracts;
using GameDesign4.Build.Definition;
using GameDesign4.Build.Presentation;
using GameDesign4.Grid.Contracts.Model;
using GameDesign4.Grid.Contracts.Service;
using GameDesign4.Infrastructure.Runtime.Logging;
using GameDesign4.Infrastructure.Runtime.Pointer;
using GameDesign4.Infrastructure.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

namespace GameDesign4.Build.Runtime
{
    /// <summary>
    /// 建造放置服务。
    /// 负责管理蓝图目录、建造状态、预览实例与最终放置流程。
    /// </summary>
    public sealed class BuildPlacementService : IBuildPlacementService, ITickable
    {
        private readonly IGridControlService gridControlService;
        private readonly IGridQueryService gridQueryService;
        private readonly IObjectResolver objectResolver;
        private readonly PointerContextService pointerContextService;
        private readonly BuildPlacementValidator validator;
        private readonly BuildPlacementState state;
        private readonly List<BuildingBlueprintDef> availableBlueprints;
        private readonly Dictionary<string, BuildingBlueprintDef> blueprintLookup;

        /// <summary>
        /// 构造建造放置服务。
        /// </summary>
        public BuildPlacementService(
            BuildCatalogDef buildCatalog,
            IObjectResolver objectResolver,
            IGridControlService gridControlService,
            IGridQueryService gridQueryService,
            PointerContextService pointerContextService)
        {
            this.gridControlService = gridControlService;
            this.gridQueryService = gridQueryService;
            this.objectResolver = objectResolver;
            this.pointerContextService = pointerContextService;
            validator = new BuildPlacementValidator(gridQueryService);
            state = new BuildPlacementState();
            availableBlueprints = new List<BuildingBlueprintDef>();
            blueprintLookup = new Dictionary<string, BuildingBlueprintDef>();

            IReadOnlyList<BuildingBlueprintDef> blueprints = buildCatalog.Blueprints;
            for (int index = 0; index < blueprints.Count; index++)
            {
                BuildingBlueprintDef blueprint = blueprints[index];
                if (blueprint == null || string.IsNullOrWhiteSpace(blueprint.Id))
                {
                    continue;
                }

                blueprintLookup[blueprint.Id] = blueprint;
                availableBlueprints.Add(blueprint);
            }
        }

        /// <summary>
        /// 当前是否处于建造模式。
        /// </summary>
        public bool IsPlacementActive => state.IsPlacementActive;

        #region 每帧更新
        /// <summary>
        /// 每帧刷新建造预览位置。
        /// </summary>
        public void Tick()
        {
            if (state.IsPlacementActive == false)
            {
                return;
            }

            Pointer pointerDevice = Pointer.current;
            if (pointerDevice == null)
            {
                ApplyPointerContext(Vector3.zero, false, false);
                return;
            }

            // 直接复用共享层的指针上下文解析，避免重复维护一套地面命中逻辑。
            Vector2 screenPosition = pointerDevice.position.ReadValue();
            PointerContext pointerContext = pointerContextService.GetPointerContext(screenPosition);
            ApplyPointerContext(pointerContext.GroundHitPoint, pointerContext.HasGroundHit, pointerContext.IsOverUI);
        }
        #endregion

        #region 面板数据
        /// <summary>
        /// 获取当前可展示的建造蓝图列表。
        /// </summary>
        public IReadOnlyList<BuildingBlueprintDef> GetAvailableBlueprints()
        {
            return availableBlueprints;
        }
        #endregion

        #region 面板入口
        /// <summary>
        /// 开始一次新的建造放置。
        /// </summary>
        public void StartPlacement(string blueprintId)
        {
            Guard.Ensure(blueprintLookup.TryGetValue(blueprintId, out BuildingBlueprintDef blueprint), $"未找到建造蓝图：{blueprintId}");

            ReleasePreviewInstance();
            state.BeginPlacement(blueprint);
            gridControlService.ShowGrid();
            gridControlService.SetHoverCoord(null);
            gridControlService.ClearPreviewFootprint();
            LoadPreviewAsync(blueprint, state.PlacementVersion).Forget();
            GameLog.Log(GameLogModule.Build, $"进入建造模式：{blueprint.DisplayName}");
        }
        #endregion

        #region 输入处理
        /// <summary>
        /// 处理建造模式下的左键输入。
        /// </summary>
        public bool HandlePrimaryAction(Vector3 worldPosition, bool hasGroundHit, bool isOverUi)
        {
            if (state.IsPlacementActive == false)
            {
                return false;
            }

            // 建造模式开启后，左键优先由建造系统消费，避免透传到选择逻辑。
            if (hasGroundHit == false || isOverUi)
            {
                return true;
            }

            BuildingBlueprintDef blueprint = state.CurrentBlueprint;
            GridFootprint? previewFootprint = state.PreviewFootprint;
            if (previewFootprint.HasValue == false || validator.CanPlace(blueprint, previewFootprint.Value) == false)
            {
                return true;
            }

            Vector3 placementWorldPosition = state.PreviewPosition;
            gridQueryService.OccupyFootprint(previewFootprint.Value);
            InstantiatePlacedBuildingAsync(blueprint, placementWorldPosition, previewFootprint.Value).Forget();

            // 当前版本一次点击完成一次建造，放置成功后直接退出建造模式。
            CancelPlacementInternal();
            GameLog.Log(GameLogModule.Build, $"放置建筑：{blueprint.DisplayName}");
            return true;
        }

        /// <summary>
        /// 处理建造模式下的右键输入。
        /// </summary>
        public bool HandleSecondaryAction()
        {
            if (state.IsPlacementActive == false)
            {
                return false;
            }

            CancelPlacementInternal();
            GameLog.Log(GameLogModule.Build, "右键取消建造模式。");
            return true;
        }

        /// <summary>
        /// 处理建造模式下的取消输入。
        /// </summary>
        public bool HandleCancelAction()
        {
            if (state.IsPlacementActive == false)
            {
                return false;
            }

            CancelPlacementInternal();
            GameLog.Log(GameLogModule.Build, "取消建造模式。");
            return true;
        }
        #endregion

        #region 预览建筑加载
        /// <summary>
        /// 异步加载当前蓝图的预览实例。
        /// </summary>
        private async UniTaskVoid LoadPreviewAsync(BuildingBlueprintDef blueprint, int placementVersion)
        {
            BuildingDef building = blueprint.Building;
            if (building == null || building.PrefabReference == null || building.PrefabReference.RuntimeKeyIsValid() == false)
            {
                GameLog.Warning(GameLogModule.Build, $"建筑蓝图缺少有效预制体：{blueprint.DisplayName}");
                return;
            }

            UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> previewHandle =
                building.PrefabReference.InstantiateAsync(Vector3.zero, Quaternion.identity);

            await previewHandle.ToUniTask();
            GameObject previewInstance = previewHandle.Result;
            if (previewInstance == null)
            {
                GameLog.Warning(GameLogModule.Build, $"建造预览实例化失败：{blueprint.DisplayName}");
                return;
            }
            
            // 预览实例刚创建时默认是激活状态，先隐藏，避免在鼠标仍停留 UI 时于原点闪现一帧。
            previewInstance.SetActive(false);

            if (state.IsPlacementActive == false || state.PlacementVersion != placementVersion || state.CurrentBlueprint != blueprint)
            {
                Addressables.ReleaseInstance(previewInstance);
                return;
            }

            // 对预览实例执行依赖注入，保证后续若挂接轻量表现组件时依旧可用。
            objectResolver.InjectGameObject(previewInstance);
            BuildPreviewMarker previewMarker = previewInstance.GetComponent<BuildPreviewMarker>();
            if (previewMarker == null)
            {
                previewMarker = previewInstance.AddComponent<BuildPreviewMarker>();
            }

            previewMarker.ConfigureAsPreview();
            state.AttachPreview(previewInstance);
            ApplyPreviewState();
        }
        #endregion
        #region 正式建筑
        /// <summary>
        /// 异步实例化正式建筑。
        /// </summary>
        private async UniTaskVoid InstantiatePlacedBuildingAsync(BuildingBlueprintDef blueprint, Vector3 worldPosition, GridFootprint footprint)
        {
            BuildingDef building = blueprint.Building;
            if (building == null || building.PrefabReference == null || building.PrefabReference.RuntimeKeyIsValid() == false)
            {
                // 预占格后如果发现资源无效，需要立即回滚，避免留下假占用。
                gridQueryService.ReleaseFootprint(footprint);
                GameLog.Warning(GameLogModule.Build, $"正式建筑实例化失败，蓝图缺少有效预制体：{blueprint.DisplayName}");
                return;
            }

            UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> buildHandle =
                building.PrefabReference.InstantiateAsync(worldPosition, Quaternion.identity);

            await buildHandle.ToUniTask();
            GameObject buildingInstance = buildHandle.Result;
            if (buildingInstance == null)
            {
                // 异步实例化失败时，同样回滚本次占格。
                gridQueryService.ReleaseFootprint(footprint);
                GameLog.Warning(GameLogModule.Build, $"正式建筑实例化失败：{blueprint.DisplayName}");
                return;
            }

            // 正式建筑进入场景后执行依赖注入，便于后续接运行时逻辑。
            objectResolver.InjectGameObject(buildingInstance);
        }
        #endregion

        #region 预览清理
        /// <summary>
        /// 应用当前缓存的预览位置和可见状态。
        /// </summary>
        private void ApplyPointerContext(Vector3 worldPosition, bool hasGroundHit, bool isOverUi)
        {
            if (hasGroundHit == false || isOverUi)
            {
                // 当前没有有效地面命中或鼠标在 UI 上时，直接清除网格预览。
                state.UpdatePreviewState(Vector3.zero, false);
                state.UpdateGridPreviewState(null, null, false);
                gridControlService.SetHoverCoord(null);
                gridControlService.ClearPreviewFootprint();
                ApplyPreviewState();
                return;
            }

            bool hasGridCoord = gridQueryService.TryWorldToCoord(worldPosition, out GridCoord gridCoord);
            if (hasGridCoord == false)
            {
                // 命中地面但不在网格范围内时，同样不显示建筑预览。
                state.UpdatePreviewState(Vector3.zero, false);
                state.UpdateGridPreviewState(null, null, false);
                gridControlService.SetHoverCoord(null);
                gridControlService.ClearPreviewFootprint();
                ApplyPreviewState();
                return;
            }

            BuildingBlueprintDef blueprint = state.CurrentBlueprint;
            GridFootprint footprint = CreateFootprint(blueprint, gridCoord);
            bool isPreviewValid = validator.CanPlace(blueprint, footprint);
            Vector3 previewWorldPosition = gridQueryService.GetFootprintWorldCenter(footprint, worldPosition.y);

            // 将吸附后的世界位置与占地状态同时缓存下来，供预览与最终落地共用。
            state.UpdatePreviewState(previewWorldPosition, true);
            state.UpdateGridPreviewState(gridCoord, footprint, isPreviewValid);
            gridControlService.SetHoverCoord(gridCoord);
            gridControlService.SetPreviewFootprint(footprint, isPreviewValid);
            ApplyPreviewState();
        }

        /// <summary>
        /// 应用当前缓存的预览位置和可见状态。
        /// </summary>
        private void ApplyPreviewState()
        {
            GameObject previewInstance = state.PreviewInstance;
            if (previewInstance == null)
            {
                return;
            }

            previewInstance.SetActive(state.HasPreviewPosition);
            if (state.HasPreviewPosition)
            {
                previewInstance.transform.position = state.PreviewPosition;
            }
        }

        /// <summary>
        /// 取消当前建造模式并释放预览。
        /// </summary>
        private void CancelPlacementInternal()
        {
            ReleasePreviewInstance();
            gridControlService.HideGrid();
            state.Clear();
        }

        /// <summary>
        /// 释放当前预览实例。
        /// </summary>
        private void ReleasePreviewInstance()
        {
            GameObject previewInstance = state.DetachPreview();
            if (previewInstance != null)
            {
                Addressables.ReleaseInstance(previewInstance);
            }
        }

        /// <summary>
        /// 基于当前悬停格和建筑占地生成矩形占地。
        /// </summary>
        private static GridFootprint CreateFootprint(BuildingBlueprintDef blueprint, GridCoord anchorCoord)
        {
            // 当前版本固定使用建筑定义中的矩形尺寸，不支持旋转。
            return new GridFootprint(anchorCoord, blueprint.Building.FootprintSize);
        }
        #endregion
    }
}
