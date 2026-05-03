using UnityEngine;

namespace GameDesign4.SceneInteract.Runtime
{
    /// <summary>
    /// 场景可选择对象组件。
    /// 负责声明对象可被场景交互系统选择，并将命中的子物体映射到真正的选择目标。
    /// </summary>
    public sealed class SceneSelectable : MonoBehaviour
    {
        private bool isSelected;


        /// <summary>
        /// 当前是否已被选中。
        /// </summary>
        public bool IsSelected => isSelected;

        #region 选择状态控制


        #endregion
    }
}
