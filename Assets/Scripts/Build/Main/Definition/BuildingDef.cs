using System.Collections.Generic;
using GameDesign4.Infrastructure.Definitions;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GameDesign4.Build.Definition
{
    /// <summary>
    /// 建筑定义。
    /// 负责描述建筑基础标识、显示名、运行时预制体与占地尺寸。
    /// </summary>
    [CreateAssetMenu(menuName = "GameDesign4/Build/Building Definition")]
    public sealed class BuildingDef : ScriptableObject, IDataValidationSelfCheck
    {
        [SerializeField] private string id = string.Empty;
        [SerializeField] private string displayName = string.Empty;
        [SerializeField] private AssetReferenceGameObject prefabReference;
        [SerializeField] private Vector2Int footprintSize = Vector2Int.one;

        /// <summary>
        /// 建筑唯一标识。
        /// </summary>
        public string Id => id;

        /// <summary>
        /// 建筑显示名称。
        /// </summary>
        public string DisplayName => displayName;

        /// <summary>
        /// 建筑运行时预制体引用。
        /// </summary>
        public AssetReferenceGameObject PrefabReference => prefabReference;

        /// <summary>
        /// 建筑占地尺寸。
        /// </summary>
        public Vector2Int FootprintSize => footprintSize;

        #region 定义自校验
        /// <summary>
        /// 执行建筑定义自校验。
        /// </summary>
        public void ValidateSelf(List<string> issues)
        {
            // 建筑标识不能为空。
            if (string.IsNullOrWhiteSpace(id))
            {
                issues.Add("建筑定义缺少 Id。");
            }

            // 建筑显示名不能为空，避免面板出现空文案。
            if (string.IsNullOrWhiteSpace(displayName))
            {
                issues.Add($"建筑定义缺少 DisplayName：{name}");
            }

            // 建筑运行时预制体必须存在。
            if (prefabReference == null || prefabReference.RuntimeKeyIsValid() == false)
            {
                issues.Add($"建筑定义缺少有效 Addressable 预制体：{name}");
            }

            // 占地尺寸当前仍用于后续放置判定，至少需要为正数。
            if (footprintSize.x <= 0 || footprintSize.y <= 0)
            {
                issues.Add($"建筑定义占地尺寸必须大于 0：{name}");
            }
        }
        #endregion
    }
}
