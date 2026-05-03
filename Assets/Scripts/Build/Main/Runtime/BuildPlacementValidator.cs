using GameDesign4.Build.Definition;
using UnityEngine;

namespace GameDesign4.Build.Runtime
{
    /// <summary>
    /// 建造放置合法性判定器。
    /// 当前阶段暂不引入真实规则，统一返回可放置。
    /// </summary>
    public sealed class BuildPlacementValidator
    {
        #region 合法性判定
        /// <summary>
        /// 判定指定蓝图是否可以放置到目标坐标。
        /// </summary>
        public bool CanPlace(BuildingBlueprintDef blueprint, Vector3 worldPosition)
        {
            // 当前版本先打通建造链路，后续再补占地、碰撞、资源等规则。
            return true;
        }
        #endregion
    }
}
