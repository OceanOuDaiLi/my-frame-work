using Core.Interface.IO;
using System.Security.Cryptography;

/********************************************************************
Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com

Created:	    2018 ~ 2023
Filename: 	    Crypt.cs
Author:		    DaiLi.Ou
Descriptions:   使用AES256进行文件的加密解密.
*********************************************************************/
namespace Core.IO
{
    public class IOAESCrypt : IIOCrypt
    {
        /// <summary>
        /// 加密密钥
        /// </summary>
        public string key = string.Empty;

        private byte[] desrgbKey;
        private byte[] desrgbIv;

        ICryptoTransform aesDecrypt = null;
        ICryptoTransform aesEncrypt = null;

        public IOAESCrypt(string key, string iv)
        {
            this.key = key;
            this.desrgbKey = System.Convert.FromBase64String(key);
            this.desrgbIv = System.Convert.FromBase64String(iv);

            RijndaelManaged aes = new RijndaelManaged
            {
                KeySize = 256,
                BlockSize = 256,
                Padding = PaddingMode.PKCS7,
                Mode = CipherMode.ECB,
                Key = desrgbKey,
                IV = desrgbIv
            };
            aesDecrypt = aes.CreateDecryptor(desrgbKey, desrgbIv);
            aesEncrypt = aes.CreateEncryptor(desrgbKey, desrgbIv);
            aes.Clear();
        }

        ~IOAESCrypt()
        {
            aesDecrypt.Dispose();
            aesEncrypt.Dispose();
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data"><文件数据>
        /// <returns></returns>
        public byte[] Decrypted(byte[] data)
        {
            return ForDecrypted(data, key);
        }
        /// <summary>
        /// For解密
        /// </summary>
        /// <param name="decryptString"><文件字符串数据>
        /// <param name="decryptKey"><密匙>
        /// <returns></returns>
        private byte[] ForDecrypted(byte[] data, string decryptKey)
        {
            var aesBuffer = data;
            aesBuffer = aesDecrypt.TransformFinalBlock(aesBuffer, 0, aesBuffer.Length);

            return aesBuffer;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data"><文件数据>
        /// <returns></returns>
        public byte[] Encrypted(byte[] data)
        {
            return ForEncrypted(data, key);
        }

        /// <summary>
        /// for 加密
        /// </summary>
        /// <param name="encryptString"><文件字符串数据>
        /// <param name="encryptKey"><密匙>
        /// <returns></returns>
        private byte[] ForEncrypted(byte[] data, string encryptKey)
        {
            var buffer = data;
            buffer = aesEncrypt.TransformFinalBlock(buffer, 0, buffer.Length);

            return buffer;
        }

    }
}