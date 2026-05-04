using System.Collections.Generic;
using GameDesign4.Infrastructure.Definitions;
using UnityEngine;

namespace GameDesign4.Build.Definition
{
    /// <summary>
    /// 建筑定义。
    /// 负责描述建筑基础标识、显示名、运行时预制体与占地尺寸。
    /// </summary>
    [CreateAssetMenu(menuName = "GameDesign4/Build/Building Definition")]
    public sealed class BuildingDef : EntityDef
    {
        [SerializeField] private Vector2Int footprintSize = Vector2Int.one;

        /// <summary>
        /// 建筑占地尺寸。
        /// </summary>
        public Vector2Int FootprintSize => footprintSize;

        #region 定义自校验
        /// <summary>
        /// 执行建筑定义自校验。
        /// </summary>
        public override void ValidateSelf(List<string> issues)
        {
            // 先复用实体通用字段校验，避免在建筑定义里重复维护同样的规则。
            base.ValidateSelf(issues);

            // 占地尺寸当前仍用于后续放置判定，至少需要为正数。
            if (footprintSize.x <= 0 || footprintSize.y <= 0)
            {
                issues.Add($"建筑定义占地尺寸必须大于 0：{name}");
            }
        }
        #endregion
    }
}
