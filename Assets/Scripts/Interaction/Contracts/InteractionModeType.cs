namespace GameDesign4.Interaction.Contracts
{
    /// <summary>
    /// 交互模式枚举。
    /// 负责统一声明默认、建造与部署三种输入模式。
    /// </summary>
    public enum InteractionModeType
    {
        Default = 0,
        Build = 1,
        Deployment = 2,
    }
}
