using UnityEngine;

namespace GameDesign4.Infrastructure.Runtime.Pointer
{
    /// <summary>
    /// 指针上下文结构体。
    /// 描述一次指针检测对应的屏幕坐标、场景命中与地面命中结果。
    /// </summary>
    public readonly struct PointerContext
    {
        /// <summary>
        /// 构造指针上下文。
        /// </summary>
        public PointerContext(
            Vector2 screenPosition,
            bool isOverUI,
            Transform hitTransform,
            Vector3 sceneHitPoint,
            bool hasGroundHit,
            Vector3 groundHitPoint)
        {
            ScreenPosition = screenPosition;
            IsOverUI = isOverUI;
            HitTransform = hitTransform;
            SceneHitPoint = sceneHitPoint;
            HasGroundHit = hasGroundHit;
            GroundHitPoint = groundHitPoint;
        }

        /// <summary>
        /// 当前指针屏幕坐标。
        /// </summary>
        public Vector2 ScreenPosition { get; }

        /// <summary>
        /// 当前指针是否悬停在 UI 上。
        /// </summary>
        public bool IsOverUI { get; }

        /// <summary>
        /// 当前命中的场景节点。
        /// </summary>
        public Transform HitTransform { get; }

        /// <summary>
        /// 当前命中的场景坐标。
        /// </summary>
        public Vector3 SceneHitPoint { get; }

        /// <summary>
        /// 当前是否命中地面。
        /// </summary>
        public bool HasGroundHit { get; }

        /// <summary>
        /// 当前命中的地面坐标。
        /// </summary>
        public Vector3 GroundHitPoint { get; }
    }
}
