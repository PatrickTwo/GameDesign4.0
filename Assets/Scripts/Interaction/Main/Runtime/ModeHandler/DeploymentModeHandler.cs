using GameDesign4.Deployment.Contracts;
using GameDesign4.Infrastructure.Runtime;
using GameDesign4.Infrastructure.Runtime.Pointer;
using GameDesign4.Interaction.Contracts;

namespace GameDesign4.Interaction.Runtime
{
    /// <summary>
    /// 部署模式控制器。
    /// 负责把地图点击转发给部署系统。
    /// </summary>
    public sealed class DeploymentModeHandler : IInteractionModeHandler
    {
        private readonly IDeploymentService deploymentService;

        /// <summary>
        /// 当前处理器负责的交互模式。
        /// </summary>
        public InteractionModeType ModeType => InteractionModeType.Deployment;

        /// <summary>
        /// 构造部署模式控制器。
        /// </summary>
        public DeploymentModeHandler(IDeploymentService deploymentService)
        {
            this.deploymentService = deploymentService;
        }

        #region 输入处理
        /// <summary>
        /// 处理部署模式下的主操作输入。
        /// </summary>
        public InputHandleResult HandlePrimaryAction(PointerContext pointerContext, SceneSelectable hitSelectable)
        {
            return deploymentService.HandlePrimaryAction(pointerContext.GroundHitPoint, pointerContext.HasGroundHit, pointerContext.IsOverUI);
        }

        /// <summary>
        /// 处理部署模式下的次操作输入。
        /// </summary>
        public InputHandleResult HandleSecondaryAction(PointerContext pointerContext, SceneSelectable hitSelectable)
        {
            return deploymentService.HandleSecondaryAction();
        }

        /// <summary>
        /// 处理部署模式下的取消输入。
        /// </summary>
        public InputHandleResult HandleCancelAction()
        {
            return deploymentService.HandleCancelAction();
        }
        #endregion
    }
}
