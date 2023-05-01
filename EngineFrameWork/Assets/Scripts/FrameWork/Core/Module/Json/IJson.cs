
namespace Core.Interface.Json
{
    /// <summary>
    /// Json
    /// </summary>
    public interface IJson
    {
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="json">json数据</param>
        /// <returns>反序列化的类型</returns>
        T Decode<T>(string json);

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="item">需要序列化的对象</param>
        /// <returns>json数据</returns>
        string Encode(object item);
    }
}