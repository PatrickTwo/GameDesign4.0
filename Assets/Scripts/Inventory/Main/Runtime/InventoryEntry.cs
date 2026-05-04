namespace GameDesign4.Inventory.Runtime
{
    /// <summary>
    /// 仓库条目运行时状态。
    /// 负责保存单种物品的显示名与库存数量。
    /// </summary>
    public sealed class InventoryEntry
    {
        /// <summary>
        /// 构造仓库条目状态。
        /// </summary>
        public InventoryEntry(string itemId, string displayName, int amount)
        {
            ItemId = itemId;
            DisplayName = displayName;
            Amount = amount;
        }

        /// <summary>
        /// 物品唯一标识。
        /// </summary>
        public string ItemId { get; }

        /// <summary>
        /// 物品显示名称。
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// 当前库存数量。
        /// </summary>
        public int Amount { get; private set; }

        #region 数量变更
        /// <summary>
        /// 增加库存数量。
        /// </summary>
        public void AddAmount(int amount)
        {
            Amount += amount;
        }
        #endregion
    }
}
