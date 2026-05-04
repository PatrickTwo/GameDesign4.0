using System.Collections.Generic;
using GameDesign4.Grid.Contracts.Model;
using GameDesign4.Grid.Definition;
using GameDesign4.Infrastructure.Utilities;
using UnityEngine;
using VContainer;

namespace GameDesign4.Grid.Presentation
{
    /// <summary>
    /// 网格视图。
    /// 使用场景中预配置的 Quad 物体执行基础网格与高亮显示。
    /// 读取 GridState 数据驱动渲染：悬停与预览每帧轮询，占用标记由事件驱动。
    /// </summary>
    public sealed class GridView : MonoBehaviour
    {
        private static readonly int GridWidthShaderId = Shader.PropertyToID("_GridWidth");
        private static readonly int GridHeightShaderId = Shader.PropertyToID("_GridHeight");
        private static readonly int GridLineThicknessShaderId = Shader.PropertyToID("_GridLineThickness");
        private static readonly int GridLineColorShaderId = Shader.PropertyToID("_GridLineColor");
        private static readonly int GridFillColorShaderId = Shader.PropertyToID("_GridFillColor");

        [Header("网格显示根节点")]
        [SerializeField] private GameObject gridRoot;
        [Header("网格定义")]
        [SerializeField] private GridDefinition gridDefinition;
        [Header("网格显示")]
        [SerializeField] private Transform gridSurfaceQuad;
        [Header("占用显示容器")]
        [SerializeField] private Transform occupiedHighlightContainer;
        [Header("悬停与预览")]
        [SerializeField] private GameObject hoverQuad;
        [SerializeField] private GameObject validPreviewQuad;
        [SerializeField] private GameObject invalidPreviewQuad;
        [Header("Quad 预制体")]
        [SerializeField] private GameObject quadPrefab;

        private GridState gridState;
        // 占用标记实例池
        private readonly List<GameObject> occupiedInstances = new List<GameObject>();

        [Inject]
        private void Construct(GridState gridState)
        {
            this.gridState = gridState;
        }

        #region 生命周期
        private void Awake()
        {
            // 初始化基础网格面片。
            Vector3 center = GridViewUtility.GetGridCenter(gridDefinition, gridDefinition.LineYOffset);
            Vector2 size = GridViewUtility.GetGridSize(gridDefinition);
            GridViewUtility.SetQuadTransform(gridSurfaceQuad, center, size);
            ConfigureGridSurfaceMaterial();

            hoverQuad.SetActive(false);
            validPreviewQuad.SetActive(false);
            invalidPreviewQuad.SetActive(false);

            // 订阅占用变更事件。
            gridState.OnOccupiedChanged += HandleOccupiedChanged;

            // 默认隐藏网格。
            gridRoot.SetActive(false);
        }

        private void OnDestroy()
        {
            gridState.OnOccupiedChanged -= HandleOccupiedChanged;
        }

        private void Update()
        {
            gridRoot.SetActive(gridState.IsGridVisible);
            if (!gridState.IsGridVisible)
            {
                return;
            }
            RefreshHover();
            RefreshPreview();
        }
        #endregion

        #region 轮询刷新
        /// <summary>
        /// 初始化底面网格线材质。
        /// </summary>
        private void ConfigureGridSurfaceMaterial()
        {
            MeshRenderer gridSurfaceRenderer = gridSurfaceQuad.GetComponent<MeshRenderer>();
            Material gridSurfaceMaterial = gridSurfaceRenderer.sharedMaterial;
            ApplyGridSurfaceMaterialProperties(gridSurfaceMaterial);
        }

        /// <summary>
        /// 将网格定义参数写入底面材质。
        /// </summary>
        private void ApplyGridSurfaceMaterialProperties(Material targetMaterial)
        {
            // 使用逻辑网格尺寸和显示参数驱动整张底面上的网格线绘制。
            targetMaterial.SetFloat(GridWidthShaderId, gridDefinition.GridWidth);
            targetMaterial.SetFloat(GridHeightShaderId, gridDefinition.GridHeight);
            targetMaterial.SetFloat(GridLineThicknessShaderId, gridDefinition.LineThickness);
            targetMaterial.SetColor(GridLineColorShaderId, gridDefinition.LineColor);
            targetMaterial.SetColor(GridFillColorShaderId, gridDefinition.FillColor);
        }

        /// <summary>
        /// 刷新悬停高亮。
        /// </summary>
        private void RefreshHover()
        {
            if (gridState.HoverCoord.HasValue)
            {
                hoverQuad.SetActive(true);
                Vector3 center = GridViewUtility.GetCellCenter(
                    gridDefinition, gridState.HoverCoord.Value, gridDefinition.QuadYOffset);
                Vector2 cellSize = GridViewUtility.GetScaledCellSize(
                    gridDefinition, gridDefinition.HoverQuadScale);
                GridViewUtility.SetQuadTransform(hoverQuad.transform, center, cellSize);
            }
            else
            {
                hoverQuad.SetActive(false);
            }
        }

        /// <summary>
        /// 刷新预览高亮。
        /// </summary>
        private void RefreshPreview()
        {
            if (gridState.PreviewFootprint.HasValue)
            {
                bool isValid = gridState.IsPreviewValid;
                validPreviewQuad.SetActive(isValid);
                invalidPreviewQuad.SetActive(!isValid);
                Transform previewQuad = isValid ? validPreviewQuad.transform : invalidPreviewQuad.transform;

                GridFootprint footprint = gridState.PreviewFootprint.Value;
                Vector3 center = GridViewUtility.GetFootprintCenter(gridDefinition, footprint, gridDefinition.QuadYOffset);
                Vector2 size = GridViewUtility.GetScaledFootprintSize(gridDefinition, footprint, gridDefinition.FootprintQuadScale);
                GridViewUtility.SetQuadTransform(previewQuad.transform, center, size);
            }
            else
            {
                validPreviewQuad.SetActive(false);
                invalidPreviewQuad.SetActive(false);
            }
        }
        #endregion

        #region 事件响应
        /// <summary>
        /// 响应占用标记变更，重建占用高亮实例。
        /// </summary>
        private void HandleOccupiedChanged(IReadOnlyList<GridFootprint> footprints)
        {
            // HACK 后续可考虑只处理变更的占用标记，而不是全部重建。
            // 销毁旧实例。
            for (int i = occupiedInstances.Count - 1; i >= 0; i--)
            {
                Destroy(occupiedInstances[i]);
            }
            occupiedInstances.Clear();

            // 重建占用标记。
            foreach (GridFootprint footprint in footprints)
            {
                GameObject instance = Instantiate(quadPrefab, occupiedHighlightContainer);
                Vector3 center = GridViewUtility.GetFootprintCenter(
                    gridDefinition, footprint, gridDefinition.QuadYOffset);
                Vector2 size = GridViewUtility.GetScaledFootprintSize(
                    gridDefinition, footprint, gridDefinition.FootprintQuadScale);
                GridViewUtility.SetQuadTransform(instance.transform, center, size);
                occupiedInstances.Add(instance);
            }
        }
        #endregion
    }
}
