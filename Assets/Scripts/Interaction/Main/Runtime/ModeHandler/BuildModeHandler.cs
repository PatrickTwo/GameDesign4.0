using GameDesign4.Build.Contracts;
using GameDesign4.Infrastructure.Runtime;
using GameDesign4.Infrastructure.Runtime.Pointer;
using GameDesign4.Interaction.Contracts;

namespace GameDesign4.Interaction.Runtime
{
    /// <summary>
    /// 建造模式控制器。
    /// 负责把地图点击转发给建造系统。
    /// </summary>
    public sealed class BuildModeHandler : IInteractionModeHandler
    {
        private readonly IBuildPlacementService buildPlacementService;

        /// <summary>
        /// 当前处理器负责的交互模式。
        /// </summary>
        public InteractionModeType ModeType => InteractionModeType.Build;

        /// <summary>
        /// 构造建造模式控制器。
        /// </summary>
        public BuildModeHandler(IBuildPlacementService buildPlacementService)
        {
            this.buildPlacementService = buildPlacementService;
        }

        #region 输入处理
        /// <summary>
        /// 处理建造模式下的主操作输入。
        /// </summary>
        public InputHandleResult HandlePrimaryAction(PointerContext pointerContext, SceneSelectable hitSelectable)
        {
            return buildPlacementService.HandlePrimaryAction(pointerContext.GroundHitPoint, pointerContext.HasGroundHit, pointerContext.IsOverUI);
        }

        /// <summary>
        /// 处理建造模式下的次操作输入。
        /// </summary>
        public InputHandleResult HandleSecondaryAction(PointerContext pointerContext, SceneSelectable hitSelectable)
        {
            return buildPlacementService.HandleSecondaryAction();
        }

        /// <summary>
        /// 处理建造模式下的取消输入。
        /// </summary>
        public InputHandleResult HandleCancelAction()
        {
            return buildPlacementService.HandleCancelAction();
        }
        #endregion
    }
}
