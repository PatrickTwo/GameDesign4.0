using GameDesign4.Unit.Component;
using GameDesign4.Unit.Contracts.Model;

namespace GameDesign4.Unit.Runtime
{
    /// <summary>
    /// 单位自动战斗执行器。
    /// 在没有手动命令时请求外部规则服务提供自动目标。
    /// </summary>
    public sealed class UnitAutoCombatExecutor
    {
        private readonly UnitEntity ownerUnit;

        /// <summary>
        /// 构造自动战斗执行器。
        /// </summary>
        public UnitAutoCombatExecutor(UnitEntity ownerUnit)
        {
            this.ownerUnit = ownerUnit;
        }

        #region 自动目标获取
        /// <summary>
        /// 尝试获取自动攻击目标。
        /// </summary>
        public bool TryAcquireAutoAttackTarget(out UnitId targetUnitId)
        {
            return ownerUnit.CombatRuleService.TryGetAutoAttackTarget(ownerUnit.UnitId, out targetUnitId);
        }
        #endregion
    }
}
