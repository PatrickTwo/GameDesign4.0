using GameDesign4.Grid.Contracts.Model;

namespace GameDesign4.Grid.Contracts.Service
{
    /// <summary>
    /// 网格控制服务接口。
    /// 负责统一提供网格显示控制能力。
    /// </summary>
    public interface IGridControlService
    {
        #region 显示控制

        /// <summary>
        /// 显示网格。
        /// </summary>
        void ShowGrid();

        /// <summary>
        /// 隐藏网格。
        /// </summary>
        void HideGrid();

        #endregion

        #region 悬停控制

        /// <summary>
        /// 更新悬停坐标。传 null 取消悬停。
        /// </summary>
        void SetHoverCoord(GridCoord? coord);

        #endregion

        #region 预览控制

        /// <summary>
        /// 设置预览占用区域及其有效性。
        /// </summary>
        void SetPreviewFootprint(GridFootprint footprint, bool isValid);

        /// <summary>
        /// 清除预览占用区域。
        /// </summary>
        void ClearPreviewFootprint();

        #endregion

        #region 占用控制

        /// <summary>
        /// 添加一片占地到显示状态。
        /// </summary>
        void AddOccupiedFootprint(GridFootprint footprint);

        /// <summary>
        /// 从显示状态移除一片占地。
        /// </summary>
        void RemoveOccupiedFootprint(GridFootprint footprint);

        #endregion
    }
}
