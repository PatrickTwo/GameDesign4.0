using System.Collections.Generic;
using GameDesign4.Input.Contracts;
using GameDesign4.UI.Definitions;

namespace GameDesign4.UI.Runtime
{
    /// <summary>
    /// UI 控制器。
    /// 负责处理 UI 快捷键、互斥策略、打开顺序与对外控制接口。
    /// </summary>
    public sealed class UIController : IUIController, IUIInputConsumer
    {
        private readonly Dictionary<string, List<string>> panelExclusive = new Dictionary<string, List<string>>();
        private readonly HashSet<string> closeByCancelPanelIds = new HashSet<string>();
        private readonly List<string> openedPanelOrder = new List<string>();
        private readonly UIService uiService;

        /// <summary>
        /// 构造 UI 控制器。
        /// </summary>
        public UIController(UiPanelCatalogDef uiPanelCatalog, UIService uiService)
        {
            this.uiService = uiService;
            InitializePanelPolicies(uiPanelCatalog);
        }

        #region 配置初始化
        /// <summary>
        /// 初始化面板互斥关系与取消关闭策略。
        /// </summary>
        private void InitializePanelPolicies(UiPanelCatalogDef uiPanelCatalog)
        {
            IReadOnlyList<UiPanelEntryDef> panelEntries = uiPanelCatalog.PanelEntries;
            for (int index = 0; index < panelEntries.Count; index++)
            {
                UiPanelEntryDef panelEntry = panelEntries[index];
                if (panelEntry == null || string.IsNullOrWhiteSpace(panelEntry.PanelId))
                {
                    continue;
                }

                panelExclusive[panelEntry.PanelId] = new List<string>(panelEntry.ExclusivePanelIds);
                if (panelEntry.CloseByCancel)
                {
                    closeByCancelPanelIds.Add(panelEntry.PanelId);
                }
            }
        }
        #endregion

        #region IUIController
        /// <summary>
        /// 判断指定面板当前是否处于打开状态。
        /// </summary>
        public bool IsPanelOpen(string panelId)
        {
            return uiService.IsPanelOpen(panelId);
        }

        /// <summary>
        /// 打开指定面板，并同步维护顺序栈。
        /// </summary>
        public void OpenPanel(string panelId)
        {
            if (uiService.IsPanelOpen(panelId))
            {
                RefreshPanelOrder(panelId);
                return;
            }

            if (panelExclusive.TryGetValue(panelId, out List<string> exclusivePanelIds))
            {
                for (int index = 0; index < exclusivePanelIds.Count; index++)
                {
                    ClosePanel(exclusivePanelIds[index]);
                }
            }

            uiService.OpenPanel(panelId);
            if (uiService.IsPanelOpen(panelId))
            {
                RefreshPanelOrder(panelId);
            }
        }

        /// <summary>
        /// 关闭指定面板，并从顺序栈移除。
        /// </summary>
        public void ClosePanel(string panelId)
        {
            if (uiService.IsPanelOpen(panelId) == false)
            {
                openedPanelOrder.Remove(panelId);
                return;
            }

            uiService.ClosePanel(panelId);
            openedPanelOrder.Remove(panelId);
        }

        /// <summary>
        /// 按最近打开顺序关闭一个允许取消关闭的面板。
        /// </summary>
        public bool CloseLastOpenedPanel()
        {
            for (int index = openedPanelOrder.Count - 1; index >= 0; index--)
            {
                string panelId = openedPanelOrder[index];
                if (closeByCancelPanelIds.Contains(panelId) == false)
                {
                    openedPanelOrder.RemoveAt(index);
                    continue;
                }

                if (uiService.IsPanelOpen(panelId) == false)
                {
                    openedPanelOrder.RemoveAt(index);
                    continue;
                }

                ClosePanel(panelId);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 切换指定面板的打开状态。
        /// </summary>
        public void TogglePanel(string panelId)
        {
            if (uiService.IsPanelOpen(panelId))
            {
                ClosePanel(panelId);
                return;
            }

            OpenPanel(panelId);
        }
        #endregion

        #region IUIInputConsumer
        /// <summary>
        /// 处理建造面板切换输入。
        /// </summary>
        public void HandleToggleBuildPanel()
        {
            TogglePanel(UIPanelId.Build);
        }

        /// <summary>
        /// 处理仓库面板切换输入。
        /// </summary>
        public void HandleToggleInventoryPanel()
        {
            TogglePanel(UIPanelId.Inventory);
        }

        /// <summary>
        /// 处理生产面板切换输入。
        /// </summary>
        public void HandleToggleProductionPanel()
        {
            TogglePanel(UIPanelId.Production);
        }

        /// <summary>
        /// 处理取消输入。
        /// </summary>
        public bool HandleCancelAction()
        {
            return CloseLastOpenedPanel();
        }
        #endregion

        #region 顺序维护
        /// <summary>
        /// 刷新面板最近打开顺序。
        /// </summary>
        private void RefreshPanelOrder(string panelId)
        {
            openedPanelOrder.Remove(panelId);
            openedPanelOrder.Add(panelId);
        }
        #endregion
    }
}
