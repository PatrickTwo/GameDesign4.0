using System;
using GameDesign4.Input.Contracts;
using GameDesign4.Input.Generated;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace GameDesign4.Input.Runtime
{
    /// <summary>
    /// 输入控制器。
    /// 负责持有 GameInput，并将底层输入转换为业务模块可消费的语义输入。
    /// </summary>
    public sealed class InputController : IInitializable, IDisposable, GameInput.IGlobalActions, GameInput.ISceneInteractActions, GameInput.IUIActions
    {
        private readonly IInteractionInputConsumer interactionInputConsumer;
        private readonly IUIInputConsumer uiInputConsumer;
        private GameInput gameInput;

        /// <summary>
        /// 构造输入控制器。
        /// </summary>
        public InputController(
            IInteractionInputConsumer interactionInputConsumer,
            IUIInputConsumer uiInputConsumer)
        {
            this.interactionInputConsumer = interactionInputConsumer;
            this.uiInputConsumer = uiInputConsumer;
        }

        #region 生命周期
        /// <summary>
        /// 初始化输入资产并启用全部输入映射。
        /// </summary>
        public void Initialize()
        {
            gameInput = new GameInput();
            gameInput.Global.SetCallbacks(this);
            gameInput.SceneInteract.SetCallbacks(this);
            gameInput.UI.SetCallbacks(this);
            gameInput.Global.Enable();
            gameInput.SceneInteract.Enable();
            gameInput.UI.Enable();
        }

        /// <summary>
        /// 关闭输入监听并释放输入资产。
        /// </summary>
        public void Dispose()
        {
            if (gameInput == null)
            {
                return;
            }

            gameInput.Global.Disable();
            gameInput.UI.Disable();
            gameInput.SceneInteract.Disable();
            gameInput.Global.RemoveCallbacks(this);
            gameInput.UI.RemoveCallbacks(this);
            gameInput.SceneInteract.RemoveCallbacks(this);
            gameInput.Dispose();
            gameInput = null;
        }
        #endregion

        #region Global 输入回调
        /// <summary>
        /// 处理统一取消输入。
        /// 优先交给 UI 消费，未消费时再交给场景交互。
        /// </summary>
        public void OnCancel(InputAction.CallbackContext context)
        {
            if (context.performed == false)
            {
                return;
            }
            // 优先交给 UI 消费。
            if (uiInputConsumer.HandleCancelAction())
            {
                return;
            }

            interactionInputConsumer.HandleCancelAction();
        }
        #endregion

        #region SceneInteract 输入回调
        /// <summary>
        /// 当前阶段不直接消费指针位置流。
        /// </summary>
        public void OnPointerPosition(InputAction.CallbackContext context)
        {
        }

        /// <summary>
        /// 处理主操作输入。
        /// </summary>
        public void OnLeftClick(InputAction.CallbackContext context)
        {
            if (context.performed == false || gameInput == null)
            {
                return;
            }

            Vector2 screenPosition = gameInput.SceneInteract.PointerPosition.ReadValue<Vector2>();
            interactionInputConsumer.HandlePrimaryAction(screenPosition);
        }

        /// <summary>
        /// 处理次操作输入。
        /// </summary>
        public void OnRightClick(InputAction.CallbackContext context)
        {
            if (context.performed == false || gameInput == null)
            {
                return;
            }

            Vector2 screenPosition = gameInput.SceneInteract.PointerPosition.ReadValue<Vector2>();
            interactionInputConsumer.HandleSecondaryAction(screenPosition);
        }

        #endregion

        #region UI 输入回调
        /// <summary>
        /// 处理建造面板切换输入。
        /// </summary>
        public void OnToggleBuildPanel(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                uiInputConsumer.HandleToggleBuildPanel();
            }
        }

        /// <summary>
        /// 处理仓库面板切换输入。
        /// </summary>
        public void OnToggleInventoryPanel(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                uiInputConsumer.HandleToggleInventoryPanel();
            }
        }

        /// <summary>
        /// 处理生产面板切换输入。
        /// </summary>
        public void OnToggleProductionPanel(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                uiInputConsumer.HandleToggleProductionPanel();
            }
        }
        #endregion
    }
}
