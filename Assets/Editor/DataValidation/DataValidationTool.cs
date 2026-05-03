using System.Collections.Generic;
using GameDesign4.Shared.Definitions;
using UnityEditor;
using UnityEngine;

namespace GameDesign4.EditorTools.DataValidation
{
    /// <summary>
    /// 编辑器数据验证工具。
    /// 负责在编辑器阶段校验主链路数据资产，减少运行时冗长校验逻辑。
    /// 对外可用接口：
    /// 1. 提供一键全量校验菜单。
    /// 2. 在数据资产导入后自动执行单资产轻校验。
    /// </summary>
    public static class DataValidationTool
    {
        // 数据资产根目录
        private const string DataRootPath = "Assets/Data";

        #region 菜单校验入口
        /// <summary>
        /// 校验当前项目主链路数据。
        /// </summary>
        [MenuItem("Tools/数据校验/校验全部主链路数据")]
        public static void ValidateAllMainData()
        {
            // 执行全部数据资产校验，并返回校验结果集合。
            List<DataValidationIssue> issues = ValidateAllDataAssets();
            ReportIssues(issues, "全量");
        }
        #endregion

        /// <summary>
        /// 执行全部数据资产校验。
        /// </summary>
        private static List<DataValidationIssue> ValidateAllDataAssets()
        {
            List<DataValidationIssue> issues = new List<DataValidationIssue>();
            ScriptableObject[] assets = LoadAllDataAssets();

            for (int index = 0; index < assets.Length; index++)
            {
                ScriptableObject asset = assets[index];
                ValidateDataAsset(asset, issues);
            }

            return issues;
        }

        #region 资产加载
        /// <summary>
        /// 加载 Data 目录下的全部 ScriptableObject 数据资产。
        /// </summary>
        private static ScriptableObject[] LoadAllDataAssets()
        {
            string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { DataRootPath });
            List<ScriptableObject> assets = new List<ScriptableObject>(guids.Length);

            for (int index = 0; index < guids.Length; index++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[index]);
                ScriptableObject asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }

            return assets.ToArray();
        }
        #endregion

        #region 规则执行
        /// <summary>
        /// 执行单个数据资产校验。
        /// </summary>
        private static void ValidateDataAsset(Object asset, List<DataValidationIssue> issues)
        {
            if (asset is not IDataValidationSelfCheck selfCheck)
            {
                return;
            }

            List<string> localMessages = new();

            // 由资产自身负责声明本地规则，工具层只负责调度与包装输出。
            selfCheck.ValidateSelf(localMessages);

            for (int index = 0; index < localMessages.Count; index++)
            {
                issues.Add(new DataValidationIssue(asset, localMessages[index]));
            }
        }

        #endregion

        #region 输出辅助
        /// <summary>
        /// 输出校验结果。
        /// </summary>
        private static void ReportIssues(List<DataValidationIssue> issues, string sourceLabel)
        {
            if (issues.Count == 0)
            {
                Debug.Log($"[数据校验][{sourceLabel}] 未发现问题。");
                return;
            }

            for (int index = 0; index < issues.Count; index++)
            {
                DataValidationIssue issue = issues[index];
                Debug.LogError($"[数据校验][{sourceLabel}] {issue.Message}", issue.Context);
            }

            Debug.LogWarning($"[数据校验][{sourceLabel}] 校验完成，发现问题数：{issues.Count}");
        }
        #endregion
    }
}
