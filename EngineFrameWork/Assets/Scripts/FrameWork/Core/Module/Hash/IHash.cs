
namespace Core.Interface.Hash
{
    /// <summary>
    /// 哈希
    /// </summary>
    public interface IHash
    {
        /// <summary>
        /// 盐
        /// </summary>
        string GenerateSalt { get; }

        /// <summary>
        /// 哈希一个输入值
        /// </summary>
        /// <param name="input">输入值</param>
        /// <returns></returns>
        string Make(string input);

        /// <summary>
        /// 检查是否匹配
        /// </summary>
        /// <param name="input">输入值</param>
        /// <param name="hash">需要验证的hash</param>
        /// <returns>是否匹配</returns>
        bool Check(string input, string hash);

        /// <summary>
        /// 用于计算文件的md5，您不应该用它进行密码等高敏感的hash加密
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>md5加密的值</returns>
        string FileMd5(string path);

        /// <summary>
        /// 用于计算二进制byte的md5，您不应该用它进行密码等高敏感的hash加密
        /// </summary>
        /// <param name="bytes">二进制byte</param>
        /// <returns>md5加密的值</returns>
        string ByteMd5(byte[] bytes);

        /// <summary>
        /// 字符串md5，您不应该用它进行密码等高敏感的hash加密
        /// </summary>
        /// <param name="input">输入的字符串</param>
        /// <returns>md5加密的值</returns>
        string StringMd5(string input);
    }
}