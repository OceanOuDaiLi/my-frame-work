
using UnityEngine;

namespace Core.Interface.Resources
{
    /// <summary>
    /// 资源
    /// </summary>
    public interface IResources
    {
        /// <summary>
        /// 增加后缀关系
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="extension">对应后缀</param>
        void AddExtension(System.Type type, string extension);

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <returns>加载的对象</returns>
        IObject Load(string path);

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="type">资源类型</param>
        /// <returns>加载的对象</returns>
        IObject Load(string path, System.Type type);

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="path">资源路径</param>
        /// <returns>加载的对象</returns>
        IObject Load<T>(string path) where T : Object;

        /// <summary>
        /// 异步加载
        /// </summary>
        /// <param name="path">加载路径</param>
        /// <param name="callback">回调</param>
        /// <returns>协程</returns>
        UnityEngine.Coroutine LoadAsync(string path, System.Action<IObject> callback);

        /// <summary>
        /// 异步加载
        /// </summary>
        /// <param name="path">加载路径</param>
        /// <param name="type">资源类型</param>
        /// <param name="callback">回调</param>
        /// <returns>协程</returns>
        UnityEngine.Coroutine LoadAsync(string path, System.Type type, System.Action<IObject> callback);

        /// <summary>
        /// 异步加载
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="path">加载路径</param>
        /// <param name="callback">回调</param>
        /// <returns>协程</returns>
        UnityEngine.Coroutine LoadAsync<T>(string path, System.Action<IObject> callback) where T : Object;
    }
}