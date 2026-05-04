using System.Collections.Generic;
using GameDesign4.Build.Contracts;
using GameDesign4.Infrastructure.Runtime.Logging;
using GameDesign4.Infrastructure.Utilities;
using GameDesign4.Inventory.Contracts;
using GameDesign4.Production.Contracts;
using GameDesign4.Production.Definition;
using UnityEngine;
using VContainer.Unity;

namespace GameDesign4.Production.Runtime
{
    /// <summary>
    /// 生产服务。
    /// 负责排产、等待队列、生产槽分配、计时推进与完成入库。
    /// </summary>
    public sealed class ProductionService : IProductionService, ITickable
    {
        private readonly IBuildingRegistry buildingProductionRegistry;
        private readonly IInventoryService inventoryService;
        private readonly List<ProductionBlueprintDef> availableBlueprints;
        private readonly Dictionary<string, ProductionBlueprintDef> blueprintLookup;
        private readonly List<ProductionTaskState> waitingTasks;
        private readonly List<ProductionBuildingSlotState> buildingSlots;
        private readonly List<ProductionTaskState> completedTasks;

        public IReadOnlyList<ProductionBlueprintDef> AvailableBlueprints => availableBlueprints;

        /// <summary>
        /// 构造生产服务。
        /// </summary>
        public ProductionService(
            ProductionCatalogDef productionCatalog,
            IBuildingRegistry buildingProductionRegistry,
            IInventoryService inventoryService)
        {
            Guard.EnsureNotNull(productionCatalog, nameof(productionCatalog));
            Guard.EnsureNotNull(buildingProductionRegistry, nameof(buildingProductionRegistry));
            Guard.EnsureNotNull(inventoryService, nameof(inventoryService));

            this.buildingProductionRegistry = buildingProductionRegistry;
            this.inventoryService = inventoryService;
            availableBlueprints = new List<ProductionBlueprintDef>();
            blueprintLookup = new Dictionary<string, ProductionBlueprintDef>();
            waitingTasks = new List<ProductionTaskState>();
            buildingSlots = new List<ProductionBuildingSlotState>();
            completedTasks = new List<ProductionTaskState>();

            IReadOnlyList<ProductionBlueprintDef> blueprints = productionCatalog.Blueprints;
            for (int index = 0; index < blueprints.Count; index++)
            {
                ProductionBlueprintDef blueprint = blueprints[index];
                if (blueprint == null || string.IsNullOrWhiteSpace(blueprint.Id))
                {
                    continue;
                }

                blueprintLookup[blueprint.Id] = blueprint;
                availableBlueprints.Add(blueprint);
            }
        }

        #region 排产入口
        /// <summary>
        /// 基于蓝图标识追加一条生产任务。
        /// </summary>
        public void EnqueueProduction(string blueprintId)
        {
            Guard.Ensure(blueprintLookup.TryGetValue(blueprintId, out ProductionBlueprintDef blueprint), $"未找到生产蓝图：{blueprintId}");

            ProductionTaskState task = new ProductionTaskState(blueprint);
            waitingTasks.Add(task);
            GameLog.Log(GameLogModule.Production, $"新增生产任务：{blueprint.DisplayName}");

            // 点击蓝图后立即尝试排产，避免必须等待下一帧才开始生产。
            SyncBuildingSlots();
            DispatchWaitingTasks();
        }
        #endregion

        #region 每帧更新
        /// <summary>
        /// 每帧推进生产逻辑。
        /// </summary>
        public void Tick()
        {
            SyncBuildingSlots();
            DispatchWaitingTasks();
            TickWorkingTasks(Time.deltaTime);
            FlushCompletedTasksToInventory();
        }
        #endregion

        #region 排产调度
        /// <summary>
        /// 同步当前已建成建筑对应的生产槽数量。
        /// </summary>
        private void SyncBuildingSlots()
        {
            for (int blueprintIndex = 0; blueprintIndex < availableBlueprints.Count; blueprintIndex++)
            {
                ProductionBlueprintDef blueprint = availableBlueprints[blueprintIndex];
                if (blueprint.ProducerBuilding == null)
                {
                    continue;
                }

                string producerBuildingId = blueprint.ProducerBuilding.Id;
                int targetSlotCount = buildingProductionRegistry.GetBuildingCount(producerBuildingId);
                int currentSlotCount = GetSlotCount(producerBuildingId);

                // 当前版本建筑只增不减，因此这里只补足新增槽位，不处理缩容。
                for (int slotIndex = currentSlotCount; slotIndex < targetSlotCount; slotIndex++)
                {
                    buildingSlots.Add(new ProductionBuildingSlotState(producerBuildingId, slotIndex));
                }
            }
        }

        /// <summary>
        /// 尝试把等待中的任务分配到空闲生产槽。
        /// </summary>
        private void DispatchWaitingTasks()
        {
            for (int taskIndex = 0; taskIndex < waitingTasks.Count;)
            {
                ProductionTaskState task = waitingTasks[taskIndex];
                if (task.Blueprint.ProducerBuilding == null)
                {
                    taskIndex++;
                    continue;
                }

                ProductionBuildingSlotState idleSlot = FindIdleSlot(task.Blueprint.ProducerBuilding.Id);
                if (idleSlot == null)
                {
                    taskIndex++;
                    continue;
                }

                idleSlot.AssignTask(task);
                waitingTasks.RemoveAt(taskIndex);
                GameLog.Log(GameLogModule.Production, $"开始生产：{task.Blueprint.DisplayName}，槽位：{idleSlot.SlotId}");
            }
        }

        /// <summary>
        /// 推进当前正在生产的任务。
        /// </summary>
        private void TickWorkingTasks(float deltaSeconds)
        {
            for (int slotIndex = 0; slotIndex < buildingSlots.Count; slotIndex++)
            {
                ProductionBuildingSlotState slot = buildingSlots[slotIndex];
                if (slot.IsBusy == false)
                {
                    continue;
                }

                ProductionTaskState task = slot.CurrentTask;
                task.Tick(deltaSeconds);
                if (task.IsCompleted == false)
                {
                    continue;
                }

                completedTasks.Add(task);
                slot.ReleaseTask();
            }
        }

        /// <summary>
        /// 把当前帧完成的生产任务统一入库。
        /// </summary>
        private void FlushCompletedTasksToInventory()
        {
            for (int index = 0; index < completedTasks.Count; index++)
            {
                ProductionTaskState completedTask = completedTasks[index];
                if (completedTask.Blueprint.Product == null)
                {
                    continue;
                }

                inventoryService.AddItem(completedTask.Blueprint.Product.Id, completedTask.Blueprint.Product.DisplayName, 1);
                GameLog.Log(GameLogModule.Production, $"生产完成并入库：{completedTask.Blueprint.Product.DisplayName}");
            }

            completedTasks.Clear();
        }

        /// <summary>
        /// 获取指定建筑当前已创建的生产槽数量。
        /// </summary>
        private int GetSlotCount(string producerBuildingId)
        {
            int count = 0;
            for (int index = 0; index < buildingSlots.Count; index++)
            {
                if (buildingSlots[index].ProducerBuildingId == producerBuildingId)
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// 查找指定建筑的空闲生产槽。
        /// </summary>
        private ProductionBuildingSlotState FindIdleSlot(string producerBuildingId)
        {
            for (int index = 0; index < buildingSlots.Count; index++)
            {
                ProductionBuildingSlotState slot = buildingSlots[index];
                if (slot.ProducerBuildingId == producerBuildingId && slot.IsBusy == false)
                {
                    return slot;
                }
            }

            return null;
        }
        #endregion
    }
}
