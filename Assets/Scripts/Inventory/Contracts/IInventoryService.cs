using System;
using System.Collections.Generic;

namespace GameDesign4.Inventory.Contracts
{
    /// <summary>
    /// 仓库服务接口。
    /// 负责统一管理运行时库存数据。
    /// </summary>
    public interface IInventoryService
    {
        /// <summary>
        /// 仓库数据变更事件。
        /// </summary>
        event Action InventoryChanged;

        #region 入库与查询
        /// <summary>
        /// 把指定物品数量加入仓库。
        /// </summary>
        void AddItem(string itemId, string displayName, int amount);

        /// <summary>
        /// 获取当前仓库内物品 Id 列表。
        /// </summary>
        IReadOnlyList<string> GetItemIds();

        /// <summary>
        /// 获取指定物品显示名称。
        /// </summary>
        string GetDisplayName(string itemId);

        /// <summary>
        /// 获取指定物品数量。
        /// </summary>
        int GetAmount(string itemId);
        #endregion
    }
}
