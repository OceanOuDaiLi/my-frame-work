
namespace Core.Crypt
{
    /// <summary>
    /// 加密适配器
    /// </summary>
    public interface ICryptAdapter
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="str">需要加密的字符串</param>
        /// <param name="key">密钥</param>
        /// <returns>加密后的值</returns>
        byte[] Encrypt(byte[] content, byte[] key, byte[] iv);

        /// <summary>
        /// 解密被加密的内容
        /// </summary>
        /// <param name="str">需要解密的字符串</param>
        /// <param name="key">密钥</param>
        /// <returns>解密后的值</returns>
        byte[] Decrypt(byte[] content, byte[] key, byte[] iv);
    }
}
