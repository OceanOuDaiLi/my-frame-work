using System;
using System.IO;
using UnityEngine;
using Library.Interface.IO;
using System.Security.Cryptography;

namespace Library.IO
{
    public class IODESCrypt : IIOCrypt
    {
        /// <summary>
        /// 加密密钥
        /// </summary>
        public string key = string.Empty;

        private byte[] desrgbKey;
        private byte[] desrgbIv;

        public IODESCrypt(string key, string iv)
        {
            this.key = key;
            desrgbKey = Convert.FromBase64String(key);
            desrgbIv = Convert.FromBase64String(iv);
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
        private byte[] ForDecrypted(byte[] data, string decryptKey) //接change 加
        {
            CryptoStream cryptoStream = null;
            MemoryStream memoryStream = null;
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                memoryStream = new MemoryStream();
                cryptoStream = new CryptoStream(memoryStream, des.CreateDecryptor(desrgbKey, desrgbIv), CryptoStreamMode.Write);
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.FlushFinalBlock();
                return memoryStream.ToArray();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return null;
            }
            finally
            {
                if (cryptoStream != null)
                {
                    cryptoStream.Close();
                    cryptoStream.Dispose();
                }
                if (memoryStream != null)
                {
                    memoryStream.Close();
                    memoryStream.Dispose();
                }
            }
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
            MemoryStream memoryStream = null;
            CryptoStream cryptoStream = null;

            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                memoryStream = new MemoryStream();
                cryptoStream = new CryptoStream(memoryStream, des.CreateEncryptor(desrgbKey, desrgbIv), CryptoStreamMode.Write);
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.FlushFinalBlock();
                return memoryStream.ToArray();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return null;
            }
            finally
            {
                if (cryptoStream != null)
                {
                    cryptoStream.Close();
                    cryptoStream.Dispose();
                }
                if (memoryStream != null)
                {
                    memoryStream.Close();
                    memoryStream.Dispose();
                }
            }
        }

    }
}