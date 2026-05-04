using System.Text;
using GameDesign4.Infrastructure.Runtime.Debug;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameDesign4.Infrastructure.Runtime.Pointer
{
    /// <summary>
    /// 指针上下文服务。
    /// 负责根据屏幕坐标执行场景与地面射线检测，不承载具体业务语义。
    /// </summary>
    public sealed class PointerContextService
    {
        private readonly int groundLayerMask;
        private readonly int sceneLayerMask;
        private readonly float maxRayDistance;

        /// <summary>
        /// 构造指针上下文服务。
        /// </summary>
        public PointerContextService()
        {
            groundLayerMask = LayerMask.GetMask("Ground");
            sceneLayerMask = Physics.DefaultRaycastLayers & ~groundLayerMask;
            maxRayDistance = 1000f;
        }

        #region 指针上下文查询
        /// <summary>
        /// 根据屏幕坐标构建一次指针命中上下文。
        /// </summary>
        public PointerContext GetPointerContext(Vector2 screenPosition)
        {
            EventSystem eventSystem = EventSystem.current;
            bool isOverUI = eventSystem != null && eventSystem.IsPointerOverGameObject();

            Camera sceneCamera = Camera.main;
            if (sceneCamera == null)
            {
                return new PointerContext(screenPosition, isOverUI, null, Vector3.zero, false, Vector3.zero);
            }

            Ray pointerRay = sceneCamera.ScreenPointToRay(screenPosition);

            Transform hitTransform = null;
            Vector3 sceneHitPoint = Vector3.zero;
            if (Physics.Raycast(pointerRay, out RaycastHit sceneHit, maxRayDistance, sceneLayerMask))
            {
                hitTransform = sceneHit.transform;
                sceneHitPoint = sceneHit.point;
            }

            bool hasGroundHit = Physics.Raycast(pointerRay, out RaycastHit groundHit, maxRayDistance, groundLayerMask);
            Vector3 groundHitPoint = hasGroundHit ? groundHit.point : Vector3.zero;

            return new PointerContext(screenPosition, isOverUI, hitTransform, sceneHitPoint, hasGroundHit, groundHitPoint);
        }
        #endregion
    }

    /// <summary>
    /// 指针上下文调试输出。
    /// 负责实时读取当前指针状态，并将通用命中结果格式化到调试面板。
    /// </summary>
    public sealed class PointerContextDebugOutput : IDebugOutput
    {
        private readonly PointerContextService pointerContextService;

        /// <summary>
        /// 构造指针上下文调试输出。
        /// </summary>
        public PointerContextDebugOutput(PointerContextService pointerContextService)
        {
            this.pointerContextService = pointerContextService;
        }

        /// <summary>
        /// 调试输出标题。
        /// </summary>
        public string OutputName => "指针上下文";

        #region 调试文本生成
        /// <summary>
        /// 获取当前帧的指针上下文调试文本。
        /// </summary>
        public string GetDebugText()
        {
            // 使用 Legacy Input 读取鼠标位置，避免为调试输出额外引入 InputSystem 依赖。
            Vector2 screenPosition = Input.mousePosition;
            PointerContext pointerContext = pointerContextService.GetPointerContext(screenPosition);

            StringBuilder builder = new StringBuilder(256);
            builder.Append("屏幕坐标: ").Append(FormatVector2(pointerContext.ScreenPosition)).AppendLine();
            builder.Append("悬停UI: ").Append(pointerContext.IsOverUI ? "是" : "否").AppendLine();
            builder.Append("命中对象: ").Append(GetHitObjectText(pointerContext.HitTransform)).AppendLine();
            builder.Append("场景命中点: ").Append(FormatOptionalPoint(pointerContext.HitTransform != null, pointerContext.SceneHitPoint)).AppendLine();
            builder.Append("命中地面: ").Append(pointerContext.HasGroundHit ? "是" : "否").AppendLine();
            builder.Append("地面命中点: ").Append(FormatOptionalPoint(pointerContext.HasGroundHit, pointerContext.GroundHitPoint));
            return builder.ToString();
        }
        #endregion

        /// <summary>
        /// 生成命中对象显示文本。
        /// </summary>
        private static string GetHitObjectText(Transform hitTransform)
        {
            if (hitTransform == null)
            {
                return "无";
            }

            return hitTransform.name;
        }

        /// <summary>
        /// 格式化二维坐标。
        /// </summary>
        private static string FormatVector2(Vector2 value)
        {
            return "(" + value.x.ToString("F2") + ", " + value.y.ToString("F2") + ")";
        }

        /// <summary>
        /// 格式化三维坐标。
        /// </summary>
        private static string FormatVector3(Vector3 value)
        {
            return "(" + value.x.ToString("F2") + ", " + value.y.ToString("F2") + ", " + value.z.ToString("F2") + ")";
        }

        /// <summary>
        /// 按命中状态格式化可选坐标。
        /// </summary>
        private static string FormatOptionalPoint(bool hasPoint, Vector3 value)
        {
            return hasPoint ? FormatVector3(value) : "无";
        }
    }
}
