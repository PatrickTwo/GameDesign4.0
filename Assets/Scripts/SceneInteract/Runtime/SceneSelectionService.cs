namespace GameDesign4.SceneInteract.Runtime
{
    /// <summary>
    /// 场景选择服务。
    /// 负责维护当前单选目标并切换选中表现。
    /// </summary>
    public sealed class SceneSelectionService
    {
        private SceneSelectable currentSelection;

        /// <summary>
        /// 当前选中的对象。
        /// </summary>
        public SceneSelectable CurrentSelection => currentSelection;

        /// <summary>
        /// 当前是否存在有效选中对象。
        /// </summary>
        public bool HasSelection => currentSelection != null;

        #region 选择状态管理
        /// <summary>
        /// 选中指定对象。
        /// </summary>
        public void Select(SceneSelectable selectable)
        {
            if (selectable == currentSelection)
            {
                return;
            }

            ClearSelection();
            currentSelection = selectable;
            if (currentSelection != null)
            {
                currentSelection.SetSelected(true);
            }
        }

        /// <summary>
        /// 清空当前选中对象。
        /// </summary>
        public void ClearSelection()
        {
            if (currentSelection == null)
            {
                return;
            }

            currentSelection.SetSelected(false);
            currentSelection = null;
        }
        #endregion
    }
}
