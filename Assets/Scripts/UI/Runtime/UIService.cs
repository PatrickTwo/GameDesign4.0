using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameDesign4.Infrastructure.Runtime.Logging;
using GameDesign4.Infrastructure.Utilities;
using GameDesign4.UI.Definitions;
using GameDesign4.UI.Presentation;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using VContainer.Unity;

namespace GameDesign4.UI.Runtime
{
    /// <summary>
    /// 运行时 UI 服务。
    /// 负责根据配置加载并缓存全部 BasePanel，并提供基础的面板开关能力。
    /// </summary>
    public sealed class UIService : IDisposable
    {
        private readonly UIRoot uiRoot;
        private readonly IObjectResolver objectResolver;

        // 缓存全部面板，键为 PanelId。
        private readonly Dictionary<string, BasePanel> loadedPanels = new Dictionary<string, BasePanel>();

        /// <summary>
        /// 构造运行时 UI 服务。
        /// </summary>
        public UIService(
            UiPanelCatalogDef uiPanelCatalog,
            UIRoot uiRoot,
            IObjectResolver objectResolver)
        {
            Guard.EnsureNotNull(uiPanelCatalog, nameof(uiPanelCatalog));
            Guard.EnsureNotNull(uiRoot, nameof(uiRoot));
            Guard.EnsureNotNull(objectResolver, nameof(objectResolver));

            this.uiRoot = uiRoot;
            this.objectResolver = objectResolver;
            InitializePanelsAsync(uiPanelCatalog).Forget();
        }

        #region 初始化
        /// <summary>
        /// 根据面板目录配置加载并缓存所有 BasePanel。
        /// </summary>
        private async UniTask InitializePanelsAsync(UiPanelCatalogDef uiPanelCatalog)
        {
            loadedPanels.Clear();

            IReadOnlyList<UiPanelEntryDef> panelEntries = uiPanelCatalog.PanelEntries;
            for (int index = 0; index < panelEntries.Count; index++)
            {
                UiPanelEntryDef panelEntry = panelEntries[index];
                // 通过场景内绑定组件直接获取层级节点，避免字符串查找带来的脆弱性。
                Transform layerRoot = uiRoot.GetLayerRoot(panelEntry.Layer);

                // 资产数据已由编辑器校验
                UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> panelHandle =
                    Addressables.InstantiateAsync(panelEntry.AddressKey, layerRoot);
                await panelHandle.ToUniTask();
                GameObject panelInstance = panelHandle.Result;
                if (panelInstance == null)
                {
                    GameLog.Warning(GameLogModule.UI, $"UI 面板实例化失败：{panelEntry.PanelId}");
                    continue;
                }

                // 对新实例执行依赖注入，确保控制器依赖可用。
                objectResolver.InjectGameObject(panelInstance);

                Guard.Ensure(panelInstance.TryGetComponent(out BasePanel panel), $"UI UI预制体缺少 BasePanel：{panelEntry.PanelId}");

                loadedPanels.Add(panelEntry.PanelId, panel);

                if (panelEntry.OpenOnStartup)
                {
                    OpenPanel(panelEntry.PanelId);
                }
                else
                {
                    panel.Close();
                }
            }

            GameLog.Log(GameLogModule.UI, $"UI 面板缓存完成，数量：{loadedPanels.Count}");
        }
        #endregion

        #region 面板查询
        /// <summary>
        /// 尝试获取指定面板。
        /// </summary>
        private bool TryGetPanel(string panelId, out BasePanel panel)
        {
            if (string.IsNullOrWhiteSpace(panelId))
            {
                panel = null;
                return false;
            }

            return loadedPanels.TryGetValue(panelId, out panel);
        }
        #endregion

        #region 基础面板控制
        /// <summary>
        /// 判断指定面板当前是否处于打开状态。
        /// </summary>
        public bool IsPanelOpen(string panelId)
        {
            if (TryGetPanel(panelId, out BasePanel panel) == false)
            {
                return false;
            }

            return panel.IsOpen;
        }

        /// <summary>
        /// 打开指定面板。
        /// </summary>
        public void OpenPanel(string panelId)
        {
            if (TryGetPanel(panelId, out BasePanel panel) == false || panel.IsOpen)
            {
                return;
            }

            panel.Open();
            // 同层中后打开的面板显示在最前。
            panel.transform.SetAsLastSibling();
        }

        /// <summary>
        /// 关闭指定面板。
        /// </summary>
        public void ClosePanel(string panelId)
        {
            if (TryGetPanel(panelId, out BasePanel panel) == false || panel.IsOpen == false)
            {
                return;
            }

            panel.Close();
        }
        #endregion

        #region 释放
        /// <summary>
        /// 释放缓存引用。
        /// </summary>
        public void Dispose()
        {
            foreach (KeyValuePair<string, BasePanel> panelPair in loadedPanels)
            {
                if (panelPair.Value != null)
                {
                    Addressables.ReleaseInstance(panelPair.Value.gameObject);
                }
            }

            loadedPanels.Clear();
        }
        #endregion
    }
}
