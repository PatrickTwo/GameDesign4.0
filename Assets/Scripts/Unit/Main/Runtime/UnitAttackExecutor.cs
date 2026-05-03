using GameDesign4.Unit.Component;
using GameDesign4.Unit.Contracts.Model;
using GameDesign4.Unit.Presentation;
using GameDesign4.Unit.Registry;
using UnityEngine;

namespace GameDesign4.Unit.Runtime
{
    /// <summary>
    /// 单位攻击执行器。
    /// 负责“先追击，再攻击”的攻击流程编排。
    /// </summary>
    public sealed class UnitAttackExecutor
    {
        private readonly UnitEntity ownerUnit;
        private readonly UnitMovementExecutor movementExecutor;
        private readonly UnitVisualController visualController;

        /// <summary>
        /// 构造攻击执行器。
        /// </summary>
        public UnitAttackExecutor(
            UnitEntity ownerUnit,
            UnitMovementExecutor movementExecutor,
            UnitVisualController visualController)
        {
            this.ownerUnit = ownerUnit;
            this.movementExecutor = movementExecutor;
            this.visualController = visualController;
        }

        #region 攻击执行
        /// <summary>
        /// 执行一次攻击 Tick。
        /// 返回值表示当前攻击流程是否仍然有效。
        /// </summary>
        public bool TickAttack(UnitId targetUnitId, out UnitActionState nextState)
        {
            if (UnitRegistry.TryGetUnit(targetUnitId, out UnitEntity targetUnit) == false)
            {
                nextState = UnitActionState.Idle;
                return false;
            }

            if (ownerUnit.CombatRuleService.IsTargetValid(ownerUnit.UnitId, targetUnitId) == false)
            {
                nextState = UnitActionState.Idle;
                return false;
            }

            Vector3 ownerPosition = ownerUnit.Position;
            Vector3 targetPosition = targetUnit.Position;

            // 进入攻击流程后始终朝向目标，保证待机开火时表现正确。
            visualController.FaceTo(targetPosition);

            if (ownerUnit.CombatRuleService.IsTargetInAttackRange(ownerUnit.UnitId, ownerPosition, targetUnitId, targetPosition) == false)
            {
                movementExecutor.TickMove(targetPosition);
                nextState = UnitActionState.ChasingTarget;
                return true;
            }

            movementExecutor.Stop();
            ownerUnit.CombatRuleService.TryExecuteAttack(ownerUnit.UnitId, targetUnitId);
            nextState = UnitActionState.Attacking;
            return true;
        }
        #endregion
    }
}
