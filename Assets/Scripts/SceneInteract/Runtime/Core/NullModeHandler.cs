

using System;
using GameDesign4.Shared.Utilities;

namespace GameDesign4.SceneInteract.Runtime
{
    #region 空实现处理器
    /// <summary>
    /// 用于测试或模块未接入时的空模式处理器。
    /// 保证控制器在缺少具体处理器实现时仍可安全切换模式。
    /// </summary>
    public sealed class NullModeHandler : ISceneInteractModeHandler
    {
        private readonly SceneInteractModeType modeType;

        /// <summary>
        /// 构造空模式处理器。
        /// </summary>
        public NullModeHandler(SceneInteractModeType modeType)
        {
            this.modeType = modeType;
        }

        /// <summary>
        /// 当前处理器对应的交互模式类型。
        /// </summary>
        public SceneInteractModeType ModeType => modeType;

        /// <summary>
        /// 请求切换交互模式事件。
        /// </summary>
        public event Action<SceneInteractModeType> RequestModeSwitch
        {
            add
            {
            }
            remove
            {
            }
        }

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
        }
    }
    #endregion

}