using System.Collections.Generic;
using GameDesign4.Production.Definition;
using GameDesign4.Production.Runtime;
using GameDesign4.UI.Presentation;
using GameDesign4.UI.Runtime;
using TMPro;
using UnityEngine;
using VContainer;

namespace GameDesign4.Production.Presentation
{
    /// <summary>
    /// 生产面板控制器。
    /// 负责展示当前可用生产蓝图，并在点击条目时转发排产请求。
    /// </summary>
    public sealed class ProductionPanelController : BasePanel
    {
        [SerializeField] private RectTransform itemContainer;
        [SerializeField] private GameObject pfItem;

        private readonly List<ProductionItemUI> spawnedItems = new List<ProductionItemUI>();
        private ProductionService productionService;

        /// <summary>
        /// 当前面板唯一标识。
        /// </summary>
        public override string PanelId => UIPanelId.Production;

        #region 初始化
        /// <summary>
        /// 注入生产面板所需的生产服务。
        /// </summary>
        [Inject]
        public void Construct(ProductionService productionService)
        {
            this.productionService = productionService;
        }

        /// <summary>
        /// 面板打开时刷新可用蓝图。
        /// </summary>
        protected override void OnOpened()
        {
            base.OnOpened();
            RefreshItems();
        }

        /// <summary>
        /// 刷新生产面板条目。
        /// </summary>
        private void RefreshItems()
        {
            ClearSpawnedItems();

            IReadOnlyList<ProductionBlueprintDef> blueprints = productionService.AvailableBlueprints;
            for (int index = 0; index < blueprints.Count; index++)
            {
                ProductionBlueprintDef blueprint = blueprints[index];
                ProductionItemUI itemInstance = Instantiate(pfItem, itemContainer).GetComponent<ProductionItemUI>();
                itemInstance.gameObject.SetActive(true);

                // 每个条目仅负责把点击翻译成“按蓝图追加生产任务”的请求。
                itemInstance.Bind(
                    blueprint.DisplayName,
                    blueprint.DurationSeconds,
                    () => productionService.EnqueueProduction(blueprint.Id));
                spawnedItems.Add(itemInstance);
            }
        }

        /// <summary>
        /// 清理当前已生成的面板条目实例。
        /// </summary>
        private void ClearSpawnedItems()
        {
            for (int index = spawnedItems.Count - 1; index >= 0; index--)
            {
                ProductionItemUI spawnedItem = spawnedItems[index];
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
