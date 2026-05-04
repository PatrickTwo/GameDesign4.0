using System.Collections.Generic;
using GameDesign4.Infrastructure.Definitions;
using UnityEngine;

namespace GameDesign4.Grid.Definition
{
    /// <summary>
    /// 网格定义。
    /// 负责存储逻辑网格参数和基础显示参数。
    /// </summary>
    [CreateAssetMenu(menuName = "GameDesign4/Grid/Grid Definition")]
    public sealed class GridDefinition : ScriptableObject, IDataValidationSelfCheck
    {
        [Header("逻辑网格")]
        [SerializeField] private Vector2 gridOrigin = Vector2.zero;
        [SerializeField] private int gridWidth = 1;
        [SerializeField] private int gridHeight = 1;
        [SerializeField] private float cellSize = 1.0f;

        [Header("显示偏移")]
        [SerializeField] private float lineYOffset = 0.05f;
        [SerializeField] private float quadYOffset = 0.06f;
        
        [Header("网格线显示")]
        [SerializeField] private Color lineColor = new Color(0.0f, 0.82f, 1.0f, 0.65f);
        [SerializeField] private Color fillColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        [SerializeField] private float lineThickness = 0.02f;

        [Header("显示缩放")]
        [SerializeField] private Vector2 hoverQuadScale = new Vector2(0.92f, 0.92f);
        [SerializeField] private Vector2 footprintQuadScale = new Vector2(0.92f, 0.92f);

        #region 对外属性
        /// <summary>
        /// 网格原点。
        /// </summary>
        public Vector2 GridOrigin => gridOrigin;

        /// <summary>
        /// 网格宽度。
        /// </summary>
        public int GridWidth => gridWidth;

        /// <summary>
        /// 网格高度。
        /// </summary>
        public int GridHeight => gridHeight;

        /// <summary>
        /// 单格尺寸。
        /// </summary>
        public float CellSize => cellSize;

        /// <summary>
        /// 网格线高度偏移。
        /// </summary>
        public float LineYOffset => lineYOffset;

        /// <summary>
        /// 高亮面片高度偏移。
        /// </summary>
        public float QuadYOffset => quadYOffset;

        /// <summary>
        /// 网格线颜色。
        /// </summary>
        public Color LineColor => lineColor;

        /// <summary>
        /// 网格底面填充色。
        /// </summary>
        public Color FillColor => fillColor;

        /// <summary>
        /// 网格线宽度。
        /// </summary>
        public float LineThickness => lineThickness;

        /// <summary>
        /// 悬停格缩放。
        /// </summary>
        public Vector2 HoverQuadScale => hoverQuadScale;

        /// <summary>
        /// 占地缩放。
        /// </summary>
        public Vector2 FootprintQuadScale => footprintQuadScale;
        #endregion
        
        #region 资产自校验
        /// <summary>
        /// 执行当前网格定义的本地校验。
        /// </summary>
        public void ValidateSelf(List<string> issues)
        {
            // 网格宽度必须大于零，否则无法形成有效逻辑区域。
            if (gridWidth <= 0)
            {
                issues.Add($"网格宽度必须大于 0：{name}");
            }

            // 网格高度必须大于零，否则无法形成有效逻辑区域。
            if (gridHeight <= 0)
            {
                issues.Add($"网格高度必须大于 0：{name}");
            }

            // 单格尺寸必须大于零，否则坐标换算将失效。
            if (cellSize <= 0f)
            {
                issues.Add($"网格单格尺寸必须大于 0：{name}");
            }

            // 网格线宽度必须大于零，否则不会产生可见线条。
            if (lineThickness <= 0f)
            {
                issues.Add($"网格线宽度必须大于 0：{name}");
            }
        }
        #endregion
    }
}
