using System;
using GameDesign4.Shared.Utilities;

namespace GameDesign4.SceneInteract.Runtime
{
    /// <summary>
    /// 单位部署模式处理器。
    /// </summary>
    public sealed class UnitDeploymentModeHandler : ISceneInteractModeHandler
    {
        /// <summary>
        /// 当前处理器对应的交互模式类型。
        /// </summary>
        public SceneInteractModeType ModeType => SceneInteractModeType.UnitDeployment;

        /// <summary>
        /// 请求切换交互模式事件。
        /// </summary>
        public event Action<SceneInteractModeType> RequestModeSwitch;

        #region 模式生命周期
        /// <summary>
        /// 进入当前模式。
        /// </summary>
        public void Enter()
        {
        }

        /// <summary>
        /// 退出当前模式。
        /// </summary>
        public void Exit()
        {
        }
        #endregion

        #region 输入处理
        /// <summary>
        /// 每帧更新当前模式状态。
        /// </summary>
        public void Tick(PointerContext pointerContext)
        {
            Guard.EnsureNotNull(pointerContext, nameof(pointerContext));
        }

        /// <summary>
        /// 处理输入命令。
        /// </summary>
        public void HandleInputCommand(InputCommandType command, PointerContext pointerContext)
        {
            Guard.EnsureNotNull(pointerContext, nameof(pointerContext));

            // 当前阶段部署模式确认或取消后统一回到默认模式。
            if (command == InputCommandType.PrimaryClick || command == InputCommandType.SecondaryClick || command == InputCommandType.Cancel)
            {
                RequestModeSwitch?.Invoke(SceneInteractModeType.SceneCommand);
            }
        }
        #endregion
    }
}
