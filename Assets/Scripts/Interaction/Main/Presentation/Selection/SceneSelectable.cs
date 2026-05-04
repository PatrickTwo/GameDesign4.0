using GameDesign4.Unit.Contracts.Identity;
using GameDesign4.Unit.Contracts.Model;
using UnityEngine;

namespace GameDesign4.Interaction.Runtime
{
    /// <summary>
    /// 场景可选择对象组件。
    /// 负责声明当前对象可被场景交互系统选中，并桥接基础选中表现。
    /// </summary>
    public sealed class SceneSelectable : MonoBehaviour
    {
        [Header("选择配置")]
        [SerializeField] private Transform targetRoot;
        [SerializeField] private GameObject selectionVisual;

        private IUnitIdentity unitIdentity;

        /// <summary>
        /// 当前真实选择根节点。
        /// </summary>
        public Transform TargetRoot => targetRoot == null ? transform : targetRoot;

        #region 生命周期
        /// <summary>
        /// 初始化默认引用。
        /// </summary>
        private void Awake()
        {
            ResolveDefaultReferences();
            SetSelected(false);
        }

        /// <summary>
        /// 编辑器下自动回填默认引用。
        /// </summary>
        private void OnValidate()
        {
            ResolveDefaultReferences();
        }
        #endregion

        #region 选择状态控制
        /// <summary>
        /// 设置当前对象的选中表现。
        /// </summary>
        public void SetSelected(bool selected)
        {
            if (selectionVisual != null)
            {
                selectionVisual.SetActive(selected);
            }
        }
        #endregion

        #region 单位身份解析
        /// <summary>
        /// 尝试获取当前对象对应的单位标识。
        /// </summary>
        public bool TryGetUnitId(out UnitId unitId)
        {
            ResolveUnitIdentity();
            if (unitIdentity == null || unitIdentity.UnitId == null)
            {
                unitId = null;
                return false;
            }

            unitId = unitIdentity.UnitId;
            return true;
        }

        /// <summary>
        /// 回填默认场景引用。
        /// </summary>
        private void ResolveDefaultReferences()
        {
            if (targetRoot == null)
            {
                targetRoot = transform;
            }
        }

        /// <summary>
        /// 解析当前对象上的单位身份组件。
        /// </summary>
        private void ResolveUnitIdentity()
        {
            if (unitIdentity != null)
            {
                return;
            }

            MonoBehaviour[] behaviours = TargetRoot.GetComponents<MonoBehaviour>();
            for (int index = 0; index < behaviours.Length; index++)
            {
                MonoBehaviour behaviour = behaviours[index];
                if (behaviour is IUnitIdentity resolvedIdentity)
                {
                    unitIdentity = resolvedIdentity;
                    return;
                }
            }
        }
        #endregion
    }
}
