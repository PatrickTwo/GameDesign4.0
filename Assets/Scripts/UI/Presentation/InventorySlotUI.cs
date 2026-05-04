using System;
using GameDesign4.Infrastructure.Definitions;
using GameDesign4.Unit.Definition;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameDesign4.UI.Presentation
{
    /// <summary>
    /// 仓库格子控制器。
    /// 负责展示物品、响应右键并弹出部署菜单。
    /// </summary>
    public sealed class InventorySlotUI : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TMP_Text itemNameText;
        [SerializeField] private TMP_Text amountText;
        [SerializeField] private GameObject deployButtonRoot;
        [SerializeField] private Button deployButton;

        private bool canDeploy;
        private Action onDeploySelected;

        #region 初始化
        /// <summary>
        /// 绑定仓库格子显示数据和部署行为。
        /// </summary>
        public void Bind(EntityDef definition, int amount, Action onDeploySelected)
        {
            itemNameText.text = definition == null ? string.Empty : definition.DisplayName;
            amountText.text = amount.ToString();
            canDeploy = definition is UnitDef;
            this.onDeploySelected = onDeploySelected;

            deployButtonRoot.SetActive(false);
            deployButton.onClick.RemoveAllListeners();
            deployButton.onClick.AddListener(HandleDeploySelected);
        }
        #endregion

        #region 指针输入
        /// <summary>
        /// 处理仓库格子的鼠标点击。
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                deployButtonRoot.SetActive(canDeploy);
                return;
            }

            // 非右键点击时统一关闭弹出菜单，避免菜单残留。
            deployButtonRoot.SetActive(false);
        }
        #endregion

        #region 菜单行为
        /// <summary>
        /// 处理部署菜单点击。
        /// </summary>
        private void HandleDeploySelected()
        {
            deployButtonRoot.SetActive(false);
            onDeploySelected?.Invoke();
        }
        #endregion
    }
}
