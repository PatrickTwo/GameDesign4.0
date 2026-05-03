using System.Collections.Generic;
using GameDesign4.Shared.Definitions;
using UnityEngine;

namespace GameDesign4.UI.Definitions
{
    /// <summary>
    /// UI 面板目录定义。
    /// 负责集中声明运行时可加载面板的元数据。
    /// </summary>
    [CreateAssetMenu(menuName = "GameDesign4/UI/UI Panel Catalog")]
    public sealed class UiPanelCatalogDef : ScriptableObject, IDataValidationSelfCheck
    {
        [SerializeField] private List<UiPanelEntryDef> panelEntries = new List<UiPanelEntryDef>();

        /// <summary>
        /// 当前目录内的全部面板条目。
        /// </summary>
        public IReadOnlyList<UiPanelEntryDef> PanelEntries => panelEntries;

        #region UI目录自校验
        /// <summary>
        /// 执行 UI 目录自校验。
        /// </summary>
        public void ValidateSelf(List<string> issues)
        {
            HashSet<string> panelIdSet = new HashSet<string>();

            for (int index = 0; index < panelEntries.Count; index++)
            {
                UiPanelEntryDef entry = panelEntries[index];
                // 目录条目不能为空。
                if (entry == null)
                {
                    issues.Add($"UI 面板目录存在空条目，索引：{index}");
                    continue;
                }

                // 面板标识必须存在。
                if (string.IsNullOrWhiteSpace(entry.PanelId))
                {
                    issues.Add($"UI 面板条目缺少 PanelId，索引：{index}");
                    continue;
                }

                // 面板地址键必须存在。
                if (string.IsNullOrWhiteSpace(entry.AddressKey))
                {
                    issues.Add($"UI 面板缺少 Addressable 地址键：{entry.PanelId}");
                }

                // 同一个 PanelId 不应重复出现。
                if (panelIdSet.Add(entry.PanelId) == false)
                {
                    issues.Add($"UI 面板目录存在重复 PanelId：{entry.PanelId}");
                }
            }

            for (int index = 0; index < panelEntries.Count; index++)
            {
                UiPanelEntryDef entry = panelEntries[index];
                if (entry == null || string.IsNullOrWhiteSpace(entry.PanelId))
                {
                    continue;
                }

                IReadOnlyList<string> exclusivePanelIds = entry.ExclusivePanelIds;
                for (int exclusiveIndex = 0; exclusiveIndex < exclusivePanelIds.Count; exclusiveIndex++)
                {
                    string exclusivePanelId = exclusivePanelIds[exclusiveIndex];
                    // 互斥列表中不应出现空值。
                    if (string.IsNullOrWhiteSpace(exclusivePanelId))
                    {
                        issues.Add($"UI 面板互斥配置存在空值：{entry.PanelId}");
                        continue;
                    }

                    // 面板不能把自己配置成互斥对象。
                    if (exclusivePanelId == entry.PanelId)
                    {
                        issues.Add($"UI 面板不能将自己配置为互斥对象：{entry.PanelId}");
                    }

                    // 互斥目标必须真实存在于当前目录中。
                    if (panelIdSet.Contains(exclusivePanelId) == false)
                    {
                        issues.Add($"UI 面板互斥目标不存在：{entry.PanelId} -> {exclusivePanelId}");
                    }
                }
            }
        }
        #endregion
    }
}
