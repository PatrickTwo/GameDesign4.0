using System;
using System.Collections.Generic;

namespace GameDesign4.Infrastructure.Utilities
{
    /// <summary>
    /// 通用前置条件守卫工具。
    /// FUNC：
    /// 1. Ensure        <see cref="Ensure"/>        确保条件成立，否则抛出异常。
    /// 2. EnsureNotNull <see cref="EnsureNotNull"/> 确保对象不为空，否则抛出异常，支持多个对象检查重载。
    /// 3. EnsureNotNullOrWhiteSpace <see cref="EnsureNotNullOrWhiteSpace"/> 确保字符串不为空且不全为空白字符，支持多个字符串检查重载。
    /// 4. EnsureInRange <see cref="EnsureInRange"/> 确保数值在指定范围内，提供int和float两种版本。
    /// 5. EnsureDictionaryContainsKey <see cref="EnsureDictionaryContainsKey"/> 确保字典包含指定键，否则抛出异常。
    /// </summary>
    public static class Guard
    {
        #region 条件检查
        /// <summary>
        /// 确保条件成立。
        /// </summary>
        public static void Ensure(bool condition, string message)
        {
            // 条件不成立时直接抛出异常，阻止错误流程继续执行。
            if (condition == false)
            {
                throw new InvalidOperationException("条件判定失败：" + message);
            }
        }
        #endregion

        #region 空值检查
        /// <summary>
        /// 确保对象不为空。
        /// </summary>
        public static void EnsureNotNull(object value, string message = "")
        {
            // 对象为空时直接抛出异常。
            if (value == null)
            {
                throw new InvalidOperationException("对象不能为空：" + message);
            }
        }

        /// <summary>
        /// 确保多个对象都不为空。
        /// </summary>
        public static void EnsureNotNull(string message, params object[] values)
        {
            // 参数集合本身不能为空。
            if (values == null)
            {
                throw new InvalidOperationException("参数集合不能为空：" + message);
            }

            // 任一对象为空时直接抛出异常。
            for (int index = 0; index < values.Length; index++)
            {
                if (values[index] == null)
                {
                    throw new InvalidOperationException($"对象不能为空，索引：{index}，{message}");
                }
            }
        }
        #endregion

        #region 字符串检查
        /// <summary>
        /// 确保字符串不为空且不全为空白字符。
        /// null、""、全是空白字符（" ", "\t", "\n"）
        ///  IsNullOrWhiteSpace 范围更大，更严格
        /// </summary>
        public static void EnsureNotNullOrWhiteSpace(string value, string message)
        {
            // 字符串为空或全为空白字符时直接抛出异常。
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException("字符串不能为空或空白：" + message);
            }
        }
        /// <summary>
        /// 确保字符串不为空。
        /// null或 ""
        /// </summary>
        /// <param name="value"></param>
        /// <param name="message"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void EnsureNotNullOrEmpty(string value, string message)
        {
            // 字符串为空时直接抛出异常。
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException("字符串不能为空：" + message);
            }
        }

        /// <summary>
        /// 确保多个字符串都不为空且不全为空白字符。
        /// </summary>
        public static void EnsureNotNullOrWhiteSpace(string message, params string[] values)
        {
            // 参数集合本身不能为空。
            if (values == null)
            {
                throw new InvalidOperationException("字符串集合不能为空：" + message);
            }

            // 任一字符串非法时直接抛出异常。
            for (int index = 0; index < values.Length; index++)
            {
                if (string.IsNullOrWhiteSpace(values[index]))
                {
                    throw new InvalidOperationException($"字符串不能为空或空白，索引：{index}，错误信息：{message}");
                }
            }
        }
        #endregion

        #region 范围检查
        /// <summary>
        /// 确保整数值在给定范围内。
        /// </summary>
        public static void EnsureInRange(int value, int minInclusive, int maxInclusive, string message)
        {
            // 超出范围时直接抛出异常。
            if (value < minInclusive || value > maxInclusive)
            {
                throw new InvalidOperationException($"数值超出范围：{message}，允许范围：[{minInclusive}, {maxInclusive}]，当前值：{value}");
            }
        }

        /// <summary>
        /// 确保浮点值在给定范围内。
        /// </summary>
        public static void EnsureInRange(float value, float minInclusive, float maxInclusive, string message)
        {
            // 超出范围时直接抛出异常。
            if (value < minInclusive || value > maxInclusive)
            {
                throw new InvalidOperationException($"数值超出范围：{message}，允许范围：[{minInclusive}, {maxInclusive}]，当前值：{value}");
            }
        }
        #endregion

        #region 字典检查
        /// <summary>
        /// 确保字典包含指定键。
        /// </summary>
        public static void EnsureDictionaryContainsKey<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key, string message)
        {
            // 字典本身不能为空。
            if (dictionary == null)
            {
                throw new InvalidOperationException("字典不能为空：" + message);
            }

            // 字典不包含指定键时直接抛出异常。
            if (dictionary.ContainsKey(key) == false)
            {
                throw new InvalidOperationException($"字典不包含指定键：{message}，键值：{key}");
            }
        }
        #endregion
    }
}
