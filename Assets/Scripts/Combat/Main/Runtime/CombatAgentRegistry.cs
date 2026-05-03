using System.Collections.Generic;
using GameDesign4.Combat.Component;
using GameDesign4.Unit.Contracts.Model;

namespace GameDesign4.Combat.Runtime
{
    /// <summary>
    /// 战斗代理注册表。
    /// 用于按 UnitId 查询运行中的战斗代理。
    /// </summary>
    internal static class CombatAgentRegistry
    {
        private static readonly Dictionary<UnitId, UnitCombatAgent> AgentsByUnitId = new Dictionary<UnitId, UnitCombatAgent>();
        private static readonly List<UnitCombatAgent> AgentList = new List<UnitCombatAgent>();

        /// <summary>
        /// 当前全部运行中战斗代理。
        /// </summary>
        public static IReadOnlyList<UnitCombatAgent> Agents => AgentList;

        #region 注册管理
        /// <summary>
        /// 清空全部战斗代理。
        /// 主要用于测试收尾，避免静态注册表污染后续用例。
        /// </summary>
        public static void Clear()
        {
            AgentsByUnitId.Clear();
            AgentList.Clear();
        }

        /// <summary>
        /// 注册战斗代理。
        /// </summary>
        public static void Register(UnitCombatAgent combatAgent)
        {
            if (combatAgent == null || combatAgent.UnitId == null || combatAgent.UnitId.IsValid == false)
            {
                return;
            }

            if (AgentsByUnitId.TryGetValue(combatAgent.UnitId, out UnitCombatAgent existingAgent))
            {
                AgentList.Remove(existingAgent);
            }

            AgentsByUnitId[combatAgent.UnitId] = combatAgent;
            AgentList.Add(combatAgent);
        }

        /// <summary>
        /// 注销战斗代理。
        /// </summary>
        public static void Unregister(UnitId unitId)
        {
            if (unitId == null || unitId.IsValid == false)
            {
                return;
            }

            if (AgentsByUnitId.TryGetValue(unitId, out UnitCombatAgent existingAgent))
            {
                AgentList.Remove(existingAgent);
            }

            AgentsByUnitId.Remove(unitId);
        }
        #endregion

        #region 查询
        /// <summary>
        /// 查询指定战斗代理。
        /// </summary>
        public static bool TryGetAgent(UnitId unitId, out UnitCombatAgent combatAgent)
        {
            if (unitId == null)
            {
                combatAgent = null;
                return false;
            }

            return AgentsByUnitId.TryGetValue(unitId, out combatAgent);
        }
        #endregion
    }
}
