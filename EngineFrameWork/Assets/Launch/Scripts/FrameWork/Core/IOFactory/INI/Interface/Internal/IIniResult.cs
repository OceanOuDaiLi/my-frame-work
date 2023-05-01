using System;
using System.Collections.Generic;

namespace Core.Interface.INI
{
    /// <summary>
    /// ini结果集
    /// </summary>
    public interface IIniResult
    {
        /// <summary>
        /// 当存储时
        /// </summary>
        event Action<string> OnSave;

        /// <summary>
        /// 获取一个ini配置
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="key">键</param>
        /// <param name="def">默认值</param>
        /// <returns>配置值</returns>
        string Get(string section, string key, string def = null);

        /// <summary>
        /// 获取一个节下的全部配置
        /// </summary>
        /// <param name="section">节</param>
        /// <returns></returns>
        Dictionary<string, string> Get(string section);

        /// <summary>
        /// 设定一个ini配置
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        void Set(string section, string key, string val);

        /// <summary>
        /// 设定一个节的配置
        /// </summary>
        /// <param name="section"></param>
        /// <param name="kv"></param>
        void Set(string section, Dictionary<string, string> kv);

        /// <summary>
        /// 移除一个ini配置
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="key">键</param>
        void Remove(string section, string key);

        /// <summary>
        /// 移除一个ini区块
        /// </summary>
        /// <param name="section">节</param>
        void Remove(string section);

        /// <summary>
        /// 保存ini文件
        /// </summary>
        void Save();
    }
}