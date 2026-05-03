using System;
using UnityEngine;

namespace GameDesign4.CameraControl.Runtime
{
    /// <summary>
    /// 相机输入读取器。
    /// 负责读取平移、缩放和旋转输入值。
    /// </summary>
    public sealed class CameraInputReader : IDisposable
    {
        private readonly CameraControlInputMap inputMap;

        /// <summary>
        /// 构造相机输入读取器。
        /// </summary>
        public CameraInputReader()
        {
            // 初始化相机控制输入图。
            inputMap = new CameraControlInputMap();
        }

        #region 输入读取
        /// <summary>
        /// 获取平移输入。
        /// </summary>
        public Vector2 GetMoveInput()
        {
            // 读取当前 WASD 输入向量。
            return inputMap.MoveAction.ReadValue<Vector2>();
        }

        /// <summary>
        /// 获取缩放输入。
        /// </summary>
        public float GetZoomInput()
        {
            // 读取当前滚轮输入值。
            return inputMap.ZoomAction.ReadValue<float>();
        }

        /// <summary>
        /// 获取旋转输入。
        /// </summary>
        public float GetRotateInput()
        {
            // 读取当前 Q/E 旋转输入值。
            return inputMap.RotateAction.ReadValue<float>();
        }
        #endregion

        #region 资源释放
        /// <summary>
        /// 释放输入读取器资源。
        /// </summary>
        public void Dispose()
        {
            // 释放底层输入动作图。
            inputMap.Dispose();
        }
        #endregion
    }
}
