using GameDesign4.Build.Contracts;
using GameDesign4.Command.Contracts;
using GameDesign4.Deployment.Contracts;
using GameDesign4.Infrastructure.Definitions;
using GameDesign4.Infrastructure.Runtime;
using GameDesign4.Infrastructure.Runtime.Pointer;
using GameDesign4.Input.Contracts;
using GameDesign4.Interaction.Contracts;
using UnityEngine;

namespace GameDesign4.Interaction.Runtime
{
    /// <summary>
    /// 交互主控制器。
    /// 负责维护当前交互模式，并根据当前模式把输入分发给对应处理器。
    /// </summary>
    public sealed class InteractionModeController : IInteractionInputConsumer, IInteractionModeController
    {
        private readonly PointerContextService pointerContextService;
        private readonly IBuildPlacementService buildPlacementService;
        private readonly IDeploymentService deploymentService;
        // 三种交互模式处理器
        private readonly DefaultModeHandler defaultModeHandler;
        private readonly BuildModeHandler buildModeHandler;
        private readonly DeploymentModeHandler deploymentModeHandler;
        // 当前交互模式处理器
        private IInteractionModeHandler currentHandler;

        /// <summary>
        /// 当前交互模式。
        /// </summary>
        public InteractionModeType CurrentMode { get; private set; }

        /// <summary>
        /// 构造交互主控制器。
        /// </summary>
        public InteractionModeController(
            PointerContextService pointerContextService,
            ICommandBus commandBus,
            IBuildPlacementService buildPlacementService,
            IDeploymentService deploymentService)
        {
            this.pointerContextService = pointerContextService;
            this.buildPlacementService = buildPlacementService;
            this.deploymentService = deploymentService;
            defaultModeHandler = new DefaultModeHandler(commandBus);
            buildModeHandler = new BuildModeHandler(buildPlacementService);
            deploymentModeHandler = new DeploymentModeHandler(deploymentService);
            CurrentMode = InteractionModeType.Default;
            currentHandler = defaultModeHandler;
        }

        #region 模式切换
        /// <summary>
        /// 切换到指定交互模式。
        /// </summary>
        public void SwitchMode(InteractionModeType mode)
        {
            CurrentMode = mode;
            currentHandler = mode switch
            {
                InteractionModeType.Default => defaultModeHandler,
                InteractionModeType.Build => buildModeHandler,
                InteractionModeType.Deployment => deploymentModeHandler,
                _ => defaultModeHandler,
            };
        }

        /// <summary>
        /// 进入建造模式并初始化指定蓝图的建造流程。
        /// </summary>
        public void EnterBuildMode(string blueprintId)
        {
            buildPlacementService.StartPlacement(blueprintId);
            SwitchMode(InteractionModeType.Build);
        }

        /// <summary>
        /// 进入部署模式并初始化指定实体的部署流程。
        /// </summary>
        public void EnterDeploymentMode(EntityDef deployableEntity)
        {
            deploymentService.StartDeployment(deployableEntity);
            SwitchMode(InteractionModeType.Deployment);
        }
        #endregion

        #region 输入消费
        /// <summary>
        /// 处理主操作输入。
        /// </summary>
        public void HandlePrimaryAction(Vector2 screenPosition)
        {
            PointerContext pointerContext = pointerContextService.GetPointerContext(screenPosition);
            SceneSelectable hitSelectable = ResolveSceneSelectable(pointerContext.HitTransform);
            ApplyHandleResult(currentHandler.HandlePrimaryAction(pointerContext, hitSelectable));
        }

        /// <summary>
        /// 处理次操作输入。
        /// </summary>
        public void HandleSecondaryAction(Vector2 screenPosition)
        {
            PointerContext pointerContext = pointerContextService.GetPointerContext(screenPosition);
            SceneSelectable hitSelectable = ResolveSceneSelectable(pointerContext.HitTransform);
            ApplyHandleResult(currentHandler.HandleSecondaryAction(pointerContext, hitSelectable));
        }

        /// <summary>
        /// 处理取消输入。
        /// </summary>
        public void HandleCancelAction()
        {
            ApplyHandleResult(currentHandler.HandleCancelAction());
        }
        #endregion

        #region 结果处理
        /// <summary>
        /// 根据输入处理结果决定是否退出当前模式。
        /// </summary>
        private void ApplyHandleResult(InputHandleResult result)
        {
            if (result == InputHandleResult.Completed || result == InputHandleResult.Cancelled)
            {
                SwitchMode(InteractionModeType.Default);
            }
        }
        #endregion

        #region 场景对象解析
        /// <summary>
        /// 从命中的场景节点解析可选择对象。
        /// </summary>
        private static SceneSelectable ResolveSceneSelectable(Transform hitTransform)
        {
            if (hitTransform == null)
            {
                return null;
            }

            return hitTransform.GetComponentInParent<SceneSelectable>();
        }
        #endregion
    }
}
