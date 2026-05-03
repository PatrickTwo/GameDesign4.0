using System;
using System.Collections.Generic;
using GameDesign4.Shared.Contracts.Events;
using GameDesign4.Shared.Utilities;

namespace GameDesign4.Shared.Runtime.Events
{
    /// <summary>
    /// 跨模块事件分发器。
    /// 负责在单进程内按事件类型同步广播共享业务事件。
    /// </summary>
    public sealed class EventSystem : IEventSystem
    {
        private readonly Dictionary<Type, Delegate> listenersByType = new Dictionary<Type, Delegate>();

        #region 事件发布
        /// <summary>
        /// 发布事件。
        /// </summary>
        /// <typeparam name="TEvent">事件类型。</typeparam>
        /// <param name="eventData">事件数据。</param>
        public void Publish<TEvent>(TEvent eventData) where TEvent : struct, IGameEvent
        {
            Type eventType = typeof(TEvent);

            // 未注册监听器时直接返回，避免无意义分发。
            if (listenersByType.TryGetValue(eventType, out Delegate listenerDelegate) == false)
            {
                return;
            }

            Action<TEvent> typedListener = listenerDelegate as Action<TEvent>;
            typedListener?.Invoke(eventData);
        }
        #endregion

        #region 事件订阅
        /// <summary>
        /// 订阅事件。
        /// </summary>
        /// <typeparam name="TEvent">事件类型。</typeparam>
        /// <param name="handler">事件处理委托。</param>
        /// <returns>用于显式解除订阅的释放句柄。</returns>
        public IDisposable Subscribe<TEvent>(Action<TEvent> handler) where TEvent : struct, IGameEvent
        {
            Guard.EnsureNotNull(handler, nameof(handler));

            Type eventType = typeof(TEvent);

            if (listenersByType.TryGetValue(eventType, out Delegate listenerDelegate))
            {
                listenersByType[eventType] = Delegate.Combine(listenerDelegate, handler);
            }
            else
            {
                listenersByType.Add(eventType, handler);
            }

            return new Subscription(
                delegate
                {
                    Unsubscribe(handler);
                });
        }

        /// <summary>
        /// 取消订阅事件。
        /// </summary>
        /// <typeparam name="TEvent">事件类型。</typeparam>
        /// <param name="handler">事件处理委托。</param>
        public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : struct, IGameEvent
        {
            Guard.EnsureNotNull(handler, nameof(handler));

            Type eventType = typeof(TEvent);

            if (listenersByType.TryGetValue(eventType, out Delegate listenerDelegate) == false)
            {
                return;
            }

            Delegate updatedDelegate = Delegate.Remove(listenerDelegate, handler);

            // 当前事件没有剩余监听器时移除键，避免字典长期保留空槽位。
            if (updatedDelegate == null)
            {
                listenersByType.Remove(eventType);
                return;
            }

            listenersByType[eventType] = updatedDelegate;
        }
        #endregion

        #region 订阅释放句柄
        /// <summary>
        /// 订阅释放句柄。
        /// 用于显式解除事件订阅，避免把 Unity 生命周期耦合进基础契约。
        /// </summary>
        private sealed class Subscription : IDisposable
        {
            private Action disposeAction;

            /// <summary>
            /// 初始化一个订阅释放句柄。
            /// </summary>
            /// <param name="disposeAction">释放时执行的注销逻辑。</param>
            public Subscription(Action disposeAction)
            {
                this.disposeAction = disposeAction;
            }

            /// <summary>
            /// 执行订阅释放。
            /// </summary>
            public void Dispose()
            {
                // 只执行一次注销逻辑，避免重复释放造成额外噪音。
                if (disposeAction == null)
                {
                    return;
                }

                Action cachedDisposeAction = disposeAction;
                disposeAction = null;
                cachedDisposeAction.Invoke();
            }
        }
        #endregion
    }
}
