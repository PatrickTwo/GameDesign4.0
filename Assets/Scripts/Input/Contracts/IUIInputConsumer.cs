namespace GameDesign4.Input.Contracts
{
    /// <summary>
    /// UI 输入消费接口。
    /// 负责接收输入模块转发的 UI 快捷键语义输入。
    /// </summary>
    public interface IUIInputConsumer
    {
        /// <summary>
        /// 处理建造面板切换输入。
        /// </summary>
        void HandleToggleBuildPanel();

        /// <summary>
        /// 处理仓库面板切换输入。
        /// </summary>
        void HandleToggleInventoryPanel();

        /// <summary>
        /// 处理生产面板切换输入。
        /// </summary>
        void HandleToggleProductionPanel();

        /// <summary>
        /// 处理取消输入。
        /// 返回值用于告知输入模块本次取消是否已被 UI 消费。
        /// </summary>
        bool HandleCancelAction();
    }
}
