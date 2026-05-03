using System;
using Cinemachine;
using GameDesign4.CameraControl.Contracts.Service;
using GameDesign4.CameraControl.Definition;
using UnityEngine;
using VContainer.Unity;

namespace GameDesign4.CameraControl.Runtime
{
    /// <summary>
    /// RTS 相机控制器。
    /// 负责平移、缩放、旋转以及控制开关。
    /// </summary>
    public sealed class CameraController : ITickable, IDisposable, ICameraControlService
    {
        private readonly CameraInputReader inputReader;
        private readonly CameraControlSettings settings;
        private readonly Transform cameraTarget;
        private readonly CinemachineVirtualCamera virtualCamera;
        private readonly CinemachineTransposer transposer;

        private bool isControlEnabled;
        private float targetZoomDistance;
        private Vector3 followDirection;

        /// <summary>
        /// 构造相机控制器。
        /// </summary>
        public CameraController(
            Transform cameraTarget,
            CinemachineVirtualCamera virtualCamera,
            CameraControlSettings settings)
        {
            // 初始化输入读取器。
            inputReader = new CameraInputReader();

            // 保存场景引用与配置。
            this.settings = settings;
            this.cameraTarget = cameraTarget;
            this.virtualCamera = virtualCamera;

            // 读取虚拟相机上的 Transposer 组件。
            transposer = this.virtualCamera.GetCinemachineComponent<CinemachineTransposer>();

            // 根据初始 FollowOffset 计算相机方向和目标距离。
            Vector3 offset = transposer.m_FollowOffset;
            followDirection = offset.normalized;
            targetZoomDistance = offset.magnitude;

            // 默认允许控制相机。
            isControlEnabled = true;
        }

        /// <summary>
        /// 当前是否允许操作相机。
        /// </summary>
        public bool IsControlEnabled => isControlEnabled;

        #region 控制开关
        /// <summary>
        /// 启用相机控制。
        /// </summary>
        public void EnableControl()
        {
            // 打开相机控制开关。
            isControlEnabled = true;
        }

        /// <summary>
        /// 关闭相机控制。
        /// </summary>
        public void DisableControl()
        {
            // 关闭相机控制开关。
            isControlEnabled = false;
        }
        #endregion

        #region 帧更新驱动
        /// <summary>
        /// 每帧驱动相机控制。
        /// </summary>
        public void Tick()
        {
            // 未开启控制时，不消费任何输入。
            if (isControlEnabled == false)
            {
                return;
            }

            // 依次处理平移、缩放与旋转。
            HandleMovement();
            HandleZoom();
            HandleRotation();
        }

        /// <summary>
        /// 释放相机控制资源。
        /// </summary>
        public void Dispose()
        {
            // 释放输入读取器。
            inputReader.Dispose();
        }
        #endregion

        #region 平移逻辑
        /// <summary>
        /// 处理 WASD 平移。
        /// </summary>
        private void HandleMovement()
        {
            Vector2 moveInput = inputReader.GetMoveInput();
            if (moveInput == Vector2.zero)
            {
                return;
            }

            // 基于当前相机朝向计算地面平移方向。
            Vector3 forward = GetCameraForward();
            Vector3 right = GetCameraRight();

            // 根据缩放距离计算当前有效速度。
            float distanceScale = GetDistanceSpeedScale();
            float effectiveSpeed = settings.MoveSpeed * distanceScale;
            float deltaTimeSpeed = effectiveSpeed * Time.deltaTime;

            // 组合平移向量并推动跟随目标。
            Vector3 movement = (forward * moveInput.y + right * moveInput.x) * deltaTimeSpeed;
            cameraTarget.position += movement;
        }

        /// <summary>
        /// 根据缩放距离计算平移速度倍率。
        /// </summary>
        private float GetDistanceSpeedScale()
        {
            float currentDistance = transposer.m_FollowOffset.magnitude;
            float distanceLerp = Mathf.InverseLerp(settings.MinZoomDistance, settings.MaxZoomDistance, currentDistance);
            return Mathf.Lerp(1f, 1f + settings.MoveSpeedScaleFactor, distanceLerp);
        }

        /// <summary>
        /// 获取当前相机前方向。
        /// </summary>
        private Vector3 GetCameraForward()
        {
            // 忽略 Y 轴，只保留地面平移方向。
            Vector3 forward = virtualCamera.transform.forward;
            forward.y = 0f;
            return forward.normalized;
        }

        /// <summary>
        /// 获取当前相机右方向。
        /// </summary>
        private Vector3 GetCameraRight()
        {
            // 忽略 Y 轴，只保留地面平移方向。
            Vector3 right = virtualCamera.transform.right;
            right.y = 0f;
            return right.normalized;
        }
        #endregion

        #region 缩放逻辑
        /// <summary>
        /// 处理滚轮缩放。
        /// </summary>
        private void HandleZoom()
        {
            float zoomInput = inputReader.GetZoomInput();
            if (Mathf.Abs(zoomInput) >= 0.01f)
            {
                // 应用缩放灵敏度，再更新目标距离。
                float zoomDelta = zoomInput * settings.ZoomSensitivity;
                float newDistance = targetZoomDistance - zoomDelta * settings.ZoomSpeed * Time.deltaTime;
                targetZoomDistance = Mathf.Clamp(newDistance, settings.MinZoomDistance, settings.MaxZoomDistance);
            }

            // 使用插值平滑靠近目标缩放距离。
            float currentDistance = transposer.m_FollowOffset.magnitude;
            float smoothedDistance = Mathf.Lerp(currentDistance, targetZoomDistance, settings.ZoomSmoothFactor * Time.deltaTime);
            transposer.m_FollowOffset = followDirection * smoothedDistance;
        }
        #endregion

        #region 旋转逻辑
        /// <summary>
        /// 处理 Q/E 旋转。
        /// </summary>
        private void HandleRotation()
        {
            float rotateInput = inputReader.GetRotateInput();
            if (Mathf.Abs(rotateInput) < 0.01f)
            {
                return;
            }

            // 根据输入值绕 Y 轴旋转相机偏移方向。
            float rotationAngle = rotateInput * settings.RotateSpeed * Time.deltaTime;
            Quaternion rotation = Quaternion.Euler(0f, rotationAngle, 0f);
            followDirection = rotation * followDirection;

            // 保持当前距离，仅更新方向。
            float currentDistance = transposer.m_FollowOffset.magnitude;
            transposer.m_FollowOffset = followDirection * currentDistance;
        }
        #endregion
    }
}
