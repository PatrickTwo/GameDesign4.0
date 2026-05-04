using GameDesign4.Combat.Component;
using UnityEditor;
using UnityEngine;

namespace GameDesign4.Combat.Editor
{
    /// <summary>
    /// 单位战斗代理 Gizmo 绘制器。
    /// 负责在 Scene 视图中常驻显示攻击范围与自动索敌范围。
    /// </summary>
    public static class UnitCombatAgentGizmoDrawer
    {
        private static readonly Color AttackRangeColor = new Color(1f, 0.25f, 0.25f, 0.9f);
        private static readonly Color AutoSearchRangeColor = new Color(1f, 0.85f, 0.2f, 0.9f);

        #region Gizmo绘制
        /// <summary>
        /// 为单位战斗代理绘制范围 Gizmo。
        /// </summary>
        [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
        private static void DrawUnitCombatAgentGizmos(UnitCombatAgent agent, GizmoType gizmoType)
        {
            if (agent == null)
            {
                return;
            }

            // 以单位当前位置为圆心，在水平面上绘制攻击范围。
            Handles.color = AttackRangeColor;
            Handles.DrawWireDisc(agent.transform.position, Vector3.up, agent.AttackRange);

            // 以单位当前位置为圆心，在水平面上绘制自动索敌范围。
            Handles.color = AutoSearchRangeColor;
            Handles.DrawWireDisc(agent.transform.position, Vector3.up, agent.AutoSearchRange);
        }
        #endregion
    }
}
