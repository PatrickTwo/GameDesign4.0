using UnityEngine;

namespace GameDesign4.Build.Contracts.ViewData
{
    /// <summary>
    /// 建造面板条目视图数据。
    /// 负责向展示层暴露蓝图标识、显示名与图标。
    /// </summary>
    public readonly struct BuildPanelEntry
    {
        /// <summary>
        /// 构造建造面板条目视图数据。
        /// </summary>
        public BuildPanelEntry(string blueprintId, string displayName, Sprite icon)
        {
            BlueprintId = blueprintId;
            DisplayName = displayName;
            Icon = icon;
        }

        /// <summary>
        /// 蓝图唯一标识。
        /// </summary>
        public string BlueprintId { get; }

        /// <summary>
        /// 蓝图显示名称。
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// 蓝图图标。
        /// </summary>
        public Sprite Icon { get; }
    }
}
