using GameDesign4.Infrastructure.Utilities;
using GameDesign4.UI.Definitions;
using UnityEngine;

namespace GameDesign4.UI.Presentation
{
    /// <summary>
    /// UI 根节点组件。
    /// 负责通过 Inspector 绑定各层节点，并为运行时提供统一查询入口。
    /// </summary>
    public sealed class UIRoot : MonoBehaviour
    {
        [SerializeField] private Transform hudLayer;
        [SerializeField] private Transform normalLayer;
        [SerializeField] private Transform popupLayer;
        [SerializeField] private Transform overlayLayer;

        #region UI层级查询
        /// <summary>
        /// 根据层级枚举获取对应的层级根节点。
        /// </summary>
        public Transform GetLayerRoot(UiLayerType layer)
        {
            Transform layerRoot = layer switch
            {
                UiLayerType.Hud => hudLayer,
                UiLayerType.Normal => normalLayer,
                UiLayerType.Popup => popupLayer,
                UiLayerType.Overlay => overlayLayer,
                _ => null
            };

            // 运行时必须拿到有效层节点，否则面板无法挂载到正确层级。
            Guard.EnsureNotNull(layerRoot, $"未绑定 UI 层级节点：{layer}");
            return layerRoot;
        }
        #endregion
    }
}
