using UnityEngine;

namespace GameDesign4.SceneInteract.Runtime
{
    /// <summary>
    /// 指针上下文结构体。
    /// 包含当前帧指针的屏幕坐标、是否悬停在 UI 上以及场景命中结果。
    /// </summary>
    public readonly struct PointerContext
    {
        /// <summary>
        /// 当前指针屏幕坐标。
        /// </summary>
        public Vector2 ScreenPosition { get; }

        /// <summary>
        /// 当前指针是否悬停在 UI 上。
        /// </summary>
        public bool IsOverUI { get; }

        /// <summary>
        /// 场景命中信息。
        /// </summary>
        public SceneHitInfo SceneHitInfo { get; }

        /// <summary>
        /// 构造指针上下文。
        /// </summary>
        public PointerContext(Vector2 screenPosition, bool isOverUI, SceneHitInfo sceneHitInfo)
        {
            ScreenPosition = screenPosition;
            IsOverUI = isOverUI;
            SceneHitInfo = sceneHitInfo;
        }
    }
}
