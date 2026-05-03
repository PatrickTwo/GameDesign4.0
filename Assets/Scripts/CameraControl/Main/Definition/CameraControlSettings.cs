using System;
using UnityEngine;

namespace GameDesign4.CameraControl.Definition
{
    /// <summary>
    /// 相机控制参数。
    /// 负责存储 RTS 相机平移、缩放与旋转配置。
    /// </summary>
    [Serializable]
    public sealed class CameraControlSettings
    {
        [Header("平移")]
        [SerializeField] private float moveSpeed = 50f;
        [SerializeField] private float moveSpeedScaleFactor = 2f;

        [Header("缩放")]
        [SerializeField] private float zoomSmoothFactor = 10f;
        [SerializeField] private float zoomSpeed = 100f;
        [SerializeField] private float zoomSensitivity = 1f;
        [SerializeField] private float minZoomDistance = 30f;
        [SerializeField] private float maxZoomDistance = 200f;

        [Header("旋转")]
        [SerializeField] private float rotateSpeed = 90f;

        /// <summary>
        /// 相机基础平移速度。
        /// </summary>
        public float MoveSpeed => moveSpeed;

        /// <summary>
        /// 相机距离越远时的平移速度加成。
        /// </summary>
        public float MoveSpeedScaleFactor => moveSpeedScaleFactor;

        /// <summary>
        /// 缩放平滑系数。
        /// </summary>
        public float ZoomSmoothFactor => zoomSmoothFactor;

        /// <summary>
        /// 缩放速度。
        /// </summary>
        public float ZoomSpeed => zoomSpeed;

        /// <summary>
        /// 缩放输入灵敏度。
        /// </summary>
        public float ZoomSensitivity => zoomSensitivity;

        /// <summary>
        /// 最小缩放距离。
        /// </summary>
        public float MinZoomDistance => minZoomDistance;

        /// <summary>
        /// 最大缩放距离。
        /// </summary>
        public float MaxZoomDistance => maxZoomDistance;

        /// <summary>
        /// 旋转速度。
        /// </summary>
        public float RotateSpeed => rotateSpeed;
    }
}
