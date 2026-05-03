using System;
using UnityEngine.InputSystem;

namespace GameDesign4.CameraControl.Runtime
{
    /// <summary>
    /// 相机控制输入映射。
    /// 负责创建并启用相机控制所需的输入动作。
    /// </summary>
    public sealed class CameraControlInputMap : IDisposable
    {
        private readonly InputActionMap cameraControlActionMap;

        /// <summary>
        /// 构造相机控制输入映射。
        /// </summary>
        public CameraControlInputMap()
        {
            // 创建相机控制动作图。
            cameraControlActionMap = new InputActionMap("CameraControl");

            // 注册 WASD 平移输入。
            MoveAction = cameraControlActionMap.AddAction("MoveCamera", InputActionType.Value);
            MoveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");

            // 注册滚轮缩放输入。
            ZoomAction = cameraControlActionMap.AddAction("ZoomCamera", InputActionType.PassThrough, "<Mouse>/scroll/y");

            // 注册 Q/E 旋转输入。
            RotateAction = cameraControlActionMap.AddAction("RotateCamera", InputActionType.Value);
            RotateAction.AddCompositeBinding("1DAxis")
                .With("Negative", "<Keyboard>/e")
                .With("Positive", "<Keyboard>/q");

            // 启用输入动作图。
            cameraControlActionMap.Enable();
        }

        /// <summary>
        /// 相机平移输入动作。
        /// </summary>
        public InputAction MoveAction { get; }

        /// <summary>
        /// 相机缩放输入动作。
        /// </summary>
        public InputAction ZoomAction { get; }

        /// <summary>
        /// 相机旋转输入动作。
        /// </summary>
        public InputAction RotateAction { get; }

        #region 输入释放
        /// <summary>
        /// 释放输入动作图。
        /// </summary>
        public void Dispose()
        {
            // 停止接收相机控制输入。
            cameraControlActionMap.Disable();
        }
        #endregion
    }
}
