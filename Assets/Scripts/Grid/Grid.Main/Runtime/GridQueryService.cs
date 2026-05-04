using System.Collections.Generic;
using GameDesign4.Grid.Contracts.Model;
using GameDesign4.Grid.Contracts.Service;
using GameDesign4.Grid.Definition;
using UnityEngine;

namespace GameDesign4.Grid.Runtime
{
    /// <summary>
    /// 网格查询服务。
    /// 负责提供坐标换算、占地计算和运行时占格能力。
    /// </summary>
    public sealed class GridQueryService : IGridQueryService
    {
        private readonly GridDefinition gridDefinition;
        private readonly GridState gridState;

        /// <summary>
        /// 构造网格查询服务。
        /// </summary>
        public GridQueryService(GridDefinition gridDefinition, GridState gridState)
        {
            this.gridDefinition = gridDefinition;
            this.gridState = gridState;
        }

        #region 坐标换算
        /// <summary>
        /// 尝试将世界坐标转换为网格坐标。
        /// </summary>
        public bool TryWorldToCoord(Vector3 worldPosition, out GridCoord gridCoord)
        {
            // 先将世界坐标转换为以网格原点为基准的本地偏移。
            float localX = worldPosition.x - gridDefinition.GridOrigin.x;
            float localY = worldPosition.z - gridDefinition.GridOrigin.y;
            int coordX = Mathf.FloorToInt(localX / gridDefinition.CellSize);
            int coordY = Mathf.FloorToInt(localY / gridDefinition.CellSize);
            GridCoord candidateCoord = new GridCoord(coordX, coordY);

            // 只有落在网格有效范围内时，才认为命中了有效格子。
            if (IsCoordInBounds(candidateCoord) == false)
            {
                gridCoord = default;
                return false;
            }

            gridCoord = candidateCoord;
            return true;
        }

        /// <summary>
        /// 将网格坐标转换为格心世界坐标。
        /// </summary>
        public Vector3 CoordToWorldCenter(GridCoord gridCoord, float worldY)
        {
            // 格心坐标按原点、格索引和单格尺寸直接回算。
            float centerX = gridDefinition.GridOrigin.x + ((gridCoord.X + 0.5f) * gridDefinition.CellSize);
            float centerZ = gridDefinition.GridOrigin.y + ((gridCoord.Y + 0.5f) * gridDefinition.CellSize);
            return new Vector3(centerX, worldY, centerZ);
        }

        /// <summary>
        /// 将世界坐标吸附到最近格心。
        /// </summary>
        public Vector3 SnapWorldToCenter(Vector3 worldPosition)
        {
            // 先找所在格，再返回对应格心。
            bool hasCoord = TryWorldToCoord(worldPosition, out GridCoord gridCoord);
            return hasCoord ? CoordToWorldCenter(gridCoord, worldPosition.y) : worldPosition;
        }

        /// <summary>
        /// 获取占地矩形中心点。
        /// </summary>
        public Vector3 GetFootprintWorldCenter(GridFootprint footprint, float worldY)
        {
            // 占地中心由左下锚点和尺寸共同决定。
            float centerX = gridDefinition.GridOrigin.x
                + ((footprint.AnchorCoord.X + (footprint.Size.x * 0.5f)) * gridDefinition.CellSize);
            float centerZ = gridDefinition.GridOrigin.y
                + ((footprint.AnchorCoord.Y + (footprint.Size.y * 0.5f)) * gridDefinition.CellSize);
            return new Vector3(centerX, worldY, centerZ);
        }
        #endregion

        #region 占地查询
        /// <summary>
        /// 计算占地覆盖的全部格坐标。
        /// </summary>
        public IReadOnlyList<GridCoord> GetFootprintCoords(GridFootprint footprint)
        {
            List<GridCoord> coords = new List<GridCoord>(footprint.Size.x * footprint.Size.y);

            // 从左下锚点开始，按矩形尺寸展开所有格坐标。
            for (int y = 0; y < footprint.Size.y; y++)
            {
                for (int x = 0; x < footprint.Size.x; x++)
                {
                    coords.Add(new GridCoord(footprint.AnchorCoord.X + x, footprint.AnchorCoord.Y + y));
                }
            }

            return coords;
        }

        /// <summary>
        /// 判断指定格是否在网格范围内。
        /// </summary>
        public bool IsCoordInBounds(GridCoord gridCoord)
        {
            // 横纵索引都必须落在逻辑网格宽高之内。
            return gridCoord.X >= 0
                && gridCoord.X < gridDefinition.GridWidth
                && gridCoord.Y >= 0
                && gridCoord.Y < gridDefinition.GridHeight;
        }

        /// <summary>
        /// 判断整片占地是否在网格范围内。
        /// </summary>
        public bool IsFootprintInBounds(GridFootprint footprint)
        {
            IReadOnlyList<GridCoord> coords = GetFootprintCoords(footprint);
            for (int index = 0; index < coords.Count; index++)
            {
                if (IsCoordInBounds(coords[index]) == false)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 判断指定格是否已被占用。
        /// </summary>
        public bool IsCoordOccupied(GridCoord gridCoord)
        {
            IReadOnlyList<GridFootprint> occupiedFootprints = gridState.OccupiedFootprints;
            for (int footprintIndex = 0; footprintIndex < occupiedFootprints.Count; footprintIndex++)
            {
                IReadOnlyList<GridCoord> occupiedCoords = GetFootprintCoords(occupiedFootprints[footprintIndex]);
                for (int coordIndex = 0; coordIndex < occupiedCoords.Count; coordIndex++)
                {
                    if (occupiedCoords[coordIndex] == gridCoord)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 判断整片占地是否存在占用冲突。
        /// </summary>
        public bool IsFootprintOccupied(GridFootprint footprint)
        {
            IReadOnlyList<GridCoord> coords = GetFootprintCoords(footprint);
            for (int index = 0; index < coords.Count; index++)
            {
                if (IsCoordOccupied(coords[index]))
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region 占格写入
        /// <summary>
        /// 记录一片已占用占地。
        /// </summary>
        public void OccupyFootprint(GridFootprint footprint)
        {
            // 直接写入共享状态，供查询与显示共用。
            gridState.AddFootprint(footprint);
        }

        /// <summary>
        /// 释放一片已占用占地。
        /// </summary>
        public void ReleaseFootprint(GridFootprint footprint)
        {
            // 当前阶段释放逻辑只需回写共享状态。
            gridState.RemoveFootprint(footprint);
        }

        /// <summary>
        /// 清空全部占格记录。
        /// </summary>
        public void ClearOccupancy()
        {
            // 当前阶段直接清空共享状态中的占格列表。
            gridState.ClearOccupiedFootprints();
        }
        #endregion
    }
}
