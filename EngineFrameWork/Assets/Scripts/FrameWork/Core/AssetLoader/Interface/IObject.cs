﻿
using UnityEngine;

namespace Core.Interface.Resources
{
    /// <summary>
    /// 对象信息
    /// </summary>
    public interface IObject
    {
        /// <summary>
        /// 实例化对象
        /// </summary>
        /// <returns>GameObject</returns>
        GameObject Instantiate();

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T">转换的类型</typeparam>
        /// <param name="hostedObject">宿主</param>
        /// <returns>获取对象</returns>
        T Get<T>(object hostedObject) where T : Object;

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="hostedObject">宿主</param>
        /// <returns>获取对象</returns>
        Object Get(object hostedObject);

        /// <summary>
        /// 原始对象，注意这个访问将不会引用计数
        /// </summary>
        Object Original { get; }
    }
}