using System;
using System.Collections.Generic;
using GameDesign4.Infrastructure.Utilities;
using GameDesign4.Inventory.Contracts;

namespace GameDesign4.Inventory.Runtime
{
    /// <summary>
    /// 仓库服务。
    /// 负责维护运行时库存条目，并向 UI 提供查询能力。
    /// </summary>
    public sealed class InventoryService : IInventoryService
    {
        private readonly List<string> itemOrder = new List<string>();
        private readonly Dictionary<string, InventoryEntry> entryLookup = new Dictionary<string, InventoryEntry>();

        /// <summary>
        /// 仓库数据变更事件。
        /// </summary>
        public event Action InventoryChanged;

        #region 入库与查询
        /// <summary>
        /// 把指定物品数量加入仓库。
        /// </summary>
        public void AddItem(string itemId, string displayName, int amount)
        {
            Guard.EnsureNotNullOrWhiteSpace(itemId, nameof(itemId));
            Guard.EnsureNotNullOrWhiteSpace(displayName, nameof(displayName));
            Guard.Ensure(amount > 0, "入库数量必须大于 0。");

            if (entryLookup.TryGetValue(itemId, out InventoryEntry entry) == false)
            {
                entry = new InventoryEntry(itemId, displayName, 0);
                entryLookup.Add(itemId, entry);
                itemOrder.Add(itemId);
            }

            // 同一种物品统一累加数量。
            entry.AddAmount(amount);
            InventoryChanged?.Invoke();
        }

        /// <summary>
        /// 获取当前仓库内物品 Id 列表。
        /// </summary>
        public IReadOnlyList<string> GetItemIds()
        {
            return itemOrder;
        }

        /// <summary>
        /// 获取指定物品显示名称。
        /// </summary>
        public string GetDisplayName(string itemId)
        {
            Guard.EnsureDictionaryContainsKey(entryLookup, itemId, "仓库中不存在指定物品。");
            return entryLookup[itemId].DisplayName;
        }

        /// <summary>
        /// 获取指定物品数量。
        /// </summary>
        public int GetAmount(string itemId)
        {
            Guard.EnsureDictionaryContainsKey(entryLookup, itemId, "仓库中不存在指定物品。");
            return entryLookup[itemId].Amount;
        }
        #endregion
    }
}
