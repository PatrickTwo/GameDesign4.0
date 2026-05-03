namespace GameDesign4.SceneInteract.Runtime
{
    /// <summary>
    /// 交互系统交互模式类型。
    /// </summary>
    public enum SceneInteractModeType
    {
        SceneCommand = 0, // 场景命令模式
        BuildingPlacement = 1, // 建筑放置模式
        UnitDeployment = 2, // 单位部署模式
        Targeting = 3, // 目标选择模式
        AreaCommand = 4 // 区域命令模式
    }
}
