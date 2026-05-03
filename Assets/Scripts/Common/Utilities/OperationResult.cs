using System;

namespace GameDesign4.Infrastructure.Runtime
{
    #region 通用操作结果
    /// <summary>
    /// 通用操作结果。
    /// </summary>
    [Serializable]
    public readonly struct OperationResult
    {
        /// <summary>
        /// 成功结果。
        /// </summary>
        public static OperationResult Success()
        {
            return new OperationResult(true, string.Empty);
        }

        /// <summary>
        /// 失败结果。
        /// </summary>
        public static OperationResult Failure(string errorMessage)
        {
            return new OperationResult(false, errorMessage);
        }

        private OperationResult(bool isSuccess, string errorMessage)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage ?? string.Empty;
        }

        /// <summary>
        /// 是否成功。
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// 失败信息。
        /// </summary>
        public string ErrorMessage { get; }
    }
    #endregion
    #region 泛型操作结果
    /// <summary>
    /// 带返回值的通用操作结果。
    /// </summary>
    [Serializable]
    public readonly struct OperationResult<T>
    {
        /// <summary>
        /// 成功结果。
        /// </summary>
        public static OperationResult<T> Success(T value)
        {
            return new OperationResult<T>(true, value, string.Empty);
        }

        /// <summary>
        /// 失败结果。
        /// </summary>
        public static OperationResult<T> Failure(string errorMessage)
        {
            return new OperationResult<T>(false, default, errorMessage);
        }

        private OperationResult(bool isSuccess, T value, string errorMessage)
        {
            IsSuccess = isSuccess;
            Value = value;
            ErrorMessage = errorMessage ?? string.Empty;
        }

        /// <summary>
        /// 是否成功。
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// 返回值。
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// 失败信息。
        /// </summary>
        public string ErrorMessage { get; }
    }
    #endregion
}
