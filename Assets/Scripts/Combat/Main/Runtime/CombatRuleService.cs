using System.Collections.Generic;
using GameDesign4.Combat.Component;
using GameDesign4.Combat.Contracts.Service;
using GameDesign4.Unit.Contracts.Model;
using UnityEngine;

namespace GameDesign4.Combat.Runtime
{
    /// <summary>
    /// 单位战斗规则服务。
    /// 提供普通攻击所需的自动索敌、目标校验、距离判断与伤害结算。
    /// </summary>
    public sealed class CombatRuleService : IUnitCombatRuleService
    {
        #region 自动索敌
        /// <summary>
        /// 为指定单位提供最近的敌方目标。
        /// <para>算法：线性遍历全量 Agent，过滤存活+异阵营，在搜索半径内取距离最近者（O(n)）</para>
        /// </summary>
        public bool TryGetAutoAttackTarget(UnitId sourceUnitId, out UnitId targetUnitId)
        {
            targetUnitId = null;
            // 检查源单位是否存在且存活
            if (CombatAgentRegistry.TryGetAgent(sourceUnitId, out UnitCombatAgent sourceAgent) == false || sourceAgent.IsAlive == false)
            {
                return false;
            }
            // 遍历所有 Agent，筛选存活且阵营不同的有效目标，取搜索范围内距离最近者
            float bestDistanceSqr = float.MaxValue;
            IReadOnlyList<UnitCombatAgent> combatAgents = CombatAgentRegistry.Agents;

            for (int index = 0; index < combatAgents.Count; index++)
            {
                UnitCombatAgent candidateAgent = combatAgents[index];
                if (IsTargetValid(sourceAgent, candidateAgent) == false)
                {
                    continue;
                }

                Vector3 offset = candidateAgent.transform.position - sourceAgent.transform.position;
                float distanceSqr = offset.sqrMagnitude;
                float searchRangeSqr = sourceAgent.AutoSearchRange * sourceAgent.AutoSearchRange;
                if (distanceSqr > searchRangeSqr || distanceSqr >= bestDistanceSqr)
                {
                    continue;
                }

                bestDistanceSqr = distanceSqr;
                targetUnitId = candidateAgent.UnitId;
            }

            return targetUnitId != null;
        }
        #endregion

        #region 目标校验
        /// <summary>
        /// 判断当前目标是否仍然有效。
        /// <para>校验规则：源/目标均存活、非自身、阵营不同 → 有效</para>
        /// </summary>
        public bool IsTargetValid(UnitId sourceUnitId, UnitId targetUnitId)
        {
            if (CombatAgentRegistry.TryGetAgent(sourceUnitId, out UnitCombatAgent sourceAgent) == false)
            {
                return false;
            }

            if (CombatAgentRegistry.TryGetAgent(targetUnitId, out UnitCombatAgent targetAgent) == false)
            {
                return false;
            }

            return IsTargetValid(sourceAgent, targetAgent);
        }

        /// <summary>
        /// 判断当前是否已进入攻击范围。
        /// <para>算法：目标有效 + 距离 ≤ 攻击范围 → 可攻击</para>
        /// </summary>
        public bool IsTargetInAttackRange(UnitId sourceUnitId, Vector3 sourcePosition, UnitId targetUnitId, Vector3 targetPosition)
        {
            if (CombatAgentRegistry.TryGetAgent(sourceUnitId, out UnitCombatAgent sourceAgent) == false)
            {
                return false;
            }

            if (CombatAgentRegistry.TryGetAgent(targetUnitId, out UnitCombatAgent targetAgent) == false)
            {
                return false;
            }

            if (IsTargetValid(sourceAgent, targetAgent) == false)
            {
                return false;
            }

            float distanceSqr = (targetPosition - sourcePosition).sqrMagnitude;
            float attackRangeSqr = sourceAgent.AttackRange * sourceAgent.AttackRange;
            return distanceSqr <= attackRangeSqr;
        }
        #endregion

        #region 攻击执行
        /// <summary>
        /// 尝试执行一次攻击。
        /// <para>算法：目标有效 + 攻击冷却就绪 → 造成伤害并重置冷却</para>
        /// </summary>
        public bool TryExecuteAttack(UnitId sourceUnitId, UnitId targetUnitId)
        {
            if (CombatAgentRegistry.TryGetAgent(sourceUnitId, out UnitCombatAgent sourceAgent) == false)
            {
                return false;
            }

            if (CombatAgentRegistry.TryGetAgent(targetUnitId, out UnitCombatAgent targetAgent) == false)
            {
                return false;
            }

            if (IsTargetValid(sourceAgent, targetAgent) == false)
            {
                return false;
            }

            float currentTime = Time.time;
            if (sourceAgent.CanAttack(currentTime) == false)
            {
                return false;
            }

            targetAgent.ReceiveDamage(sourceAgent.AttackDamage);
            sourceAgent.MarkAttack(currentTime);
            return true;
        }
        #endregion

        #region 规则辅助
        /// <summary>
        /// 判断目标对源单位是否有效。
        /// </summary>
        private bool IsTargetValid(UnitCombatAgent sourceAgent, UnitCombatAgent targetAgent)
        {
            if (sourceAgent == null || targetAgent == null)
            {
                return false;
            }

            if (sourceAgent.UnitId == null || targetAgent.UnitId == null)
            {
                return false;
            }

            if (sourceAgent.UnitId == targetAgent.UnitId)
            {
                return false;
            }

            if (sourceAgent.IsAlive == false || targetAgent.IsAlive == false)
            {
                return false;
            }

            return sourceAgent.CampId != targetAgent.CampId;
        }
        #endregion
    }
}
