using System;

namespace GameDesign4.Grid.Contracts.Model
{
    /// <summary>
    /// 网格坐标。
    /// 使用整数索引描述逻辑格位置。
    /// </summary>
    public readonly struct GridCoord : IEquatable<GridCoord>
    {
        /// <summary>
        /// 构造网格坐标。
        /// </summary>
        public GridCoord(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// 水平索引。
        /// </summary>
        public int X { get; }

        /// <summary>
        /// 垂直索引。
        /// </summary>
        public int Y { get; }

        #region 运算符重载
        /// <summary>
        /// 判断两个网格坐标是否相等。
        /// </summary>
        public bool Equals(GridCoord other)
        {
            // 只有横纵索引同时一致时，两个格坐标才表示同一格。
            return X == other.X && Y == other.Y;
        }

        /// <summary>
        /// 判断当前对象是否与另一个对象相等。
        /// </summary>
        public override bool Equals(object obj)
        {
            // 仅在目标对象也是网格坐标时继续比较。
            return obj is GridCoord other && Equals(other);
        }

        /// <summary>
        /// 获取哈希值。
        /// </summary>
        public override int GetHashCode()
        {
            // 使用简单稳定的整型组合，保证字典和哈希集合可用。
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }

        /// <summary>
        /// 相等运算符。
        /// </summary>
        public static bool operator ==(GridCoord left, GridCoord right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// 不相等运算符。
        /// </summary>
        public static bool operator !=(GridCoord left, GridCoord right)
        {
            return left.Equals(right) == false;
        }
        #endregion
    }
}
