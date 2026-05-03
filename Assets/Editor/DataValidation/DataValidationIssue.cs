using UnityEngine;

namespace GameDesign4.EditorTools.DataValidation
{
    /// <summary>
    /// 数据校验问题。
    /// </summary>
    public readonly struct DataValidationIssue
    {
        /// <summary>
        /// 初始化校验问题。
        /// </summary>
        public DataValidationIssue(Object context, string message)
        {
            Context = context;
            Message = message;
        }

        /// <summary>
        /// 问题关联对象。
        /// </summary>
        public Object Context { get; }

        /// <summary>
        /// 问题描述。
        /// </summary>
        public string Message { get; }
    }
}
