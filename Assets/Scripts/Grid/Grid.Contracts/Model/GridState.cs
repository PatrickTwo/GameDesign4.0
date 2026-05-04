using System;
using System.Collections.Generic;

namespace GameDesign4.Grid.Contracts.Model
{
    /// <summary>
    /// 网格显示状态。
    /// 纯数据对象，描述网格视图的全部显示状态。
    /// 外部只修改数据，视图只读取数据并响应变更。
    /// </summary>
    public sealed class GridState
    {
        private bool isGridVisible;
        // 悬停坐标
        // ? 等价于 Nullable<GridCoord>，表示可为空
        // null 表示当前没有鼠标悬停的格子
        private GridCoord? hoverCoord;
        // 预览占用区域
        // null表示当前没有预览占用区域
        private GridFootprint? previewFootprint;
        // 预览占用区域是否有效
        private bool isPreviewValid;
        private List<GridFootprint> occupiedFootprints = new();
        // 已占用标记集合变更事件，用于通知视图更新。
        public event Action<List<GridFootprint>> OnOccupiedChanged;


        #region 对外属性
        public bool IsGridVisible => isGridVisible;
        public GridCoord? HoverCoord => hoverCoord;
        public GridFootprint? PreviewFootprint => previewFootprint;
        public bool IsPreviewValid => isPreviewValid;
        #endregion

        #region 方法接口
        /// <summary>
        /// 设置网格可见性。
        /// </summary>
        public void SetGridVisible(bool visible)
        {
            isGridVisible = visible;
        }

        /// <summary>
        /// 设置悬停坐标。传 null 表示取消悬停。
        /// </summary>
        public void SetHoverCoord(GridCoord? coord)
        {
            hoverCoord = coord;
        }

        /// <summary>
        /// 设置预览占用区域。
        /// </summary>
        public void SetPreviewFootprint(GridFootprint footprint, bool isValid)
        {
            previewFootprint = footprint;
            isPreviewValid = isValid;
        }

        /// <summary>
        /// 清除预览占用区域。
        /// </summary>
        public void ClearPreviewFootprint()
        {
            previewFootprint = null;
            isPreviewValid = false;
        }

        /// <summary>
        /// 添加一片占地。
        /// </summary>
        /// <param name="footprint"></param>
        public void AddFootprint(GridFootprint footprint)
        {
            occupiedFootprints.Add(footprint);
            OnOccupiedChanged?.Invoke(occupiedFootprints);
        }
        /// <summary>
        /// 移除一片占地。
        /// </summary>
        /// <param name="footprint"></param>
        public void RemoveFootprint(GridFootprint footprint)
        {
            occupiedFootprints.Remove(footprint);
            OnOccupiedChanged?.Invoke(occupiedFootprints);
        }
        #endregion


    }
}
