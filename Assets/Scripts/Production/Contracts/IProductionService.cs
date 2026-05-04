namespace GameDesign4.Production.Contracts
{
    /// <summary>
    /// 生产服务接口。
    /// 负责向外暴露生产排队入口。
    /// </summary>
    public interface IProductionService
    {
        #region 排产入口
        /// <summary>
        /// 基于蓝图标识追加一条生产任务。
        /// </summary>
        void EnqueueProduction(string blueprintId);
        #endregion
    }
}
