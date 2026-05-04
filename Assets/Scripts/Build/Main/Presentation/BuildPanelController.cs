using System.Collections.Generic;
using GameDesign4.Build.Definition;
using GameDesign4.Build.Runtime;
using GameDesign4.Interaction.Contracts;
using GameDesign4.UI.Presentation;
using TMPro;
using UnityEngine;
using VContainer;

namespace GameDesign4.Build.Presentation
{
    /// <summary>
    /// 建造面板控制器。
    /// 负责展示当前可建建筑列表，并在点击条目时转发建造请求。
    /// </summary>
    public sealed class BuildPanelController : BasePanel
    {
        // 组件引用
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private RectTransform itemContainer;
        [SerializeField] private GameObject pfItem;

        private readonly List<BuildItem> spawnedItems = new List<BuildItem>();
        private BuildPlacementService buildPlacementService;
        private IInteractionModeController interactionModeController;

        /// <summary>
        /// 当前面板唯一标识。
        /// </summary>
        public override string PanelId => UI.Runtime.UIPanelId.Build;

        #region 初始化
        /// <summary>
        /// 注入建造面板所需的建造放置服务。
        /// </summary>
        [Inject]
        public void Construct(BuildPlacementService buildPlacementService, IInteractionModeController interactionModeController)
        {
            this.buildPlacementService = buildPlacementService;
            this.interactionModeController = interactionModeController;
        }
        protected override void OnOpened()
        {
            base.OnOpened();
            RefreshItems();
        }

        /// <summary>
        /// 刷新建造面板条目。
        /// </summary>
        private void RefreshItems()
        {
            ClearSpawnedItems();
            pfItem.gameObject.SetActive(false);

            IReadOnlyList<BuildingBlueprintDef> blueprints = buildPlacementService.GetAvailableBlueprints();
            for (int index = 0; index < blueprints.Count; index++)
            {
                BuildingBlueprintDef blueprint = blueprints[index];
                BuildItem itemInstance = Instantiate(pfItem, itemContainer).GetComponent<BuildItem>();
                itemInstance.gameObject.SetActive(true);

                // 每个条目仅负责把点击翻译成“开始放置指定蓝图”的请求。
                itemInstance.Bind(blueprint.DisplayName, () => interactionModeController.EnterBuildMode(blueprint.Id));
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
                BuildItem spawnedItem = spawnedItems[index];
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
