using UnityEngine;

namespace GameDesign4.Input.Contracts
{
    /// <summary>
    /// 场景交互输入消费接口。
    /// 负责接收输入模块转发的场景交互语义输入。
    /// </summary>
    public interface ISceneInteractInputConsumer
    {
        /// <summary>
        /// 处理主操作输入。
        /// </summary>
        void HandlePrimaryAction(Vector2 screenPosition);

        /// <summary>
        /// 处理次操作输入。
        /// </summary>
        void HandleSecondaryAction(Vector2 screenPosition);

        /// <summary>
        /// 处理取消输入。
        /// </summary>
        void HandleCancelAction();
    }
}
