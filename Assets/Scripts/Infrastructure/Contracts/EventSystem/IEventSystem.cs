using System;

namespace GameDesign4.Infrastructure.Contracts.Events
{
    /// <summary>
    /// 跨模块事件分发器接口。
    /// 负责发布与订阅共享事件契约。
    /// </summary>
    public interface IEventSystem
    {
        #region 事件发布
        /// <summary>
        /// 发布事件。
        /// </summary>
        /// <typeparam name="TEvent">事件类型。</typeparam>
        /// <param name="eventData">事件数据。</param>
        void Publish<TEvent>(TEvent eventData) where TEvent : struct, IGameEvent;
        #endregion

        #region 事件订阅
        /// <summary>
        /// 订阅事件。
        /// 返回标准释放句柄，便于由调用方显式解除订阅。
        /// </summary>
        /// <typeparam name="TEvent">事件类型。</typeparam>
        /// <param name="handler">事件处理委托。</param>
        /// <returns>订阅释放句柄。</returns>
        IDisposable Subscribe<TEvent>(Action<TEvent> handler) where TEvent : struct, IGameEvent;

        /// <summary>
        /// 取消订阅事件。
        /// </summary>
        /// <typeparam name="TEvent">事件类型。</typeparam>
        /// <param name="handler">事件处理委托。</param>
        void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : struct, IGameEvent;
        #endregion
    }
}
