using UnityEngine;

namespace GameDesign4.UI.Presentation
{
    /// <summary>
    /// UI 面板控制器基类。
    /// 负责统一面板标识与打开关闭生命周期。
    /// </summary>
    public abstract class BasePanel : MonoBehaviour
    {
        /// <summary>
        /// 当前面板唯一标识。
        /// </summary>
        public abstract string PanelId { get; }

        #region 面板生命周期
        /// <summary>
        /// 打开面板。
        /// </summary>
        public void Open()
        {
            gameObject.SetActive(true);
            OnOpened();
        }

        /// <summary>
        /// 关闭面板。
        /// </summary>
        public void Close()
        {
            OnClosed();
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 当前面板是否处于打开状态。
        /// </summary>
        public bool IsOpen => gameObject.activeSelf;

        /// <summary>
        /// 面板打开后的扩展回调。
        /// </summary>
        protected virtual void OnOpened()
        {
        }

        /// <summary>
        /// 面板关闭前的扩展回调。
        /// </summary>
        protected virtual void OnClosed()
        {
        }
        #endregion
    }
}
