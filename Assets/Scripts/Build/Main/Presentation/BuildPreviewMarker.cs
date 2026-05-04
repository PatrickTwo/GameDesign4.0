using UnityEngine;

namespace GameDesign4.Build.Presentation
{
    /// <summary>
    /// 建造预览标记组件。
    /// 负责把正式建筑预制体切换为“仅预览”状态，避免阻挡射线和显示不必要的运行时 UI。
    /// </summary>
    public sealed class BuildPreviewMarker : MonoBehaviour
    {
        #region 预览配置
        /// <summary>
        /// 将当前实例配置为建造预览。
        /// </summary>
        public void ConfigureAsPreview()
        {
            SetLayerRecursively(gameObject, LayerMask.NameToLayer("Ignore Raycast"));
            DisableColliders();
            DisableCanvases();
        }

        /// <summary>
        /// 递归设置层级，避免预览体挡住自己的地面射线。
        /// </summary>
        private static void SetLayerRecursively(GameObject target, int layer)
        {
            if (target == null || layer < 0)
            {
                return;
            }

            target.layer = layer;
            Transform targetTransform = target.transform;
            // 递归设置子物体层级
            for (int index = 0; index < targetTransform.childCount; index++)
            {
                SetLayerRecursively(targetTransform.GetChild(index).gameObject, layer);
            }
        }

        /// <summary>
        /// 关闭预览体上的全部碰撞体。
        /// </summary>
        private void DisableColliders()
        {
            Collider[] colliders = GetComponentsInChildren<Collider>(true);
            for (int index = 0; index < colliders.Length; index++)
            {
                colliders[index].enabled = false;
            }
        }

        /// <summary>
        /// 关闭预览体上的世界空间 UI，避免生命条等表现混入预览。
        /// </summary>
        private void DisableCanvases()
        {
            Canvas[] canvases = GetComponentsInChildren<Canvas>(true);
            for (int index = 0; index < canvases.Length; index++)
            {
                canvases[index].enabled = false;
            }
        }
        #endregion
    }
}
