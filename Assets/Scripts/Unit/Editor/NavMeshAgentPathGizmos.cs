#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Editor
{
    /// <summary>
    /// NavMeshAgent 寻路调试可视化绘制器。
    /// 在场景视图中绘制寻路线、拐点和终点标签。
    /// </summary>
    public static class NavMeshAgentPathGizmos
    {
        #region 常量

        /// <summary>
        /// 路径线颜色（橙色半透明）。
        /// </summary>
        private static readonly Color PathColor = new Color(1f, 0.6f, 0f, 0.8f);

        /// <summary>
        /// 拐点颜色（黄色）。
        /// </summary>
        private static readonly Color CornerColor = Color.yellow;

        /// <summary>
        /// 终点颜色（绿色）。
        /// </summary>
        private static readonly Color DestinationColor = Color.green;

        /// <summary>
        /// 拐点球体半径。
        /// </summary>
        private const float CornerSphereRadius = 0.15f;

        /// <summary>
        /// 终点球体半径。
        /// </summary>
        private const float DestinationSphereRadius = 0.25f;

        #endregion

        #region Gizmo 绘制

        /// <summary>
        /// 在场景视图中绘制寻路线、拐点和终点标签。
        /// </summary>
        /// <param name="navMeshAgent">NavMeshAgent 实例。</param>
        /// <param name="gizmoType">当前 Gizmo 绘制类型。</param>
        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected | GizmoType.Pickable)]
        private static void DrawPathGizmo(NavMeshAgent navMeshAgent, GizmoType gizmoType)
        {
            #region 参数校验
            if (navMeshAgent == null || !navMeshAgent.hasPath)
            {
                return;
            }
            #endregion

            DrawPathLine(navMeshAgent);

            DrawCornerPoints(navMeshAgent);

            DrawDestination(navMeshAgent);
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 绘制寻路路径线。
        /// </summary>
        /// <param name="navMeshAgent">NavMeshAgent 实例。</param>
        private static void DrawPathLine(NavMeshAgent navMeshAgent)
        {
            Vector3[] corners = navMeshAgent.path.corners;
            if (corners == null || corners.Length < 2)
            {
                return;
            }

            Gizmos.color = PathColor;
            for (int i = 0; i < corners.Length - 1; i++)
            {
                Gizmos.DrawLine(corners[i], corners[i + 1]);
            }
        }

        /// <summary>
        /// 绘制路径拐点。
        /// </summary>
        /// <param name="navMeshAgent">NavMeshAgent 实例。</param>
        private static void DrawCornerPoints(NavMeshAgent navMeshAgent)
        {
            Vector3[] corners = navMeshAgent.path.corners;
            if (corners == null || corners.Length < 2)
            {
                return;
            }

            Gizmos.color = CornerColor;
            for (int i = 1; i < corners.Length - 1; i++)
            {
                Gizmos.DrawWireSphere(corners[i], CornerSphereRadius);
            }
        }

        /// <summary>
        /// 绘制终点标记。
        /// </summary>
        /// <param name="navMeshAgent">NavMeshAgent 实例。</param>
        private static void DrawDestination(NavMeshAgent navMeshAgent)
        {
            Vector3 destination = navMeshAgent.destination;
            if (destination == Vector3.zero)
            {
                return;
            }

            Gizmos.color = DestinationColor;
            Gizmos.DrawWireSphere(destination, DestinationSphereRadius);

            Gizmos.color = new Color(DestinationColor.r, DestinationColor.g, DestinationColor.b, 0.5f);
            Vector3[] corners = navMeshAgent.path.corners;
            if (corners != null && corners.Length > 0)
            {
                Gizmos.DrawLine(corners[corners.Length - 1], destination);
            }
        }

        #endregion
    }
}

#endif
