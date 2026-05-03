using UnityEngine;

namespace GameDesign4.SceneInteract.Runtime
{
    /// <summary>
    /// 场景可选择对象组件。
    /// 负责声明对象可被场景交互系统选择，并将命中的子物体映射到真正的选择目标。
    /// </summary>
    public sealed class SceneSelectable : MonoBehaviour
    {
        [SerializeField] private SceneSelectableType targetType = SceneSelectableType.Other;
        [SerializeField] private GameObject targetObject;
        [SerializeField] private GameObject selectionIndicator;

        private bool isSelected;

        /// <summary>
        /// 选择目标类型。
        /// </summary>
        public SceneSelectableType TargetType => targetType;

        /// <summary>
        /// 真正的选择目标对象。
        /// 未显式指定时默认返回当前对象。
        /// </summary>
        public GameObject TargetObject => targetObject != null ? targetObject : gameObject;

        /// <summary>
        /// 当前是否已被选中。
        /// </summary>
        public bool IsSelected => isSelected;

        #region 选择状态控制
        /// <summary>
        /// 设置当前选择状态，并切换指示器显隐。
        /// </summary>
        public void SetSelected(bool selected)
        {
            isSelected = selected;
            SetIndicatorVisible(selected);
        }

        /// <summary>
        /// 清空当前选择状态并隐藏指示器。
        /// </summary>
        public void ClearSelection()
        {
            isSelected = false;
            SetIndicatorVisible(false);
        }

        /// <summary>
        /// 设置选择指示器的显隐状态。
        /// </summary>
        private void SetIndicatorVisible(bool visible)
        {
            if (selectionIndicator != null)
            {
                selectionIndicator.SetActive(visible);
            }
        }
        #endregion
    }
}
