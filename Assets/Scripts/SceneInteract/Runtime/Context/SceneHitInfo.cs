using System;
using UnityEngine;

namespace GameDesign4.SceneInteract.Runtime
{
    /// <summary>
    /// 场景命中信息。
    /// </summary>
    [Serializable]
    public readonly struct SceneHitInfo
    {
        /// <summary>
        /// 无命中信息。
        /// </summary>
        public static SceneHitInfo None => new SceneHitInfo(false, null, null, Vector3.zero);

        /// <summary>
        /// 构造场景命中信息。
        /// </summary>
        public SceneHitInfo(bool hasHit, GameObject hitGameObject, SceneSelectable hitSelectable, Vector3 hitPoint)
        {
            HasHit = hasHit;
            HitGameObject = hitGameObject;
            HitSelectable = hitSelectable;
            HitPoint = hitPoint;
        }

        /// <summary>
        /// 是否命中。
        /// </summary>
        public bool HasHit { get; }

        /// <summary>
        /// 当前命中的场景对象。
        /// </summary>
        public GameObject HitGameObject { get; }

        /// <summary>
        /// 当前命中的可选择对象。
        /// </summary>
        public SceneSelectable HitSelectable { get; }

        /// <summary>
        /// 当前命中的世界坐标。
        /// </summary>
        public Vector3 HitPoint { get; }
    }
}
