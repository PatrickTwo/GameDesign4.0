using System.Collections.Generic;
using GameDesign4.Build.Contracts;
using GameDesign4.Build.Contracts.ViewData;
using TMPro;
using UnityEngine;
using VContainer;

namespace GameDesign4.Build.Presentation
{
    /// <summary>
    /// 建造面板控制器。
    /// 负责展示当前可建建筑列表，并在点击条目时转发建造请求。
    /// </summary>
    public sealed class BuildPanelController : UI.Presentation.BasePanel
    {
        // 组件引用
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private RectTransform itemContainer;
        [SerializeField] private BuildItemController itemTemplate;

        private readonly List<BuildItemController> spawnedItems = new List<BuildItemController>();
        private IBuildPlacementService buildPlacementService;

        /// <summary>
        /// 当前面板唯一标识。
        /// </summary>
        public override string PanelId => UI.Runtime.UIPanelId.Build;

        #region 初始化
        /// <summary>
        /// 注入建造面板所需的建造放置服务。
        /// </summary>
        [Inject]
        public void Construct(IBuildPlacementService buildPlacementService)
        {
            this.buildPlacementService = buildPlacementService;
            RefreshItems();
        }

        /// <summary>
        /// 刷新建造面板条目。
        /// </summary>
        private void RefreshItems()
        {
            ClearSpawnedItems();
            itemTemplate.gameObject.SetActive(false);

            IReadOnlyList<BuildPanelEntry> panelEntries = buildPlacementService.GetPanelEntries();
            for (int index = 0; index < panelEntries.Count; index++)
            {
                BuildPanelEntry panelEntry = panelEntries[index];
                BuildItemController itemInstance = Instantiate(itemTemplate, itemContainer);
                itemInstance.gameObject.SetActive(true);

                // 每个条目仅负责把点击翻译成“开始放置指定蓝图”的请求。
                itemInstance.Bind(panelEntry.DisplayName, () => buildPlacementService.StartPlacement(panelEntry.BlueprintId));
                spawnedItems.Add(itemInstance);
            }
        }

        /// <summary>
        /// 清理当前已生成的面板条目实例。
        /// </summary>
        private void ClearSpawnedItems()
        {
            for (int index = 0; index < spawnedItems.Count; index++)
            {
                BuildItemController spawnedItem = spawnedItems[index];
                if (spawnedItem != null)
                {
                    Destroy(spawnedItem.gameObject);
                }
            }

            spawnedItems.Clear();
        }
        #endregion
    }
}
