using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameDesign4.Production.Presentation
{
    /// <summary>
    /// 生产面板条目控制器。
    /// 负责展示单个生产蓝图名称、生产时长并响应点击。
    /// </summary>
    public sealed class ProductionItemUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text durationText;
        [SerializeField] private Button selectButton;

        #region 条目绑定
        /// <summary>
        /// 绑定条目显示内容和点击行为。
        /// </summary>
        public void Bind(string displayName, float durationSeconds, Action onClicked)
        {
            nameText.text = displayName;
            durationText.text = durationSeconds.ToString("0.#") + "s";

            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => onClicked?.Invoke());
        }
        #endregion
    }
}
