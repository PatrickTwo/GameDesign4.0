#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Editor
{
    /// <summary>
    /// 为 NavMeshObstacle 提供场景视图下的形状编辑能力。
    /// </summary>
    [CustomEditor(typeof(NavMeshObstacle))]
    public sealed class NavMeshObstacleEditor : UnityEditor.Editor
    {
        /// <summary>
        /// 盒体障碍物编辑手柄。
        /// </summary>
        private readonly BoxBoundsHandle boxBoundsHandle = new BoxBoundsHandle();

        #region Inspector 绘制

        /// <summary>
        /// 绘制默认 Inspector，保留 Unity 原有参数面板。
        /// </summary>
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }

        #endregion

        #region 场景交互

        /// <summary>
        /// 在场景视图中绘制并处理 NavMeshObstacle 的编辑手柄。
        /// </summary>
        private void OnSceneGUI()
        {
            NavMeshObstacle obstacle = target as NavMeshObstacle;
            if (obstacle == null)
            {
                return;
            }

            Matrix4x4 previousMatrix = Handles.matrix;
            Color previousColor = Handles.color;

            // 使用物体本地矩阵绘制手柄，保证拖拽方向与障碍物局部坐标一致。
            Handles.matrix = obstacle.transform.localToWorldMatrix;
            Handles.color = new Color(0.25f, 0.9f, 1f, 1f);

            switch (obstacle.shape)
            {
                case NavMeshObstacleShape.Box:
                    DrawBoxHandle(obstacle);
                    break;
                case NavMeshObstacleShape.Capsule:
                    DrawCapsuleHandle(obstacle);
                    break;
            }

            Handles.matrix = previousMatrix;
            Handles.color = previousColor;
        }

        /// <summary>
        /// 绘制盒体障碍物的场景编辑手柄。
        /// </summary>
        /// <param name="obstacle">目标障碍物。</param>
        private void DrawBoxHandle(NavMeshObstacle obstacle)
        {
            boxBoundsHandle.center = obstacle.center;
            boxBoundsHandle.size = obstacle.size;

            EditorGUI.BeginChangeCheck();
            boxBoundsHandle.DrawHandle();

            if (EditorGUI.EndChangeCheck() == false)
            {
                return;
            }

            Undo.RecordObject(obstacle, "调整 NavMeshObstacle 盒体");

            // 将拖拽结果回写到障碍物参数，保持 Inspector 与场景视图一致。
            obstacle.center = boxBoundsHandle.center;
            obstacle.size = ClampBoxSize(boxBoundsHandle.size);

            EditorUtility.SetDirty(obstacle);
        }

        /// <summary>
        /// 绘制胶囊障碍物的场景编辑手柄。
        /// </summary>
        /// <param name="obstacle">目标障碍物。</param>
        private void DrawCapsuleHandle(NavMeshObstacle obstacle)
        {
            Vector3 center = obstacle.center;
            float radius = Mathf.Max(0.01f, obstacle.radius);
            float height = Mathf.Max(radius * 2f, obstacle.height);

            DrawCapsuleWireframe(center, radius, height);

            EditorGUI.BeginChangeCheck();

            // 中心点位移手柄，用于整体移动障碍物形状。
            Vector3 movedCenter = Handles.PositionHandle(center, Quaternion.identity);

            float cylinderHalfHeight = Mathf.Max(0f, (height * 0.5f) - radius);
            Vector3 topHemisphereCenter = movedCenter + Vector3.up * cylinderHalfHeight;
            Vector3 bottomHemisphereCenter = movedCenter - Vector3.up * cylinderHalfHeight;

            float handleSize = HandleUtility.GetHandleSize(Handles.matrix.MultiplyPoint3x4(movedCenter)) * 0.08f;

            // 半径手柄沿局部 X 轴调整，保持胶囊体截面始终为圆。
            float adjustedRadius = Handles.ScaleValueHandle(
                radius,
                movedCenter + Vector3.right * radius,
                Quaternion.identity,
                handleSize,
                Handles.CubeHandleCap,
                0f);

            // 上下端点分别拖拽后，重新计算中心与总高度，使编辑方式更接近碰撞体体验。
            Vector3 adjustedTop = Handles.Slider(topHemisphereCenter + Vector3.up * radius, Vector3.up, handleSize, Handles.SphereHandleCap, 0f);
            Vector3 adjustedBottom = Handles.Slider(bottomHemisphereCenter - Vector3.up * radius, Vector3.down, handleSize, Handles.SphereHandleCap, 0f);

            if (EditorGUI.EndChangeCheck() == false)
            {
                return;
            }

            float finalRadius = Mathf.Max(0.01f, adjustedRadius);
            float finalTopY = adjustedTop.y;
            float finalBottomY = adjustedBottom.y;

            if (finalTopY < finalBottomY)
            {
                float tempY = finalTopY;
                finalTopY = finalBottomY;
                finalBottomY = tempY;
            }

            float finalHeight = Mathf.Max(finalRadius * 2f, finalTopY - finalBottomY);
            Vector3 finalCenter = movedCenter;
            finalCenter.y = (finalTopY + finalBottomY) * 0.5f;

            Undo.RecordObject(obstacle, "调整 NavMeshObstacle 胶囊体");

            // 将场景手柄结果写回组件，保证烘焙和运行时读取到的参数正确。
            obstacle.center = finalCenter;
            obstacle.radius = finalRadius;
            obstacle.height = finalHeight;

            EditorUtility.SetDirty(obstacle);
        }

        #endregion

        #region 形状约束

        /// <summary>
        /// 限制盒体尺寸，避免出现非正值导致编辑异常。
        /// </summary>
        /// <param name="size">原始尺寸。</param>
        /// <returns>限制后的尺寸。</returns>
        private static Vector3 ClampBoxSize(Vector3 size)
        {
            float clampedX = Mathf.Max(0.01f, size.x);
            float clampedY = Mathf.Max(0.01f, size.y);
            float clampedZ = Mathf.Max(0.01f, size.z);
            return new Vector3(clampedX, clampedY, clampedZ);
        }

        #endregion

        #region 辅助绘制

        /// <summary>
        /// 在场景中绘制胶囊体线框，帮助确认当前障碍物范围。
        /// </summary>
        /// <param name="center">障碍物中心。</param>
        /// <param name="radius">胶囊半径。</param>
        /// <param name="height">胶囊高度。</param>
        private static void DrawCapsuleWireframe(Vector3 center, float radius, float height)
        {
            float cylinderHalfHeight = Mathf.Max(0f, (height * 0.5f) - radius);
            Vector3 topHemisphereCenter = center + Vector3.up * cylinderHalfHeight;
            Vector3 bottomHemisphereCenter = center - Vector3.up * cylinderHalfHeight;

            // 上下圆环用于表达胶囊体的主体边界。
            Handles.DrawWireDisc(topHemisphereCenter, Vector3.up, radius);
            Handles.DrawWireDisc(bottomHemisphereCenter, Vector3.up, radius);

            Vector3 rightOffset = Vector3.right * radius;
            Vector3 leftOffset = Vector3.left * radius;
            Vector3 forwardOffset = Vector3.forward * radius;
            Vector3 backOffset = Vector3.back * radius;

            // 四条竖线用于连接上下圆环，直观表现胶囊中段高度。
            Handles.DrawLine(topHemisphereCenter + rightOffset, bottomHemisphereCenter + rightOffset);
            Handles.DrawLine(topHemisphereCenter + leftOffset, bottomHemisphereCenter + leftOffset);
            Handles.DrawLine(topHemisphereCenter + forwardOffset, bottomHemisphereCenter + forwardOffset);
            Handles.DrawLine(topHemisphereCenter + backOffset, bottomHemisphereCenter + backOffset);

            Handles.DrawWireArc(topHemisphereCenter, Vector3.forward, Vector3.right, 180f, radius);
            Handles.DrawWireArc(topHemisphereCenter, Vector3.right, Vector3.back, 180f, radius);
            Handles.DrawWireArc(bottomHemisphereCenter, Vector3.forward, Vector3.left, 180f, radius);
            Handles.DrawWireArc(bottomHemisphereCenter, Vector3.right, Vector3.forward, 180f, radius);
        }

        #endregion
    }
}

#endif
