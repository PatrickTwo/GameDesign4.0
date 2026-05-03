using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VContainer;

namespace GameDesign4.UI.Presentation
{
    /// <summary>
    /// 建造面板控制器。
    /// 负责展示当前可建建筑列表，并在点击条目时转发建造请求。
    /// </summary>
    public sealed class BuildPanelController : BasePanel
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private RectTransform itemContainer;
        [SerializeField] private BuildItemController itemTemplate;

        public override string PanelId => throw new System.NotImplementedException();

    }
}
