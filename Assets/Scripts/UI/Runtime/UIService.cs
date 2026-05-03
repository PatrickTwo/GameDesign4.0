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
    /// 负责根据配置加载并缓存全部 BasePanel，并统一提供面板开关能力。
    /// 对外可用接口：
    /// 1. 初始化并缓存所有 BasePanel
    /// 2. 提供统一的面板打开、关闭、切换接口
    /// </summary>
    public sealed class UIService : IUIService, IStartable, IDisposable
    {
        private readonly UIRoot uiRoot;
        private readonly IObjectResolver objectResolver;

        // 缓存全部面板，键为 PanelId。
        private readonly Dictionary<string, BasePanel> openedPanels = new();
        // 面板互斥关系，键为 PanelId，值为互斥的其他 PanelId 列表。
        private readonly Dictionary<string, List<string>> panelExclusive = new();

        /// <summary>
        /// 面板打开顺序栈。
        /// 用于支持按最近打开顺序关闭面板。
        /// </summary>
        private readonly List<string> openedPanelOrder = new();

        public void Start()
        {
            GameLog.Log(GameLogModule.UI, "UI 服务启动");
        }
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
            openedPanels.Clear();
            panelExclusive.Clear();
            openedPanelOrder.Clear();

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

                openedPanels.Add(panelEntry.PanelId, panel);

                panelExclusive[panelEntry.PanelId] = new(panelEntry.ExclusivePanelIds);

                if (panelEntry.OpenOnStartup)
                {
                    OpenPanel(panelEntry.PanelId);
                }
                else
                {
                    panel.Close();
                }
            }

            GameLog.Log(GameLogModule.UI, $"UI 面板缓存完成，数量：{openedPanels.Count}");
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

            return openedPanels.TryGetValue(panelId, out panel);
        }
        #endregion

        #region IUiPanelService 实现方法
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

            if (panelExclusive.TryGetValue(panelId, out List<string> exclusivePanelIds))
            {
                for (int index = 0; index < exclusivePanelIds.Count; index++)
                {
                    ClosePanel(exclusivePanelIds[index]);
                }
            }

            panel.Open();
            openedPanelOrder.Remove(panelId);
            openedPanelOrder.Add(panelId);
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
            openedPanelOrder.Remove(panelId);
        }

        /// <summary>
        /// 关闭最近打开的面板。
        /// </summary>
        public void CloseLastOpenedPanel()
        {
            for (int index = openedPanelOrder.Count - 1; index >= 0; index--)
            {
                string panelId = openedPanelOrder[index];
                if (IsPanelOpen(panelId) == false)
                {
                    openedPanelOrder.RemoveAt(index);
                    continue;
                }

                ClosePanel(panelId);
                return;
            }
        }

        /// <summary>
        /// 切换指定面板的打开状态。
        /// </summary>
        public void TogglePanel(string panelId)
        {
            if (IsPanelOpen(panelId))
            {
                ClosePanel(panelId);
                return;
            }

            OpenPanel(panelId);
        }
        #endregion

        #region 释放
        /// <summary>
        /// 释放缓存引用。
        /// </summary>
        public void Dispose()
        {
            foreach (KeyValuePair<string, BasePanel> panelPair in openedPanels)
            {
                if (panelPair.Value != null)
                {
                    Addressables.ReleaseInstance(panelPair.Value.gameObject);
                }
            }

            openedPanels.Clear();
            panelExclusive.Clear();
            openedPanelOrder.Clear();
        }
        #endregion
    }
}
