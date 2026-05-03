using TMPro;
using UnityEngine;

namespace GameDesign4.UI.Presentation
{
    /// <summary>
    /// 仓库面板控制器。
    /// 当前阶段仅负责原型骨架显隐与标题占位。
    /// </summary>
    public sealed class InventoryPanelController : BasePanel
    {
        [SerializeField] private TMP_Text titleText;

        /// <summary>
        /// 当前面板唯一标识。
        /// </summary>
        public override string PanelId => Runtime.UIPanelId.Inventory;
    }
}
