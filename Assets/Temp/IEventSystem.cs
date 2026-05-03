using System;
using UnityEngine;

namespace Mahjong.System.TypeEventSystem
{
    #region 接口定义
    /// <summary>
    /// 事件系统接口，提供给不同模块实例化的独立事件中心
    /// </summary>
    public interface IEventSystem
    {
        /// <summary>
        /// 发送无参事件
        /// </summary>
        void Send<T>() where T : IEvent;

        /// <summary>
        /// 发送有参事件
        /// </summary>
        void Send<T>(T e) where T : IEvent;

        /// <summary>
        /// 注册无参事件监听
        /// </summary>
        IUnRegister AddListener<T>(Action onEvent) where T : IEvent;

        /// <summary>
        /// 注册有参事件监听
        /// </summary>
        IUnRegister AddListener<T>(Action<T> onEvent) where T : IEvent;

        /// <summary>
        /// 注销事件监听
        /// </summary>
        void RemoveListener<T>(Action<T> onEvent) where T : IEvent;

        /// <summary>
        /// 清理当前事件系统的所有监听
        /// </summary>
        void Clear();
    }

    /// <summary>
    /// 所有事件的实现接口
    /// </summary>
    public interface IEvent { }

    /// <summary>
    /// 注销接口，用于消除反射调用，提升性能
    /// </summary>
    public interface IUnRegister
    {
        /// <summary>
        /// 执行注销逻辑
        /// </summary>
        void Unregister();
        // XXX: 这里把 GameObject 耦合进事件系统基础契约，会让该系统天然依赖 Unity 场景对象，不利于 Runtime/Contracts 脱离场景独立测试。
        /// <summary>
        /// 当游戏对象销毁时注销事件监听
        /// </summary>
        /// <param name="gameObject">游戏对象</param>
        void RemoveListenerWhenGameObjectDestroyed(GameObject gameObject);
    }
    #endregion
}
