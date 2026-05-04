using System.Collections.Generic;
using GameDesign4.Infrastructure.Definitions;
using GameDesign4.Inventory.Contracts;
using GameDesign4.Interaction.Contracts;
using UnityEngine;
using VContainer;
using GameDesign4.UI.Runtime;

namespace GameDesign4.UI.Presentation
{
    /// <summary>
    /// 仓库面板控制器。
    /// 负责展示当前已入库的物品列表。
    /// </summary>
    public sealed class InventoryPanelController : BasePanel
    {
        [SerializeField] private RectTransform slotContainer;
        [SerializeField] private GameObject pfSlot;

        private readonly List<InventorySlotUI> spawnedSlots = new List<InventorySlotUI>();
        private IInteractionModeController interactionModeController;
        private IUIController uiController;
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
        public void Construct(
            IInventoryService inventoryService,
            IInteractionModeController interactionModeController,
            IUIController uiController)
        {
            this.inventoryService = inventoryService;
            this.interactionModeController = interactionModeController;
            this.uiController = uiController;
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
                InventorySlotUI slotInstance = Instantiate(pfSlot, slotContainer).GetComponent<InventorySlotUI>();
                slotInstance.gameObject.SetActive(true);

                EntityDef itemDefinition = inventoryService.GetDefinition(itemId);
                int amount = inventoryService.GetAmount(itemId);

                // 只有 UnitDef 类型条目会在格子内部弹出“部署”菜单。
                slotInstance.Bind(itemDefinition, amount, () => HandleDeployRequested(itemDefinition));
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
                InventorySlotUI spawnedSlot = spawnedSlots[index];
                if (spawnedSlot != null)
                {
                    Destroy(spawnedSlot.gameObject);
                }
            }

            spawnedSlots.Clear();
        }

        /// <summary>
        /// 处理从仓库面板发起的部署请求。
        /// </summary>
        private void HandleDeployRequested(EntityDef itemDefinition)
        {
            // 先进入部署模式，再关闭仓库面板，保证面板状态与交互状态同步切换。
            interactionModeController.EnterDeploymentMode(itemDefinition);
            uiController.ClosePanel(PanelId);
        }
        #endregion
    }
}
