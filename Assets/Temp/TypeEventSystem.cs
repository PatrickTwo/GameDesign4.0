/*
    基于类型的实例事件系统
    实现IEvent接口的类或结构体（可以含有成员作为参数）可以作为事件发送
    支持自动事件注销：在注册事件时通过调用扩展方法实现自动注销功能
*/
using System;
using System.Collections.Generic;
using UnityEngine;
using Mahjong.System.TypeEventSystem;

namespace Mahjong.System.TypeEventSystem
{
    #region 自动注销
    /// <summary>
    /// 每注册一个事件返回一个移除器对象，调用RemoveListenerWhenGameObjectDestroyed方法即可实现自动注销功能
    /// 优化说明：实现IUnRegister接口，内部持有事件系统引用，彻底移除原本的反射注销逻辑，提升性能
    /// </summary>
    /// <typeparam name="T">事件类型</typeparam>
    public class ListenerAutoRemover<T> : IUnRegister where T : IEvent
    {
        /// <summary>
        /// 要移除的事件委托
        /// </summary>
        public Action<T> OnEvent { get; set; }

        /// <summary>
        /// 对应的事件系统实例
        /// </summary>
        public IEventSystem EventSystem { get; set; }

        /// <summary>
        /// 当GameObject销毁时自动注销
        /// </summary>
        public void RemoveListenerWhenGameObjectDestroyed(GameObject gameObject)
        {
            RemoveListenerOnDestroyComp trigger = gameObject.GetComponent<RemoveListenerOnDestroyComp>();
            if (!trigger)
            {
                trigger = gameObject.AddComponent<RemoveListenerOnDestroyComp>();
            }
            trigger.AddToRemove(this);
        }

        /// <summary>
        /// 执行注销逻辑
        /// </summary>
        public void Unregister()
        {
            if (EventSystem != null && OnEvent != null)
            {
                EventSystem.RemoveListener<T>(OnEvent);
                EventSystem = null;
                OnEvent = null;
            }
        }
    }
    #endregion

    #region 自动注销组件
    /// <summary>
    /// 自动注销事件组件，该组件会记录当前GameObject上注册的所有事件注销器
    /// </summary>
    public class RemoveListenerOnDestroyComp : MonoBehaviour
    {
        // 优化说明：不再按Type和object字典存储，直接存储IUnRegister接口列表，避免装箱拆箱和复杂的反射调用
        private readonly List<IUnRegister> autoRemovers = new List<IUnRegister>();

        /// <summary>
        /// 添加自动注销器
        /// </summary>
        public void AddToRemove(IUnRegister autoRemoveListener)
        {
            autoRemovers.Add(autoRemoveListener);
        }

        private void OnDestroy()
        {
            // 优化说明：直接遍历接口调用Unregister，消除了原有的GetMethod和MakeGenericMethod反射调用，极大提升销毁时的性能
            foreach (IUnRegister remover in autoRemovers)
            {
                remover.Unregister();
            }
            autoRemovers.Clear();
        }
    }
    #endregion

    #region 事件系统主要实现
    /// <summary>
    /// 基于类型的实例事件系统，不同模块可以new自己的TypeEventSystem实例，避免使用全局事件总线
    /// </summary>
    // XXX: 该实现本质上是按事件类型广播的通用总线，若直接作为项目级跨模块通信主通道，会让依赖关系隐藏在订阅处，不符合当前项目强调的稳定契约与清晰边界。
    public class TypeEventSystem : IEventSystem
    {
        /// <summary>
        /// 事件集合类接口，用于在字典中统一存储泛型集合
        /// </summary>
        private interface IListenersSet { }

        /// <summary>
        /// 事件集合类，类型为T的事件监听器集合
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        private class ListenersSet<T> : IListenersSet where T : IEvent
        {
            // 优化说明：去除了原有的默认空委托(obj => { })，避免不必要的内存分配，改为在触发时判空
            public Action<T> OnEvent;
        }

        // 优化说明：改为实例字段，不再使用static静态全局字典，支持多实例(不同模块独立事件系统)
        private readonly Dictionary<Type, IListenersSet> eventListenerSet = new Dictionary<Type, IListenersSet>();

        #region 发送
        /// <summary>
        /// 发送无参事件
        /// </summary>
        public void Send<T>() where T : IEvent
        {
            // XXX: 当 T 是引用类型事件时，default(T) 会发送 null，监听器只要读取事件字段就有空引用风险，事件载荷约束不稳定。
            Send<T>(default(T));
        }

        /// <summary>
        /// 发送有参事件
        /// </summary>
        public void Send<T>(T e) where T : IEvent
        {
            Type type = typeof(T);

            if (eventListenerSet.TryGetValue(type, out IListenersSet listenersSetObj))
            {
                ListenersSet<T> listenersSet = listenersSetObj as ListenersSet<T>;
                if (listenersSet != null && listenersSet.OnEvent != null)
                {
                    // 若需要打印日志，可在此处取消注释。为避免添加不存在的命名空间或类，这里默认隐去HLogger
                    // HLogger.Log(String.Format("发送事件：{0}", e.GetType().Name));
                    listenersSet.OnEvent.Invoke(e);
                }
            }
        }
        #endregion

        #region 注册
        /// <summary>
        /// 无参事件注册
        /// </summary>
        public IUnRegister AddListener<T>(Action onEvent) where T : IEvent
        {
            // 优化说明：将无参委托包装为有参委托
            Action<T> action = (e) => onEvent();
            return AddListener<T>(action);
        }

        /// <summary>
        /// 带参事件注册
        /// </summary>
        public IUnRegister AddListener<T>(Action<T> onEvent) where T : IEvent
        {
            Type type = typeof(T);
            ListenersSet<T> listenersSet;

            if (eventListenerSet.TryGetValue(type, out IListenersSet listenersSetObj))
            {
                listenersSet = listenersSetObj as ListenersSet<T>;
            }
            else
            {
                listenersSet = new ListenersSet<T>();
                eventListenerSet.Add(type, listenersSet);
            }

            listenersSet.OnEvent += onEvent;

            // 优化说明：将当前事件系统实例传入移除器，便于后续无反射注销
            ListenerAutoRemover<T> remover = new ListenerAutoRemover<T>()
            {
                OnEvent = onEvent,
                EventSystem = this
            };
            return remover;
        }
        #endregion

        #region 注销
        /// <summary>
        /// 注销事件
        /// </summary>
        public void RemoveListener<T>(Action<T> onEvent) where T : IEvent
        {
            Type type = typeof(T);

            if (eventListenerSet.TryGetValue(type, out IListenersSet listenersSetObj))
            {
                ListenersSet<T> listenersSet = listenersSetObj as ListenersSet<T>;
                if (listenersSet != null)
                {
                    listenersSet.OnEvent -= onEvent;
                }
            }
        }

        /// <summary>
        /// 清理当前事件系统的所有监听
        /// </summary>
        public void Clear()
        {
            eventListenerSet.Clear();
        }
        #endregion
    }
    #endregion
}
