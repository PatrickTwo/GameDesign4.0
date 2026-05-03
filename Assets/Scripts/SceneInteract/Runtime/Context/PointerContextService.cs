using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace GameDesign4.SceneInteract.Runtime
{
    /// <summary>
    /// 指针上下文服务。
    /// 根据屏幕坐标执行场景射线查询，生成包含命中结果的指针上下文。
    /// </summary>
    public sealed class PointerContextService
    {
        #region 射线查询配置

        /// <summary>
        /// 地面检测层掩码。
        /// </summary>
        private readonly int groundLayerMask;

        /// <summary>
        /// 场景物体检测层掩码。
        /// 默认包含所有可射线检测层，并排除地面层，避免命中被地面吞掉。
        /// </summary>
        private readonly int sceneLayerMask;

        /// <summary>
        /// 指针射线的最大检测距离。
        /// </summary>
        private readonly float maxRayDistance = 1000f;
        /// <summary>
        /// 获取当前指针上下文。
        /// </summary>
        public PointerContext CurrentPointerContext => GetPointerContext();

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化指针上下文服务。
        /// </summary>
        public PointerContextService()
        {
            // 地面层单独用于建筑放置、单位寻路目标等地表命中判定。
            // HACK 配置方式后续可能需要调整
            groundLayerMask = LayerMask.GetMask("Ground");

            // 使用 Unity 默认可射线层，避免把场景交互写死在 Default 层。
            // 这里排除 Ground，确保场景物体点击检测不会被地面提前截获。
            // HACK 配置方式后续可能需要调整
            sceneLayerMask = Physics.DefaultRaycastLayers & ~groundLayerMask;
        }

        #endregion

        #region 指针上下文查询

        /// <summary>
        /// 根据屏幕坐标解析当前指针上下文。
        /// </summary>
        /// <param name="screenPosition">指针屏幕坐标。</param>
        /// <returns>包含指针位置和场景命中结果的上下文结构体。</returns>
        private PointerContext GetPointerContext()
        {
            // HACK 暂时直接使用鼠标位置，不走input system
            Vector2 screenPosition = Mouse.current.position.ReadValue();
            
            // 判定是否悬停在 UI 上，供上层控制器决定是否拦截场景点击。
            bool isOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogWarning("PointerContextService 未找到主相机，无法生成指针射线");
                return new PointerContext(screenPosition, isOverUI, SceneHitInfo.None);
            }

            // 根据屏幕坐标生成世界空间射线，供地面和场景物体共用。
            Ray pointerRay = mainCamera.ScreenPointToRay(screenPosition);

            // 场景命中用于建筑/单位选择。
            bool hasSceneHit = Physics.Raycast(pointerRay, out RaycastHit sceneHit, maxRayDistance, sceneLayerMask);

            // 地面命中用于建筑预览、单位移动目标等对地交互。
            bool isGroundHit = Physics.Raycast(pointerRay, out RaycastHit groundHit, maxRayDistance, groundLayerMask);

            // 组装场景命中信息：优先返回场景物体命中，地面命中作为补充。
            SceneHitInfo sceneHitInfo;
            if (hasSceneHit)
            {
                GameObject hitGameObject = sceneHit.transform.gameObject;
                // XXX: 这里只查命中物体自身的 SceneSelectable；若碰撞体挂在子节点上，将无法按 SceneSelectable.TargetObject 的设计映射到真实目标。
                SceneSelectable hitSelectable = hitGameObject.GetComponent<SceneSelectable>();
                sceneHitInfo = new SceneHitInfo(true, hitGameObject, hitSelectable, sceneHit.point);
            }
            else if (isGroundHit)
            {
                sceneHitInfo = new SceneHitInfo(false, null, null, groundHit.point);
            }
            else
            {
                sceneHitInfo = SceneHitInfo.None;
            }

            return new PointerContext(screenPosition, isOverUI, sceneHitInfo);
        }

        #endregion
    }
}
