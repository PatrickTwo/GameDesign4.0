using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace GameDesign4.UI.Presentation
{
    /// <summary>
    /// HUD 面板控制器。
    /// 负责转发 HUD 上的建造、仓库、生产按钮点击事件。
    /// </summary>
    public sealed class HudPanelController : BasePanel
    {
        [SerializeField] private Button buildButton;
        [SerializeField] private Button inventoryButton;
        [SerializeField] private Button productionButton;

        private Runtime.IUIService uiPanelService;

        /// <summary>
        /// 当前面板唯一标识。
        /// </summary>
        public override string PanelId => Runtime.UIPanelId.Hud;

        #region 初始化
        /// <summary>
        /// 注入 HUD 所需的面板控制服务。
        /// </summary>
        [Inject]
        public void Construct(Runtime.IUIService uiPanelService)
        {
            this.uiPanelService = uiPanelService;
            BindButtons();
        }

        /// <summary>
        /// 绑定 HUD 按钮点击事件。
        /// </summary>
        private void BindButtons()
        {
            buildButton.onClick.RemoveAllListeners();
            inventoryButton.onClick.RemoveAllListeners();
            productionButton.onClick.RemoveAllListeners();

            // 统一由 HUD 把用户点击翻译成面板切换请求。
            buildButton.onClick.AddListener(() => uiPanelService.TogglePanel(Runtime.UIPanelId.Build));
            inventoryButton.onClick.AddListener(() => uiPanelService.TogglePanel(Runtime.UIPanelId.Inventory));
            productionButton.onClick.AddListener(() => uiPanelService.TogglePanel(Runtime.UIPanelId.Production));
        }
        #endregion
    }
}
