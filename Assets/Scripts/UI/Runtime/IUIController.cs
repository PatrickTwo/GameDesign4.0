namespace GameDesign4.UI.Runtime
{
    /// <summary>
    /// UI 控制器接口。
    /// 负责对外暴露统一的面板查询、开关与顺序关闭能力。
    /// </summary>
    public interface IUIController
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
        /// 关闭最近打开的可取消面板。
        /// </summary>
        bool CloseLastOpenedPanel();

        /// <summary>
        /// 切换指定面板的打开状态。
        /// </summary>
        void TogglePanel(string panelId);
    }
}
