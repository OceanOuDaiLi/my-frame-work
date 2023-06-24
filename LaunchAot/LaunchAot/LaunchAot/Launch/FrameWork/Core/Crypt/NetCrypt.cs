
using Core.Interface.Crypt;

/********************************************************************
Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com

Created:	2018 ~ 2023
Filename: 	Crypt.cs
Author:		DaiLi.Ou
Descriptions: 
            用于TcpService Request/Response 字符串的加密解密.
*********************************************************************/
namespace Core.Crypt
{
    public sealed class NetCrypt : ICrypt
    {
        /// <summary>
        /// 加密算法
        /// </summary>
        private ICryptAdapter adapter;

        private string key;
        private byte[] keyBytes;

        private string iv;
        private byte[] ivBytes;

        public void SetIV(string iv)
        {
            this.iv = iv;
            this.ivBytes = System.Convert.FromBase64String(this.iv);

            if (this.ivBytes.Length != 16) throw new System.Exception("crypt iv length must be 16");
        }

        public void SetKey(string key)
        {
            if (key.Length != 32) throw new System.Exception("crypt key length must be 32");

            this.key = key;
            this.keyBytes = System.Text.Encoding.UTF8.GetBytes(this.key);
        }

        public void SetAdapter(ICryptAdapter adapter)
        {
            this.adapter = adapter;
        }

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="str">需要被加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public byte[] Encrypt(byte[] content)
        {
            if (keyBytes == null || ivBytes == null)
            {
                throw new System.Exception("crypt key or iv is invalid");
            }
            if (adapter == null)
            {
                throw new System.Exception("undefined crypt adapter");
            }
            return adapter.Encrypt(content, keyBytes, ivBytes);
        }

        /// <summary>
        /// 解密被加密的字符串
        /// </summary>
        /// <param name="str">需要被解密的字符串</param>
        /// <returns>解密后的字符串</returns>
        public byte[] Decrypt(byte[] content)
        {
            if (keyBytes == null || ivBytes == null)
            {
                throw new System.Exception("crypt key ov iv is invalid");
            }
            if (adapter == null)
            {
                throw new System.Exception("undefined crypt adapter");
            }
            return adapter.Decrypt(content, keyBytes, ivBytes);
        }
    }
}