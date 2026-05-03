using System;

namespace GameDesign4.SceneInteract.Runtime
{
    /// <summary>
    /// 交互模式处理器接口。
    /// 定义模式处理器的完整契约，控制器仅通过此接口与处理器交互。
    /// </summary>
    public interface ISceneInteractModeHandler
    {
        /// <summary>
        /// 当前处理器对应的交互模式类型。
        /// </summary>
        SceneInteractModeType ModeType { get; }

        /// <summary>
        /// 请求切换交互模式事件。
        /// 处理器通过触发此事件向控制器请求切换，避免反向依赖控制器。
        /// </summary>
        event Action<SceneInteractModeType> RequestModeSwitch;

        /// <summary>
        /// 进入当前模式时调用。
        /// </summary>
        void Enter();

        /// <summary>
        /// 退出当前模式时调用。
        /// </summary>
        void Exit();

        /// <summary>
        /// 每帧更新当前模式状态。
        /// </summary>
        void Tick(PointerContext pointerContext);

        /// <summary>
        /// 处理已解释为统一语义的输入命令。
        /// </summary>
        void HandleInputCommand(InputCommandType command, PointerContext pointerContext);
    }
}
