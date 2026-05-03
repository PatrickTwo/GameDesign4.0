using System.Collections.Generic;
using System.Text;

namespace GameDesign4.Shared.Runtime.Logging
{
    /// <summary>
    /// 游戏开发日志全局入口。
    /// 负责格式化日志文本并转发到 Unity Console。
    /// </summary>
    public static class GameLog
    {
        #region 可选颜色配置
        private static readonly Dictionary<GameLogModule, string> moduleColors = new()
        {
            { GameLogModule.Debug, "#00a2ff" }
        };
        #endregion
        #region 日志输出
        /// <summary>
        /// 输出普通日志。
        /// </summary>
        public static void Log(GameLogModule module, string message)
        {
            Write(module, message, LogOutputType.Log);
        }

        /// <summary>
        /// 输出警告日志。
        /// </summary>
        public static void Warning(GameLogModule module, string message)
        {
            Write(module, message, LogOutputType.Warning);
        }

        /// <summary>
        /// 输出错误日志。
        /// </summary>
        public static void Error(GameLogModule module, string message)
        {
            Write(module, message, LogOutputType.Error);
        }

        /// <summary>
        /// 输出指定模块日志。
        /// </summary>
        private static void Write(GameLogModule module, string message, LogOutputType outputType)
        {
            string normalizedMessage = string.IsNullOrWhiteSpace(message) ? "空日志消息。" : message.Trim();
            string formattedMessage = FormatMessage(module, normalizedMessage);

            // 应用模块颜色配置，未配置的模块不附加颜色标签。
            if (moduleColors.TryGetValue(module, out string color))
            {
                formattedMessage = $"<color={color}>{formattedMessage}</color>";
            }


            if (outputType == LogOutputType.Warning)
            {
                // 放置与debug命名空间冲突，使用 UnityEngine.Debug.LogWarning
                UnityEngine.Debug.LogWarning(formattedMessage);
                return;
            }

            if (outputType == LogOutputType.Error)
            {
                UnityEngine.Debug.LogError(formattedMessage);
                return;
            }

            UnityEngine.Debug.Log(formattedMessage);
        }
        #endregion

        #region 文本格式化
        /// <summary>
        /// 生成统一日志文本格式。
        /// </summary>
        private static string FormatMessage(GameLogModule module, string message)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append('[');
            stringBuilder.Append(module);
            stringBuilder.Append(']');
            stringBuilder.Append(' ');
            stringBuilder.Append(message);

            return stringBuilder.ToString();
        }
        #endregion

        #region 输出类型
        /// <summary>
        /// Unity Console 输出类型。
        /// </summary>
        private enum LogOutputType
        {
            Log = 0,
            Warning = 1,
            Error = 2
        }
        #endregion
    }
}
