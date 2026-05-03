namespace GameDesign4.Shared.Runtime.Debug
{
    /// <summary>
    /// 调试输出接口。
    /// 各模块实现此接口，通过 DI 注册后由 DebugOverlay 自动显示。
    /// </summary>
    public interface IDebugOutput
    {
        /// <summary>
        /// 输出源标题，用于叠层中区分不同模块的调试信息。
        /// </summary>
        string OutputName { get; }

        /// <summary>
        /// 获取当前帧的调试文本内容。
        /// 每帧由 DebugOverlay 调用，返回格式化的调试信息。
        /// </summary>
        string GetDebugText();
    }
}
