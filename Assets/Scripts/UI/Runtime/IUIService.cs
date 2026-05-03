namespace GameDesign4.UI.Runtime
{
    /// <summary>
    /// UI 面板控制服务接口。
    /// 负责向展示层暴露统一的面板打开、关闭和切换能力。
    /// </summary>
    public interface IUIService
    {
        /// <summary>
        /// 判断指定面板当前是否处于打开状态。
        /// </summary>
        bool IsPanelOpen(string panelId);

        /// <summary>
        /// 打开指定面板。
        /// </summary>
        void OpenPanel(string panelId);

        /// <summary>
        /// 关闭指定面板。
        /// </summary>
        void ClosePanel(string panelId);
        
        /// <summary>
        /// 关闭最近打开的面板。
        /// </summary>
        void CloseLastOpenedPanel();

        /// <summary>
        /// 切换指定面板的打开状态。
        /// </summary>
        void TogglePanel(string panelId);
    }
}
