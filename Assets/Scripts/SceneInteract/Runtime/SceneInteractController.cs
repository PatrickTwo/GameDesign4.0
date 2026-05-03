using System;
using GameDesign4.Build.Contracts;
using GameDesign4.Command.Contracts;
using GameDesign4.Infrastructure.Runtime.Pointer;
using GameDesign4.SceneInteract.Presentation.Input.Generated;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace GameDesign4.SceneInteract.Runtime
{
    /// <summary>
    /// 场景交互主控制器。
    /// 负责驱动输入采集、射线命中解析、选择切换与场景命令派发。
    /// </summary>
    public sealed class SceneInteractController : IInitializable, IDisposable, GameInput.ISceneInteractActions
    {
        private readonly PointerContextService pointerContextService;
        private readonly SceneSelectionService selectionService;
        private readonly SceneCommandService commandService;
        private GameInput gameInput;

        /// <summary>
        /// 构造场景交互主控制器。
        /// </summary>
        public SceneInteractController(ICommandBus commandBus, IBuildPlacementService buildPlacementService)
        {
            pointerContextService = new PointerContextService();
            selectionService = new SceneSelectionService();
            commandService = new SceneCommandService(commandBus, buildPlacementService, selectionService);
        }

        #region 生命周期
        /// <summary>
        /// 初始化交互运行时对象并启用输入监听。
        /// </summary>
        public void Initialize()
        {
            gameInput = new GameInput();
            gameInput.SceneInteract.SetCallbacks(this);
            gameInput.SceneInteract.Enable();
        }

        /// <summary>
        /// 停止输入监听并释放输入对象。
        /// </summary>
        public void Dispose()
        {
            if (gameInput == null)
            {
                return;
            }

            gameInput.SceneInteract.Disable();
            gameInput.SceneInteract.RemoveCallbacks(this);
            selectionService.ClearSelection();
            gameInput.Dispose();
            gameInput = null;
        }
        #endregion

        #region 输入回调
        /// <summary>
        /// 当前阶段不直接消费指针位置流。
        /// </summary>
        public void OnPointerPosition(InputAction.CallbackContext context)
        {
        }

        /// <summary>
        /// 处理左键点击输入。
        /// </summary>
        public void OnLeftClick(InputAction.CallbackContext context)
        {
            if (context.performed == false || gameInput == null)
            {
                return;
            }

            PointerContext pointerContext = BuildPointerContext();
            SceneSelectable hitSelectable = ResolveSceneSelectable(pointerContext.HitTransform);
            commandService.HandlePrimaryClick(pointerContext, hitSelectable);
        }

        /// <summary>
        /// 处理右键点击输入。
        /// </summary>
        public void OnRightClick(InputAction.CallbackContext context)
        {
            if (context.performed == false || gameInput == null)
            {
                return;
            }

            PointerContext pointerContext = BuildPointerContext();
            SceneSelectable hitSelectable = ResolveSceneSelectable(pointerContext.HitTransform);
            commandService.HandleSecondaryClick(pointerContext, hitSelectable);
        }

        /// <summary>
        /// 处理取消输入。
        /// </summary>
        public void OnCancel(InputAction.CallbackContext context)
        {
            if (context.performed == false || gameInput == null)
            {
                return;
            }

            commandService.HandleCancel();
        }
        #endregion

        #region 输入辅助
        /// <summary>
        /// 构建当前指针上下文。
        /// </summary>
        private PointerContext BuildPointerContext()
        {
            Vector2 screenPosition = gameInput.SceneInteract.PointerPosition.ReadValue<Vector2>();
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
