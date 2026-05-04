using UnityEngine;

namespace GameDesign4.Grid.Contracts.Model
{
    /// <summary>
    /// 网格占地描述。
    /// 使用锚点格和尺寸描述一片连续占地。
    /// </summary>
    public readonly struct GridFootprint
    {
        /// <summary>
        /// 构造网格占地。
        /// </summary>
        public GridFootprint(GridCoord anchorCoord, Vector2Int size)
        {
            AnchorCoord = anchorCoord;
            Size = size;
        }

        /// <summary>
        /// 占地锚点格。
        /// </summary>
        public GridCoord AnchorCoord { get; }

        /// <summary>
        /// 占地尺寸。
        /// </summary>
        public Vector2Int Size { get; }
    }
}
