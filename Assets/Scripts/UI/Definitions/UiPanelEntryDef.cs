using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameDesign4.UI.Definitions
{
    /// <summary>
    /// UI 面板显示层级类型。
    /// 数值越大，显示层级越高。
    /// </summary>
    public enum UiLayerType
    {
        Hud = 0,
        Normal = 100,
        Popup = 200,
        Overlay = 300
    }

    /// <summary>
    /// UI 面板条目定义。
    /// 负责描述单个面板的标识、资源地址与互斥配置。
    /// </summary>
    [Serializable]
    public sealed class UiPanelEntryDef
    {
        [SerializeField] private string panelId = string.Empty;
        [SerializeField] private string addressKey = string.Empty;
        [SerializeField] private UiLayerType layer = UiLayerType.Normal;
        [SerializeField] private bool openOnStartup;
        [SerializeField] private List<string> exclusivePanelIds = new List<string>();

        /// <summary>
        /// 面板唯一标识。
        /// </summary>
        public string PanelId => panelId;

        /// <summary>
        /// 面板 Addressable 地址键。
        /// </summary>
        public string AddressKey => addressKey;

        /// <summary>
        /// 当前面板所属显示层级。
        /// </summary>
        public UiLayerType Layer => layer;

        /// <summary>
        /// 是否在启动时默认打开。
        /// </summary>
        public bool OpenOnStartup => openOnStartup;

        /// <summary>
        /// 打开当前面板时需要关闭的互斥面板列表。
        /// </summary>
        public IReadOnlyList<string> ExclusivePanelIds => exclusivePanelIds;
    }
}
