using System.Collections.Generic;

namespace GameDesign4.Infrastructure.Definitions
{
    /// <summary>
    /// 数据资产自校验接口。
    /// 负责让资产对象声明自身的本地校验规则。
    /// </summary>
    public interface IDataValidationSelfCheck
    {
        #region 自校验入口
        /// <summary>
        /// 执行当前对象的本地校验。
        /// </summary>
        void ValidateSelf(List<string> issues);
        #endregion
    }
}
