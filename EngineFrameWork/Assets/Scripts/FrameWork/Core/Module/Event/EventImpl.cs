using System;
using Core.Stl;
using FrameWork;
using System.Collections.Generic;
using Core.Interface.Event;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com

	Created:	2018 ~ 2023
	Filename: 	EventImpl.cs
	Author:		DaiLi.Ou

	Descriptions: Custom event for frame work.
*********************************************************************/
namespace Core.Event
{
    /// <summary>
    /// 事件实现
    /// </summary>
    public sealed class EventImpl : IEventImpl
    {
        private const string HandleException = "handler";
        private const string EventException = "eventName";

        /// <summary>
        /// 事件句柄
        /// </summary>
        private Dictionary<string, List<EventHandler>> handlers;

        /// <summary>
        /// 触发一个事件
        /// </summary>
        /// <param name="eventName">事件名称</param>
        public void Trigger(string eventName)
        {
            Trigger(eventName, null, EventArgs.Empty);
        }

        /// <summary>
        /// 触发一个事件
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="e">事件参数</param>
        public void Trigger(string eventName, EventArgs e)
        {
            Trigger(eventName, null, e);
        }

        /// <summary>
        /// 触发一个事件
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="sender">发送者</param>
        public void Trigger(string eventName, object sender)
        {
            Trigger(eventName, sender, EventArgs.Empty);
        }

        /// <summary>
        /// 触发一个事件
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        public void Trigger(string eventName, object sender, EventArgs e)
        {
            Guard.Requires<NullReferenceException>(App.Instance != null);
            Guard.NotEmptyOrNull(eventName, EventException);

            if (!App.Instance.IsMainThread)
            {
                App.Instance.MainThread(() =>
                {
                    Trigger(eventName, sender, e);
                });
                return;
            }

            if (handlers == null)
            {
                return;
            }

            if (!handlers.ContainsKey(eventName) || handlers[eventName].Count <= 0)
            {
                return;
            }

            CallEvent(handlers[eventName], sender, e);
        }

        /// <summary>
        /// 注册一个事件
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="handler">事件句柄</param>
        /// <param name="life">在几次后事件会被自动释放</param>
        /// <returns>事件句柄</returns>
        public IEventHandler On(string eventName, System.EventHandler handler, int life = 0)
        {
            Guard.NotEmptyOrNull(eventName, EventException);
            Guard.NotNull(handler, HandleException);
            Guard.Requires<ArgumentOutOfRangeException>(life >= 0);

            var callHandler = new EventHandler(this, eventName, handler, life);
            if (!App.Instance.IsMainThread)
            {
                App.Instance.MainThread(() =>
                {
                    On(eventName, callHandler);
                });
                return callHandler;
            }
            On(eventName, callHandler);
            return callHandler;
        }

        /// <summary>
        /// 注册一个事件，调用一次后就释放
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="handler">事件句柄</param>
        /// <returns></returns>
        public IEventHandler One(string eventName, System.EventHandler handler)
        {
            return On(eventName, handler, 1);
        }

        /// <summary>
        /// 移除一个事件
        /// </summary>
        /// <param name="handler">操作句柄</param>
        internal void Off(EventHandler handler)
        {
            Guard.NotNull(handler, HandleException);

            if (!App.Instance.IsMainThread)
            {
                App.Instance.MainThread(() =>
                {
                    Off(handler);
                });
                return;
            }

            if (!handlers.ContainsKey(handler.EventName))
            {
                return;
            }

            handlers[handler.EventName].Remove(handler);
            if (handlers[handler.EventName].Count <= 0)
            {
                handlers.Remove(handler.EventName);
            }
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="handler">事件句柄</param>
        private void On(string eventName, EventHandler handler)
        {
            if (handlers == null)
            {
                handlers = new Dictionary<string, List<EventHandler>>();
            }
            if (!handlers.ContainsKey(eventName))
            {
                handlers.Add(eventName, new List<EventHandler>());
            }
            handlers[eventName].Add(handler);
        }

        /// <summary>
        /// 调用事件
        /// </summary>
        /// <param name="handler">事件</param>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void CallEvent(IList<EventHandler> handler, object sender, EventArgs e)
        {
            List<EventHandler> removeList = null;
            for (var i = 0; i < handler.Count; i++)
            {
                handler[i].Call(sender, e);
                if (handler[i].IsLife)
                {
                    continue;
                }
                if (removeList == null)
                {
                    removeList = new List<EventHandler>();
                }
                removeList.Add(handler[i]);
            }

            if (removeList == null)
            {
                return;
            }

            for (var i = 0; i < removeList.Count; i++)
            {
                removeList[i].Cancel();
            }
        }
    }
}