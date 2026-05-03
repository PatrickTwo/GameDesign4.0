using System.Collections.Generic;
using GameDesign4.Infrastructure.Definitions;
using UnityEngine;

namespace GameDesign4.Build.Definition
{
    /// <summary>
    /// 建造目录定义。
    /// 负责聚合当前场景可用的建筑蓝图清单。
    /// </summary>
    [CreateAssetMenu(menuName = "GameDesign4/Build/Build Catalog")]
    public sealed class BuildCatalogDef : ScriptableObject, IDataValidationSelfCheck
    {
        [SerializeField] private List<BuildingBlueprintDef> blueprints = new List<BuildingBlueprintDef>();

        /// <summary>
        /// 当前目录内的全部建筑蓝图。
        /// </summary>
        public IReadOnlyList<BuildingBlueprintDef> Blueprints => blueprints;

        #region 目录自校验
        /// <summary>
        /// 执行建造目录自校验。
        /// </summary>
        public void ValidateSelf(List<string> issues)
        {
            HashSet<string> blueprintIdSet = new HashSet<string>();

            for (int index = 0; index < blueprints.Count; index++)
            {
                BuildingBlueprintDef blueprint = blueprints[index];
                // 目录条目不能为空。
                if (blueprint == null)
                {
                    issues.Add($"建造目录存在空蓝图条目，索引：{index}");
                    continue;
                }

                // 蓝图标识不能为空。
                if (string.IsNullOrWhiteSpace(blueprint.Id))
                {
                    issues.Add($"建造目录中的蓝图缺少 Id：{blueprint.name}");
                    continue;
                }

                // 建造目录中同一个蓝图标识不能重复。
                if (blueprintIdSet.Add(blueprint.Id) == false)
                {
                    issues.Add($"建造目录存在重复蓝图 Id：{blueprint.Id}");
                }

                // 蓝图必须指向有效建筑定义。
                if (blueprint.Building == null)
                {
                    issues.Add($"建造目录中的蓝图缺少建筑定义引用：{blueprint.name}");
                }
            }
        }
        #endregion
    }
}
