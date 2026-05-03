using GameDesign4.Unit.Contracts.Model;
using UnityEngine;

namespace GameDesign4.Combat.Contracts.Service
{
    /// <summary>
    /// 单位战斗规则服务接口。
    /// 由 Combat 模块实现，供 Unit 模块执行战斗编排时调用。
    /// </summary>
    public interface IUnitCombatRuleService
    {
        #region 自动索敌
        /// <summary>
        /// 为指定单位提供自动攻击目标。
        /// </summary>
        bool TryGetAutoAttackTarget(UnitId sourceUnitId, out UnitId targetUnitId);
        #endregion

        #region 目标校验
        /// <summary>
        /// 判断当前目标是否仍然有效。
        /// </summary>
        bool IsTargetValid(UnitId sourceUnitId, UnitId targetUnitId);

        /// <summary>
        /// 判断当前是否已进入攻击范围。
        /// </summary>
        bool IsTargetInAttackRange(UnitId sourceUnitId, Vector3 sourcePosition, UnitId targetUnitId, Vector3 targetPosition);
        #endregion

        #region 攻击执行
        /// <summary>
        /// 尝试执行一次攻击。
        /// 伤害、冷却与死亡判定由 Combat 模块完成。
        /// </summary>
        bool TryExecuteAttack(UnitId sourceUnitId, UnitId targetUnitId);
        #endregion
    }
}
