using System.Collections.Generic;
using GameDesign4.Infrastructure.Definitions;
using UnityEngine;

namespace GameDesign4.Production.Definition
{
    /// <summary>
    /// 生产目录定义。
    /// 负责聚合生产面板当前可展示的全部蓝图。
    /// </summary>
    [CreateAssetMenu(menuName = "GameDesign4/Production/Production Catalog")]
    public sealed class ProductionCatalogDef : ScriptableObject, IDataValidationSelfCheck
    {
        [SerializeField] private List<ProductionBlueprintDef> blueprints = new List<ProductionBlueprintDef>();

        /// <summary>
        /// 当前目录内的全部生产蓝图。
        /// </summary>
        public IReadOnlyList<ProductionBlueprintDef> Blueprints => blueprints;

        #region 目录自校验
        /// <summary>
        /// 执行生产目录自校验。
        /// </summary>
        public void ValidateSelf(List<string> issues)
        {
            HashSet<string> blueprintIdSet = new HashSet<string>();

            for (int index = 0; index < blueprints.Count; index++)
            {
                ProductionBlueprintDef blueprint = blueprints[index];
                // 目录条目不能为空。
                if (blueprint == null)
                {
                    issues.Add($"生产目录存在空蓝图条目，索引：{index}");
                    continue;
                }

                // 蓝图标识不能为空。
                if (string.IsNullOrWhiteSpace(blueprint.Id))
                {
                    issues.Add($"生产目录中的蓝图缺少 Id：{blueprint.name}");
                    continue;
                }

                // 同一个蓝图标识在同一目录中不能重复。
                if (blueprintIdSet.Add(blueprint.Id) == false)
                {
                    issues.Add($"生产目录存在重复蓝图 Id：{blueprint.Id}");
                }

                // 当前版本必须声明指定生产建筑。
                if (blueprint.ProducerBuilding == null)
                {
                    issues.Add($"生产目录中的蓝图缺少 ProducerBuilding：{blueprint.name}");
                }

                // 生产目录中的蓝图必须指向一个明确产物，否则完成后无法入库。
                if (blueprint.Product == null)
                {
                    issues.Add($"生产目录中的蓝图缺少 Product：{blueprint.name}");
                }
            }
        }
        #endregion
    }
}
