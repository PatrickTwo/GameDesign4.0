using GameDesign4.Infrastructure.Definitions;

namespace GameDesign4.Inventory.Runtime
{
    /// <summary>
    /// 仓库条目运行时状态。
    /// 负责保存单种物品的定义引用与库存数量。
    /// </summary>
    public sealed class InventoryEntry
    {
        /// <summary>
        /// 构造仓库条目状态。
        /// </summary>
        public InventoryEntry(EntityDef definition, int amount)
        {
            Definition = definition;
            Amount = amount;
        }

        /// <summary>
        /// 物品定义。
        /// </summary>
        public EntityDef Definition { get; }

        /// <summary>
        /// 物品唯一标识。
        /// </summary>
        public string ItemId => Definition == null ? string.Empty : Definition.Id;

        /// <summary>
        /// 物品显示名称。
        /// </summary>
        public string DisplayName => Definition == null ? string.Empty : Definition.DisplayName;

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

        /// <summary>
        /// 扣减库存数量。
        /// </summary>
        public void RemoveAmount(int amount)
        {
            Amount -= amount;
        }
        #endregion
    }
}
