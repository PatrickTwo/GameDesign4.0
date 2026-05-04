using GameDesign4.Infrastructure.Utilities;
using GameDesign4.Production.Definition;

namespace GameDesign4.Production.Runtime
{
    /// <summary>
    /// 生产任务运行时状态。
    /// 负责保存单条生产任务的蓝图、剩余时长与槽位归属。
    /// </summary>
    public sealed class ProductionTaskState
    {
        private readonly ProductionBlueprintDef blueprint;
        private float remainingSeconds;
        private string assignedSlotId = string.Empty;

        /// <summary>
        /// 构造生产任务状态。
        /// </summary>
        public ProductionTaskState(ProductionBlueprintDef blueprint)
        {
            Guard.EnsureNotNull(blueprint, nameof(blueprint));

            this.blueprint = blueprint;
            remainingSeconds = blueprint.DurationSeconds;
        }

        /// <summary>
        /// 任务蓝图。
        /// </summary>
        public ProductionBlueprintDef Blueprint => blueprint;

        /// <summary>
        /// 当前任务是否已经完成。
        /// </summary>
        public bool IsCompleted => remainingSeconds <= 0f;

        #region 任务推进
        /// <summary>
        /// 绑定生产槽位。
        /// </summary>
        public void AssignToSlot(string slotId)
        {
            Guard.EnsureNotNullOrWhiteSpace(slotId, nameof(slotId));
            assignedSlotId = slotId;
        }

        /// <summary>
        /// 推进生产计时。
        /// </summary>
        public void Tick(float deltaSeconds)
        {
            // 已完成任务不再继续扣减时长。
            if (remainingSeconds <= 0f)
            {
                return;
            }

            remainingSeconds -= deltaSeconds;
            if (remainingSeconds < 0f)
            {
                remainingSeconds = 0f;
            }
        }
        #endregion
    }
}
