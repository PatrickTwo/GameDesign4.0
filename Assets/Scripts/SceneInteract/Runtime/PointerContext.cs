using UnityEngine;

namespace GameDesign4.SceneInteract.Runtime
{
    /// <summary>
    /// 指针上下文结构体。
    /// 描述一次场景交互输入对应的屏幕位置与命中结果。
    /// </summary>
    public readonly struct PointerContext
    {
        /// <summary>
        /// 构造指针上下文。
        /// </summary>
        public PointerContext(
            Vector2 screenPosition,
            bool isOverUI,
            SceneSelectable hitSelectable,
            Vector3 sceneHitPoint,
            bool hasGroundHit,
            Vector3 groundHitPoint)
        {
            ScreenPosition = screenPosition;
            IsOverUI = isOverUI;
            HitSelectable = hitSelectable;
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
        /// 当前命中的可选择对象。
        /// </summary>
        public SceneSelectable HitSelectable { get; }

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
