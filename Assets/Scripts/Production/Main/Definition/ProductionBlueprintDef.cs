using System.Collections.Generic;
using GameDesign4.Build.Definition;
using GameDesign4.Infrastructure.Definitions;
using UnityEngine;

namespace GameDesign4.Production.Definition
{
    /// <summary>
    /// 生产蓝图定义。
    /// 负责声明单个可生产产物的展示信息、生产时长与指定生产建筑。
    /// </summary>
    [CreateAssetMenu(menuName = "GameDesign4/Production/Production Blueprint")]
    public sealed class ProductionBlueprintDef : ScriptableObject, IDataValidationSelfCheck
    {
        [SerializeField] private string id = string.Empty;
        [SerializeField] private string displayName = string.Empty;
        [SerializeField] private Sprite icon;
        [SerializeField] private float durationSeconds = 3f;
        [SerializeField] private BuildingDef producerBuilding;
        [SerializeField] private EntityDef product;

        /// <summary>
        /// 蓝图唯一标识。
        /// </summary>
        public string Id => id;

        /// <summary>
        /// 蓝图显示名称。
        /// </summary>
        public string DisplayName => displayName;

        /// <summary>
        /// 蓝图图标。
        /// </summary>
        public Sprite Icon => icon;

        /// <summary>
        /// 生产时长，单位秒。
        /// </summary>
        public float DurationSeconds => durationSeconds;

        /// <summary>
        /// 指定生产建筑定义。
        /// </summary>
        public BuildingDef ProducerBuilding => producerBuilding;

        /// <summary>
        /// 蓝图对应的产物定义。
        /// </summary>
        public EntityDef Product => product;

        #region 定义自校验
        /// <summary>
        /// 执行生产蓝图自校验。
        /// </summary>
        public void ValidateSelf(List<string> issues)
        {
            // 蓝图标识不能为空。
            if (string.IsNullOrWhiteSpace(id))
            {
                issues.Add("生产蓝图缺少 Id。");
            }

            // 蓝图显示名不能为空，避免面板出现空文案。
            if (string.IsNullOrWhiteSpace(displayName))
            {
                issues.Add($"生产蓝图缺少 DisplayName：{name}");
            }

            // 生产时长必须大于 0，否则任务会立即完成，失去排产意义。
            if (durationSeconds <= 0f)
            {
                issues.Add($"生产蓝图时长必须大于 0：{name}");
            }

            // 当前版本必须显式绑定生产建筑，避免生产能力来源不清晰。
            if (producerBuilding == null)
            {
                issues.Add($"生产蓝图缺少 ProducerBuilding：{name}");
            }

            // 当前阶段约定产物都是实体定义，因此蓝图必须指向一个有效实体。
            if (product == null)
            {
                issues.Add($"生产蓝图缺少 Product：{name}");
            }
        }
        #endregion
    }
}
