using GameDesign4.Build.Definition;
using UnityEngine;

namespace GameDesign4.Build.Component
{
    /// <summary>
    /// 建筑实体组件。
    /// 负责承载场景建筑实例的定义引用与部署出生点。
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class BuildingEntity : MonoBehaviour
    {
        [SerializeField] private Transform deploySpawnPoint;

        [SerializeField] private BuildingDef definition;

        /// <summary>
        /// 当前建筑定义。
        /// </summary>
        public BuildingDef Definition => definition;

        /// <summary>
        /// 当前部署出生点。
        /// </summary>
        public Transform DeploySpawnPoint => deploySpawnPoint;

    }
}
