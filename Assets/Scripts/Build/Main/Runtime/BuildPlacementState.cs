using GameDesign4.Build.Definition;
using GameDesign4.Grid.Contracts.Model;
using UnityEngine;

namespace GameDesign4.Build.Runtime
{
    /// <summary>
    /// 建造放置状态。
    /// 负责保存当前蓝图、预览实例与预览坐标等运行时信息。
    /// </summary>
    public sealed class BuildPlacementState
    {
        /// <summary>
        /// 当前是否处于建造模式。
        /// </summary>
        public bool IsPlacementActive => CurrentBlueprint != null;

        /// <summary>
        /// 当前正在放置的蓝图。
        /// </summary>
        public BuildingBlueprintDef CurrentBlueprint { get; private set; }

        /// <summary>
        /// 当前预览实例。
        /// </summary>
        public GameObject PreviewInstance { get; private set; }

        /// <summary>
        /// 当前预览坐标。
        /// </summary>
        public Vector3 PreviewPosition { get; private set; }

        /// <summary>
        /// 当前是否存在有效预览坐标。
        /// </summary>
        public bool HasPreviewPosition { get; private set; }

        /// <summary>
        /// 当前吸附到的格坐标。
        /// </summary>
        public GridCoord? PreviewCoord { get; private set; }

        /// <summary>
        /// 当前预览占地。
        /// </summary>
        public GridFootprint? PreviewFootprint { get; private set; }

        /// <summary>
        /// 当前预览是否有效。
        /// </summary>
        public bool IsPreviewValid { get; private set; }

        /// <summary>
        /// 当前放置会话版本号。
        /// 用于防止异步加载完成后把旧预览挂回新状态。
        /// </summary>
        public int PlacementVersion { get; private set; }

        #region 状态切换
        /// <summary>
        /// 开始一次新的建造放置。
        /// </summary>
        public void BeginPlacement(BuildingBlueprintDef blueprint)
        {
            PlacementVersion++;
            CurrentBlueprint = blueprint;
            PreviewInstance = null;
            PreviewPosition = Vector3.zero;
            HasPreviewPosition = false;
            PreviewCoord = null;
            PreviewFootprint = null;
            IsPreviewValid = false;
        }

        /// <summary>
        /// 绑定当前预览实例。
        /// </summary>
        public void AttachPreview(GameObject previewInstance)
        {
            PreviewInstance = previewInstance;
        }

        /// <summary>
        /// 记录当前预览状态。
        /// </summary>
        public void UpdatePreviewState(Vector3 previewPosition, bool hasPreviewPosition)
        {
            PreviewPosition = previewPosition;
            HasPreviewPosition = hasPreviewPosition;
        }

        /// <summary>
        /// 更新当前网格预览状态。
        /// </summary>
        public void UpdateGridPreviewState(GridCoord? previewCoord, GridFootprint? previewFootprint, bool isPreviewValid)
        {
            PreviewCoord = previewCoord;
            PreviewFootprint = previewFootprint;
            IsPreviewValid = isPreviewValid;
        }

        /// <summary>
        /// 分离当前预览实例。
        /// </summary>
        public GameObject DetachPreview()
        {
            GameObject previewInstance = PreviewInstance;
            PreviewInstance = null;
            return previewInstance;
        }

        /// <summary>
        /// 清空当前建造状态。
        /// </summary>
        public void Clear()
        {
            CurrentBlueprint = null;
            PreviewInstance = null;
            PreviewPosition = Vector3.zero;
            HasPreviewPosition = false;
            PreviewCoord = null;
            PreviewFootprint = null;
            IsPreviewValid = false;
        }
        #endregion
    }
}
