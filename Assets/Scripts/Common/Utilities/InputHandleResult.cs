namespace GameDesign4.Infrastructure.Runtime
{
    /// <summary>
    /// 输入处理结果。
    /// 负责统一表达一次输入处理后当前模式是否继续、完成或取消。
    /// </summary>
    public enum InputHandleResult
    {
        Continue = 0,
        Completed = 1,
        Cancelled = 2,
    }
}
