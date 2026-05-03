using GameDesign4.Unit.Runtime;
using UnityEngine;

namespace GameDesign4.Unit.Presentation
{
    /// <summary>
    /// 单位视觉控制器。
    /// 负责动画、朝向和选中显示。
    /// </summary>
    public sealed class UnitVisualController
    {
        private const string IsRunningParameterName = "IsRunning";
        private const string IsReadyToFireParameterName = "IsReadyToFire";

        private readonly Transform unitTransform;
        private readonly Animator animator;
        private readonly GameObject selectionIndicator;

        /// <summary>
        /// 构造视觉控制器。
        /// </summary>
        public UnitVisualController(Transform unitTransform, Animator animator, GameObject selectionIndicator)
        {
            this.unitTransform = unitTransform;
            this.animator = animator;
            this.selectionIndicator = selectionIndicator;
        }

        #region 状态表现
        /// <summary>
        /// 根据当前行为状态刷新动画参数。
        /// </summary>
        public void ApplyActionState(UnitActionState actionState)
        {
            if (animator == null)
            {
                return;
            }

            bool isRunning = actionState == UnitActionState.Moving || actionState == UnitActionState.ChasingTarget;
            bool isReadyToFire = actionState == UnitActionState.Attacking;

            animator.SetBool(IsRunningParameterName, isRunning);
            animator.SetBool(IsReadyToFireParameterName, isReadyToFire);
        }
        #endregion

        #region 朝向控制
        /// <summary>
        /// 让单位朝向指定世界坐标。
        /// </summary>
        public void FaceTo(Vector3 targetPosition)
        {
            Vector3 lookDirection = targetPosition - unitTransform.position;
            lookDirection.y = 0f;

            if (lookDirection.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            unitTransform.forward = lookDirection.normalized;
        }
        #endregion

        #region 选中显示
        /// <summary>
        /// 设置选中指示器显隐。
        /// </summary>
        public void SetSelected(bool selected)
        {
            if (selectionIndicator != null)
            {
                selectionIndicator.SetActive(selected);
            }
        }
        #endregion
    }
}
