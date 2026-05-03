namespace GameDesign4.Shared.Runtime.Logging
{
    /// <summary>
    /// 预设日志模块枚举。
    /// </summary>
    public enum GameLogModule
    {
        Shared = 0, // 共享模块
        GameFlow = 1, // 游戏流程模块
        Build = 2, // 建造模块
        ResourceNetwork = 3, // 资源网络模块
        Production = 4, // 生产模块
        Threat = 5, // 威胁模块
        Defense = 6, // 防御模块
        Deployment = 7, // 部署模块
        Unit = 8, // 单位模块
        Command = 9, // 指挥模块
        ReconVision = 10, // 侦察视觉模块
        Combat = 11, // 战斗模块
        AICombat = 12, // AI战斗模块
        UI = 13, // UI模块
        SceneInteract = 14, // 场景交互模块
        Debug = 15, // 调试模块
    }
}
