namespace GameDesign4.Build.Contracts
{
    /// <summary>
    /// 建筑生产能力注册表接口。
    /// 负责向外暴露当前已建成生产建筑数量。
    /// </summary>
    public interface IBuildingRegistry
    {
        #region 生产建筑登记
        /// <summary>
        /// 登记一个已建成建筑。
        /// </summary>
        void RegisterBuilding(string buildingId);

        /// <summary>
        /// 获取指定建筑当前已建成数量。
        /// </summary>
        int GetBuildingCount(string buildingId);
        #endregion
    }
}
