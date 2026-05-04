using GameDesign4.Build.Definition;
using GameDesign4.Infrastructure.Utilities;

namespace GameDesign4.Production.Runtime
{
    /// <summary>
    /// 生产建筑槽位状态。
    /// 负责表示一个建筑实例可提供的单个生产槽。
    /// </summary>
    public sealed class ProductionBuildingSlotState
    {
        private ProductionTaskState currentTask;

        /// <summary>
        /// 构造生产建筑槽位。
        /// </summary>
        public ProductionBuildingSlotState(BuildingDef producerBuilding, int slotIndex)
        {
            Guard.EnsureNotNull(producerBuilding, nameof(producerBuilding));
            Guard.Ensure(slotIndex >= 0, "生产槽位索引不能小于 0。");

            ProducerBuilding = producerBuilding;
            SlotIndex = slotIndex;
        }

        /// <summary>
        /// 指定生产建筑定义。
        /// </summary>
        public BuildingDef ProducerBuilding { get; }

        /// <summary>
        /// 槽位索引。
        /// </summary>
        public int SlotIndex { get; }

        /// <summary>
        /// 当前槽位唯一标识。
        /// </summary>
        public string SlotId => ProducerBuilding.Id + "_" + SlotIndex;

        /// <summary>
        /// 当前正在生产的任务。
        /// </summary>
        public ProductionTaskState CurrentTask => currentTask;

        /// <summary>
        /// 当前槽位是否正在工作。
        /// </summary>
        public bool IsBusy => currentTask != null;

        #region 槽位占用
        /// <summary>
        /// 分配一条任务到当前槽位。
        /// </summary>
        public void AssignTask(ProductionTaskState task)
        {
            Guard.Ensure(currentTask == null, $"生产槽位已被占用：{SlotId}");
            Guard.EnsureNotNull(task, nameof(task));

            currentTask = task;
            currentTask.AssignToSlot(SlotId);
        }

        /// <summary>
        /// 释放当前槽位上的任务。
        /// </summary>
        public ProductionTaskState ReleaseTask()
        {
            ProductionTaskState releasedTask = currentTask;
            currentTask = null;
            return releasedTask;
        }
        #endregion
    }
}
