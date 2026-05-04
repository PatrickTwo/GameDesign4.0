using GameDesign4.Build.Contracts;
using GameDesign4.Command.Contracts;
using GameDesign4.Infrastructure.Runtime.Pointer;
using GameDesign4.Input.Contracts;
using UnityEngine;

namespace GameDesign4.SceneInteract.Runtime
{
    /// <summary>
    /// 场景交互主控制器。
    /// 负责消费场景交互语义输入，并派发选择、移动与攻击相关命令。
    /// </summary>
    public sealed class SceneInteractController : ISceneInteractInputConsumer
    {
        private readonly PointerContextService pointerContextService;
        private readonly SceneSelectionService selectionService;
        private readonly SceneCommandService commandService;

        /// <summary>
        /// 构造场景交互主控制器。
        /// </summary>
        public SceneInteractController(
            ICommandBus commandBus,
            IBuildPlacementService buildPlacementService,
            PointerContextService pointerContextService)
        {
            this.pointerContextService = pointerContextService;
            selectionService = new SceneSelectionService();
            commandService = new SceneCommandService(commandBus, buildPlacementService, selectionService);
        }

        #region 输入消费
        /// <summary>
        /// 处理主操作输入。
        /// </summary>
        public void HandlePrimaryAction(Vector2 screenPosition)
        {
            PointerContext pointerContext = BuildPointerContext(screenPosition);
            SceneSelectable hitSelectable = ResolveSceneSelectable(pointerContext.HitTransform);
            commandService.HandlePrimaryClick(pointerContext, hitSelectable);
        }

        /// <summary>
        /// 处理次操作输入。
        /// </summary>
        public void HandleSecondaryAction(Vector2 screenPosition)
        {
            PointerContext pointerContext = BuildPointerContext(screenPosition);
            SceneSelectable hitSelectable = ResolveSceneSelectable(pointerContext.HitTransform);
            commandService.HandleSecondaryClick(pointerContext, hitSelectable);
        }

        /// <summary>
        /// 处理取消输入。
        /// </summary>
        public void HandleCancelAction()
        {
            commandService.HandleCancel();
        }
        #endregion

        #region 输入辅助
        /// <summary>
        /// 构建当前指针上下文。
        /// </summary>
        private PointerContext BuildPointerContext(Vector2 screenPosition)
        {
            return pointerContextService.GetPointerContext(screenPosition);
        }

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
