using GameDesign4.Grid.Contracts.Model;
using GameDesign4.Grid.Definition;
using UnityEngine;

namespace GameDesign4.Grid.Presentation
{
    /// <summary>
    /// 网格视图计算工具。
    /// 负责集中处理网格显示相关的几何计算与 Quad 摆放。
    /// </summary>
    public static class GridViewUtility
    {
        #region 网格尺寸
        /// <summary>
        /// 获取整张网格中心点。
        /// </summary>
        public static Vector3 GetGridCenter(GridDefinition gridDefinition, float worldY)
        {
            // 整张网格中心点由原点、宽高和单格尺寸共同决定。
            float width = gridDefinition.GridWidth * gridDefinition.CellSize;
            float height = gridDefinition.GridHeight * gridDefinition.CellSize;
            float centerX = gridDefinition.GridOrigin.x + (width * 0.5f);
            float centerZ = gridDefinition.GridOrigin.y + (height * 0.5f);
            return new Vector3(centerX, worldY, centerZ);
        }

        /// <summary>
        /// 获取整张网格尺寸。
        /// </summary>
        public static Vector2 GetGridSize(GridDefinition gridDefinition)
        {
            // 基础网格尺寸与逻辑网格宽高完全一致。
            float width = gridDefinition.GridWidth * gridDefinition.CellSize;
            float height = gridDefinition.GridHeight * gridDefinition.CellSize;
            return new Vector2(width, height);
        }
        #endregion

        #region 单格与占地
        /// <summary>
        /// 获取指定格中心点。
        /// </summary>
        public static Vector3 GetCellCenter(GridDefinition gridDefinition, GridCoord gridCoord, float worldY)
        {
            // 单格中心点直接按格索引回算。
            float centerX = gridDefinition.GridOrigin.x + ((gridCoord.X + 0.5f) * gridDefinition.CellSize);
            float centerZ = gridDefinition.GridOrigin.y + ((gridCoord.Y + 0.5f) * gridDefinition.CellSize);
            return new Vector3(centerX, worldY, centerZ);
        }

        /// <summary>
        /// 获取占地矩形中心点。
        /// </summary>
        public static Vector3 GetFootprintCenter(GridDefinition gridDefinition, GridFootprint footprint, float worldY)
        {
            // 占地中心点由锚点与占地尺寸共同决定。
            float centerX = gridDefinition.GridOrigin.x
                + ((footprint.AnchorCoord.X + (footprint.Size.x * 0.5f)) * gridDefinition.CellSize);
            float centerZ = gridDefinition.GridOrigin.y
                + ((footprint.AnchorCoord.Y + (footprint.Size.y * 0.5f)) * gridDefinition.CellSize);
            return new Vector3(centerX, worldY, centerZ);
        }

        /// <summary>
        /// 获取缩放后的单格尺寸。
        /// </summary>
        public static Vector2 GetScaledCellSize(GridDefinition gridDefinition, Vector2 scale)
        {
            // 单格高亮直接使用单格尺寸乘缩放因子。
            float width = gridDefinition.CellSize * scale.x;
            float height = gridDefinition.CellSize * scale.y;
            return new Vector2(width, height);
        }

        /// <summary>
        /// 获取缩放后的占地尺寸。
        /// </summary>
        public static Vector2 GetScaledFootprintSize(GridDefinition gridDefinition, GridFootprint footprint, Vector2 scale)
        {
            // 占地高亮直接使用整块矩形尺寸乘缩放因子。
            float width = footprint.Size.x * gridDefinition.CellSize * scale.x;
            float height = footprint.Size.y * gridDefinition.CellSize * scale.y;
            return new Vector2(width, height);
        }
        #endregion

        #region Quad 摆放
        /// <summary>
        /// 设置 Quad 位置与尺寸。
        /// </summary>
        public static void SetQuadTransform(Transform targetTransform, Vector3 center, Vector2 size)
        {
            // Quad 默认尺寸为 1x1，缩放直接对应世界尺寸。
            targetTransform.position = center;
            targetTransform.localScale = new Vector3(size.x, size.y, 1.0f);
        }
        #endregion
    }
}
