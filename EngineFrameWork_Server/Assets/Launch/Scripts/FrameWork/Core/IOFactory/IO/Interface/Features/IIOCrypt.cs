
namespace Core.Interface.IO
{
    /// <summary>
    /// 文件加解密接口
    /// </summary>
    public interface IIOCrypt
    {
        /// <summary>
        /// 解密文件
        /// </summary>
        /// <returns></returns>            
        byte[] Decrypted(byte[] data);

        /// <summary>
        /// 加密文件
        /// </summary>
        /// <returns></returns>
        byte[] Encrypted(byte[] data);

    }


}
