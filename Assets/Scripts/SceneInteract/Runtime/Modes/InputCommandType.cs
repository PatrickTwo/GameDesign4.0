namespace GameDesign4.SceneInteract.Runtime
{
    /// <summary>
    /// 输入命令类型。
    /// 定义交互模式处理器消费的统一输入语义。
    /// </summary>
    public enum InputCommandType
    {
        None = 0,
        PrimaryClick = 1,
        SecondaryClick = 2,
        Cancel = 3,
        RotatePositive = 4,
        RotateNegative = 5
    }
}
