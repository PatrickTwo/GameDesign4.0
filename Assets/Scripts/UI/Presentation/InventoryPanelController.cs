using System.Collections.Generic;
using GameDesign4.Inventory.Contracts;
using TMPro;
using UnityEngine;
using VContainer;

namespace GameDesign4.UI.Presentation
{
    /// <summary>
    /// 仓库面板控制器。
    /// 负责展示当前已入库的物品列表。
    /// </summary>
    public sealed class InventoryPanelController : BasePanel
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private RectTransform slotContainer;
        [SerializeField] private GameObject pfSlot;

        private readonly List<GameObject> spawnedSlots = new List<GameObject>();
        private IInventoryService inventoryService;

        /// <summary>
        /// 当前面板唯一标识。
        /// </summary>
        public override string PanelId => Runtime.UIPanelId.Inventory;

        #region 初始化
        /// <summary>
        /// 注入仓库面板所需的仓库服务。
        /// </summary>
        [Inject]
        public void Construct(IInventoryService inventoryService)
        {
            this.inventoryService = inventoryService;
            this.inventoryService.InventoryChanged += HandleInventoryChanged;
        }

        /// <summary>
        /// 面板打开时刷新仓库条目。
        /// </summary>
        protected override void OnOpened()
        {
            base.OnOpened();
            RefreshSlots();
        }

        /// <summary>
        /// 销毁时解除事件绑定。
        /// </summary>
        private void OnDestroy()
        {
            if (inventoryService != null)
            {
                inventoryService.InventoryChanged -= HandleInventoryChanged;
            }
        }
        #endregion

        #region 数据刷新
        /// <summary>
        /// 处理仓库数据变更。
        /// </summary>
        private void HandleInventoryChanged()
        {
            if (IsOpen)
            {
                RefreshSlots();
            }
        }

        /// <summary>
        /// 刷新仓库格子显示。
        /// </summary>
        private void RefreshSlots()
        {
            ClearSpawnedSlots();
            pfSlot.gameObject.SetActive(false);

            IReadOnlyList<string> itemIds = inventoryService.GetItemIds();
            for (int index = 0; index < itemIds.Count; index++)
            {
                string itemId = itemIds[index];
                GameObject slotInstance = Instantiate(pfSlot, slotContainer);
                slotInstance.gameObject.SetActive(true);

                TMP_Text itemNameText;
                TMP_Text amountText;
                ResolveSlotTexts(slotInstance, out itemNameText, out amountText);

                // 当前版本仓库面板只展示名称与数量，不引入额外槽位行为。
                itemNameText.text = inventoryService.GetDisplayName(itemId);
                amountText.text = inventoryService.GetAmount(itemId).ToString();
                spawnedSlots.Add(slotInstance);
            }
        }

        /// <summary>
        /// 清理当前已生成的仓库格子实例。
        /// </summary>
        private void ClearSpawnedSlots()
        {
            for (int index = 0; index < spawnedSlots.Count; index++)
            {
                GameObject spawnedSlot = spawnedSlots[index];
                if (spawnedSlot != null)
                {
                    Destroy(spawnedSlot.gameObject);
                }
            }

            spawnedSlots.Clear();
        }

        /// <summary>
        /// 解析仓库格子内的名称文本与数量文本。
        /// </summary>
        private static void ResolveSlotTexts(GameObject slotInstance, out TMP_Text itemNameText, out TMP_Text amountText)
        {
            TMP_Text[] texts = slotInstance.GetComponentsInChildren<TMP_Text>(true);
            itemNameText = null;
            amountText = null;

            for (int index = 0; index < texts.Length; index++)
            {
                TMP_Text text = texts[index];
                if (text.name == "itemNameText")
                {
                    itemNameText = text;
                    continue;
                }

                if (text.name == "amountText")
                {
                    amountText = text;
                }
            }
        }
        #endregion
    }
}
