using TMPro;
using UnityEngine;

namespace GameDesign4.UI.Presentation
{
    /// <summary>
    /// 生产面板控制器。
    /// 当前阶段仅负责原型骨架显隐与标题占位。
    /// </summary>
    public sealed class ProductionPanelController : BasePanel
    {
        [SerializeField] private TMP_Text titleText;

        /// <summary>
        /// 当前面板唯一标识。
        /// </summary>
        public override string PanelId => Runtime.UIPanelId.Production;
    }
}
