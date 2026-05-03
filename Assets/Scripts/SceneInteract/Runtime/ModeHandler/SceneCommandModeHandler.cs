using System;

namespace GameDesign4.SceneInteract.Runtime
{
    /// <summary>
    /// 默认场景命令模式处理器。
    /// 负责处理单击选中、取消选择、右键命令派发。
    /// </summary>
    public sealed class SceneCommandModeHandler : ISceneInteractModeHandler
    {
        public SceneInteractModeType ModeType => throw new NotImplementedException();

        public event Action<SceneInteractModeType> RequestModeSwitch;

        public void Enter()
        {
            throw new NotImplementedException();
        }

        public void Exit()
        {
            throw new NotImplementedException();
        }

        public void HandleInputCommand(InputCommandType command, PointerContext pointerContext)
        {
            throw new NotImplementedException();
        }

        public void Tick(PointerContext pointerContext)
        {
            throw new NotImplementedException();
        }

    }
}
