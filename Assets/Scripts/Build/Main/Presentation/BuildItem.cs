using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameDesign4.Build.Presentation
{
    /// <summary>
    /// 建造面板条目控制器。
    /// 负责展示单个建筑条目名称，并响应点击。
    /// </summary>
    public sealed class BuildItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Button selectButton;

        #region 条目绑定
        /// <summary>
        /// 绑定条目显示内容和点击行为。
        /// </summary>
        public void Bind(string displayName, Action onClicked)
        {
            nameText.text = displayName;

            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => onClicked?.Invoke());
        }
        #endregion
    }
}
