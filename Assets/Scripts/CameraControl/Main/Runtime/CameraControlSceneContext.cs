using Cinemachine;
using GameDesign4.CameraControl.Definition;
using UnityEngine;

namespace GameDesign4.CameraControl.Runtime
{
    /// <summary>
    /// 相机场景上下文。
    /// 负责聚合相机控制运行所需的场景节点与配置。
    /// </summary>
    public sealed class CameraControlSceneContext
    {
        /// <summary>
        /// 构造相机场景上下文。
        /// </summary>
        public CameraControlSceneContext(
            Transform cameraTarget,
            CinemachineVirtualCamera virtualCamera,
            CameraControlSettings settings)
        {
            CameraTarget = cameraTarget;
            VirtualCamera = virtualCamera;
            Settings = settings;
        }

        /// <summary>
        /// 相机跟随目标节点。
        /// </summary>
        public Transform CameraTarget { get; }

        /// <summary>
        /// Cinemachine 虚拟相机。
        /// </summary>
        public CinemachineVirtualCamera VirtualCamera { get; }

        /// <summary>
        /// 相机控制配置。
        /// </summary>
        public CameraControlSettings Settings { get; }
    }
}
