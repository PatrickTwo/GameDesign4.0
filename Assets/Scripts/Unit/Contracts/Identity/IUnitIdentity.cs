using GameDesign4.Unit.Contracts.Model;

namespace GameDesign4.Unit.Contracts.Identity
{
    /// <summary>
    /// 单位身份接口。
    /// 用于向外部模块公开运行时单位唯一标识。
    /// </summary>
    public interface IUnitIdentity
    {
        /// <summary>
        /// 运行时单位唯一标识。
        /// </summary>
        UnitId UnitId { get; }
    }
}
