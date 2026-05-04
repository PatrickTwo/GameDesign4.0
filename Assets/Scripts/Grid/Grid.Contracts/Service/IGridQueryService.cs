using System.Collections.Generic;
using GameDesign4.Grid.Contracts.Model;
using UnityEngine;

namespace GameDesign4.Grid.Contracts.Service
{
    /// <summary>
    /// 网格查询服务接口。
    /// 负责统一提供坐标换算、占地计算和运行时占格能力。
    /// </summary>
    public interface IGridQueryService
    {
        #region 坐标换算
        /// <summary>
        /// 尝试将世界坐标转换为网格坐标。
        /// </summary>
        bool TryWorldToCoord(Vector3 worldPosition, out GridCoord gridCoord);

        /// <summary>
        /// 将网格坐标转换为格心世界坐标。
        /// </summary>
        Vector3 CoordToWorldCenter(GridCoord gridCoord, float worldY);

        /// <summary>
        /// 将世界坐标吸附到最近格心。
        /// </summary>
        Vector3 SnapWorldToCenter(Vector3 worldPosition);

        /// <summary>
        /// 获取占地矩形中心点。
        /// </summary>
        Vector3 GetFootprintWorldCenter(GridFootprint footprint, float worldY);
        #endregion

        #region 占地查询
        /// <summary>
        /// 计算占地覆盖的全部格坐标。
        /// </summary>
        IReadOnlyList<GridCoord> GetFootprintCoords(GridFootprint footprint);

        /// <summary>
        /// 判断指定格是否在网格范围内。
        /// </summary>
        bool IsCoordInBounds(GridCoord gridCoord);

        /// <summary>
        /// 判断整片占地是否在网格范围内。
        /// </summary>
        bool IsFootprintInBounds(GridFootprint footprint);

        /// <summary>
        /// 判断指定格是否已被占用。
        /// </summary>
        bool IsCoordOccupied(GridCoord gridCoord);

        /// <summary>
        /// 判断整片占地是否存在占用冲突。
        /// </summary>
        bool IsFootprintOccupied(GridFootprint footprint);
        #endregion

        #region 占格写入
        /// <summary>
        /// 记录一片已占用占地。
        /// </summary>
        void OccupyFootprint(GridFootprint footprint);

        /// <summary>
        /// 释放一片已占用占地。
        /// </summary>
        void ReleaseFootprint(GridFootprint footprint);

        /// <summary>
        /// 清空全部占格记录。
        /// </summary>
        void ClearOccupancy();
        #endregion
    }
}
