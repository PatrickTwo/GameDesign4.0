using System.Collections.Generic;
using GameDesign4.Infrastructure.Definitions;
using UnityEngine;

namespace GameDesign4.Unit.Definition
{
    /// <summary>
    /// 单位定义。
    /// 负责承载单位实体的最小配置骨架，后续单位专属字段在此基础上继续追加。
    /// </summary>
    [CreateAssetMenu(menuName = "GameDesign4/Unit/Unit Definition")]
    public sealed class UnitDef : EntityDef
    {
        #region 定义自校验
        /// <summary>
        /// 执行单位定义自校验。
        /// </summary>
        public override void ValidateSelf(List<string> issues)
        {
            // 当前单位定义只需要校验实体通用字段。
            base.ValidateSelf(issues);
        }
        #endregion
    }
}
