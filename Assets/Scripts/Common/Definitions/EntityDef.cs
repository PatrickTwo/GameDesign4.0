using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GameDesign4.Infrastructure.Definitions
{
    /// <summary>
    /// 实体定义基类。
    /// 负责承载场景实体共用的标识、显示信息与运行时预制体引用。
    /// </summary>
    public abstract class EntityDef : ScriptableObject, IDataValidationSelfCheck
    {
        [SerializeField] private string id = string.Empty;
        [SerializeField] private string displayName = string.Empty;
        [SerializeField] private Sprite icon;
        [SerializeField] private AssetReferenceGameObject prefabReference;

        /// <summary>
        /// 实体唯一标识。
        /// </summary>
        public string Id => id;

        /// <summary>
        /// 实体显示名称。
        /// </summary>
        public string DisplayName => displayName;

        /// <summary>
        /// 实体图标。
        /// </summary>
        public Sprite Icon => icon;

        /// <summary>
        /// 实体运行时预制体引用。
        /// </summary>
        public AssetReferenceGameObject PrefabReference => prefabReference;

        #region 定义自校验
        /// <summary>
        /// 执行实体定义自校验。
        /// </summary>
        public virtual void ValidateSelf(List<string> issues)
        {
            // 实体标识不能为空。
            if (string.IsNullOrWhiteSpace(id))
            {
                issues.Add("实体定义缺少 Id。");
            }

            // 实体显示名不能为空，避免面板出现空文案。
            if (string.IsNullOrWhiteSpace(displayName))
            {
                issues.Add($"实体定义缺少 DisplayName：{name}");
            }

            // 实体运行时预制体必须存在。
            if (prefabReference == null || prefabReference.RuntimeKeyIsValid() == false)
            {
                issues.Add($"实体定义缺少有效 Addressable 预制体：{name}");
            }
        }
        #endregion
    }
}
