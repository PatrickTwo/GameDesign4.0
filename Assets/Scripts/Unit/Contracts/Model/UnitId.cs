using System;

namespace GameDesign4.Unit.Contracts.Model
{
    /// <summary>
    /// 单位唯一标识。
    /// 用于跨模块定位具体单位。
    /// </summary>
    public sealed class UnitId : IEquatable<UnitId>
    {
        /// <summary>
        /// 标识字符串值。
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// 当前标识是否有效。
        /// </summary>
        public bool IsValid => string.IsNullOrWhiteSpace(Value) == false;

        /// <summary>
        /// 创建一个新的运行时单位标识。
        /// </summary>
        public UnitId()
        {
            Value = Guid.NewGuid().ToString("N");
        }

        #region 运算符重载
        /// <summary>
        /// 判断两个单位标识是否相等。
        /// </summary>
        public bool Equals(UnitId other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            return string.Equals(Value, other.Value, StringComparison.Ordinal);
        }

        /// <summary>
        /// 判断当前对象是否与单位标识相等。
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as UnitId);
        }

        /// <summary>
        /// 获取哈希值。
        /// </summary>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// 相等运算符。
        /// </summary>
        public static bool operator ==(UnitId left, UnitId right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            {
                return false;
            }

            return left.Equals(right);
        }

        /// <summary>
        /// 不相等运算符。
        /// </summary>
        public static bool operator !=(UnitId left, UnitId right)
        {
            return (left == right) == false;
        }
        #endregion

        #region 调试输出
        /// <summary>
        /// 返回标识字符串。
        /// </summary>
        public override string ToString()
        {
            return Value;
        }
        #endregion
    }
}
