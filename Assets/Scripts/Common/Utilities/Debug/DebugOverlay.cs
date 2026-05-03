using System.Collections.Generic;
using GameDesign4.Shared.Runtime.Logging;
using GameDesign4.Shared.Utilities;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace GameDesign4.Shared.Runtime.Debug
{
    /// <summary>
    /// 通用调试叠层组件。
    /// 通过 VContainer 注入所有 IDebugOutput 实现，使用 UI Toolkit 渲染调试信息。
    /// Inspector 绑定 PanelSettings 和 UXML 模板资产。
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public sealed class DebugOverlay : MonoBehaviour
    {
        [SerializeField] private PanelSettings panelSettings;
        [SerializeField] private VisualTreeAsset outputTemplate;

        private IReadOnlyList<IDebugOutput> outputs;
        private readonly Dictionary<IDebugOutput, Label> outputLabels = new Dictionary<IDebugOutput, Label>();

        private UIDocument uiDocument;
        private VisualElement rootContainer;
        private bool isUIBuilt;

        #region 依赖注入
        /// <summary>
        /// 由 VContainer 注入所有调试输出源。
        /// </summary>
        [Inject]
        public void Construct(IReadOnlyList<IDebugOutput> outputs)
        {
            this.outputs = outputs;
            GameLog.Log(GameLogModule.Debug, "调试叠层组件已注入 " + outputs.Count + " 个调试输出源。");
            TryBuildUI();
        }
        #endregion

        #region 生命周期
        /// <summary>
        /// 初始化 UIDocument，尝试构建叠层。
        /// </summary>
        private void Start()
        {
            uiDocument = GetComponent<UIDocument>();
            uiDocument.panelSettings = panelSettings;
            TryBuildUI();
        }

        /// <summary>
        /// 每帧刷新所有调试输出源的文本内容。
        /// </summary>
        private void Update()
        {
            RefreshOutputs();
        }
        #endregion

        #region UI 构建
        /// <summary>
        /// 当注入和 UIDocument 都就绪时构建叠层。
        /// VContainer 注入可能早于 Start，因此两处都尝试构建。
        /// </summary>
        private void TryBuildUI()
        {
            if (isUIBuilt || outputs == null || outputs.Count == 0 || uiDocument == null)
            {
                return;
            }

            BuildUI();
        }

        /// <summary>
        /// 克隆 UXML 模板为每个输出源创建 UI，绑定标题和内容标签。
        /// </summary>
        private void BuildUI()
        {
            rootContainer = new VisualElement();
            rootContainer.style.position = Position.Absolute;
            rootContainer.style.left = 8;
            rootContainer.style.top = 8;
            rootContainer.style.flexDirection = FlexDirection.Column;

            foreach (IDebugOutput output in outputs)
            {
                VisualElement instance = outputTemplate.CloneTree();

                Label nameLabel = instance.Q<Label>("output-name");
                Label contentLabel = instance.Q<Label>("output-content");

                nameLabel.text = output.OutputName;
                contentLabel.text = output.GetDebugText();

                rootContainer.Add(instance);
                outputLabels[output] = contentLabel;
            }

            uiDocument.rootVisualElement.Add(rootContainer);
            isUIBuilt = true;
            GameLog.Log(GameLogModule.Debug, "DebugOverlay UI 构建完成");
        }
        #endregion

        #region 输出刷新
        /// <summary>
        /// 刷新所有调试输出源的文本内容。
        /// </summary>
        private void RefreshOutputs()
        {
            if (isUIBuilt == false)
            {
                return;
            }

            foreach (IDebugOutput output in outputs)
            {
                if (outputLabels.TryGetValue(output, out Label contentLabel))
                {
                    contentLabel.text = output.GetDebugText();
                }
            }
        }
        #endregion
    }
}
