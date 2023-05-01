
using System.Collections.Generic;

namespace Core.Stl
{
    /// <summary>
    /// 缓存节点
    /// </summary>
    internal sealed class CacheNode<TKey, TVal>
    {
        /// <summary>
        /// 键值
        /// </summary>
        public KeyValuePair<TKey, TVal> KeyValue { get; private set; }

        /// <summary>
        /// 上一个节点
        /// </summary>
        public CacheNode<TKey, TVal> Backward { get; set; }

        /// <summary>
        /// 下一个节点
        /// </summary>
        public CacheNode<TKey, TVal> Forward { get; set; }

        /// <summary>
        /// 创建一个缓存节点
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        public CacheNode(TKey key, TVal val)
        {
            KeyValue = new KeyValuePair<TKey, TVal>(key, val);
        }

        /// <summary>
        /// 替换元素
        /// </summary>
        /// <param name="val">值</param>
        public void Replace(TVal val)
        {
            KeyValue = new KeyValuePair<TKey, TVal>(KeyValue.Key, val);
        }
    }
}