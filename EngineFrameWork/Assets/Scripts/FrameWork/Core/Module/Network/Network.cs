using FrameWork;
using Core.Interface;
using System.Collections;
using Core.Interface.Network;
using System.Collections.Generic;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com

	Created:	2018 ~ 2023
	Filename: 	Network.cs
	Author:		DaiLi.Ou

	Descriptions: Network Base Class.
*********************************************************************/
namespace Core.Network
{
    /// <summary>
    /// 网络服务
    /// </summary>
    public sealed class Network : IDestroy
    {
        public enum NetworkRequestType
        {
            Http,
            Tcp,
            UnityWeb,
            WebSocket,
        }

        public enum NetworkConfigKey
        {
            Packing,
            Protocol,
            Host,
            Port,
            Timeout,
            ContentType,
            Accept
        }

        /// <summary>
        /// 建立出的连接器
        /// </summary>
        private readonly Dictionary<NetworkRequestType, IConnector> connector = new Dictionary<NetworkRequestType, IConnector>();

        /// <summary>
        /// 创建一个网络链接
        /// </summary>
        /// <param name="type">连接名</param>
        public T Create<T>(NetworkRequestType type, Hashtable config) where T : IConnector
        {
            if (this.connector.ContainsKey(type))
            {
                return (T)this.connector[type];
            }
            IConnector connector = null;
            switch (type)
            {
                case NetworkRequestType.Http:
                    connector = new HttpWebRequest(); break;
                case NetworkRequestType.Tcp:
                    connector = new TcpRequest(); break;
                case NetworkRequestType.UnityWeb:
                    connector = new WebRequest(); break;
                case NetworkRequestType.WebSocket:
                    connector = new WebSocketRequest(); break;
            }
            this.connector.Add(type, connector);
            InitConnector(connector, config);
            App.Instance.StartCoroutine(connector.StartServer());
            return (T)connector;
        }

        /// <summary>
        /// 释放一个网络链接
        /// </summary>
        /// <param name="type">连接名</param>
        public void Destroy(NetworkRequestType type)
        {
            if (connector.ContainsKey(type))
            {
                connector[type].Destroy();
            }
            connector.Remove(type);
        }

        /// <summary>
        /// 获取一个链接器
        /// </summary>
        /// <typeparam name="T">连接类型</typeparam>
        /// <param name="type">连接名</param>
        /// <returns>连接器</returns>
        public T Get<T>(NetworkRequestType type) where T : IConnector
        {
            if (connector.ContainsKey(type))
            {
                return (T)connector[type];
            }
            return default(T);
        }

        /// <summary>
        /// 当释放时
        /// </summary>
        public void OnDestroy()
        {
            foreach (var conn in connector)
            {
                conn.Value.Destroy();
            }
            connector.Clear();
        }

        /// <summary>
        /// 初始化连接器
        /// </summary>
        /// <param name="connector">连接器</param>
        /// <param name="name">连接名</param>
        /// <param name="table">配置表</param>
        private void InitConnector(IConnector connector, Hashtable table)
        {
            if (table == null)
            {
                return;
            }
            connector.SetConfig(table);
        }
    }
}
