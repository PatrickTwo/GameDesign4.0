using System.Collections.Generic;
using GameDesign4.Infrastructure.Definitions;
using UnityEngine;

namespace GameDesign4.Build.Definition
{
    /// <summary>
    /// 建筑蓝图定义。
    /// 负责声明建造面板入口信息，并指向最终建筑定义。
    /// </summary>
    [CreateAssetMenu(menuName = "GameDesign4/Build/Building Blueprint")]
    public sealed class BuildingBlueprintDef : ScriptableObject, IDataValidationSelfCheck
    {
        [SerializeField] private string id = string.Empty;
        [SerializeField] private string displayName = string.Empty;
        [SerializeField] private Sprite icon;
        [SerializeField] private BuildingDef building;

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
        /// 蓝图对应的建筑定义。
        /// </summary>
        public BuildingDef Building => building;

        #region 定义自校验
        /// <summary>
        /// 执行建筑蓝图自校验。
        /// </summary>
        public void ValidateSelf(List<string> issues)
        {
            // 蓝图标识不能为空。
            if (string.IsNullOrWhiteSpace(id))
            {
                issues.Add("建筑蓝图缺少 Id。");
            }

            // 蓝图显示名不能为空，避免面板出现空文案。
            if (string.IsNullOrWhiteSpace(displayName))
            {
                issues.Add($"建筑蓝图缺少 DisplayName：{name}");
            }

            // 蓝图必须指向一个最终建筑定义。
            if (building == null)
            {
                issues.Add($"建筑蓝图缺少 Building 引用：{name}");
            }
        }
        #endregion
    }
}
