
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Core.Hash
{
    /// <summary>
    /// Md5算法
    /// </summary>
    public static class Md5
    {
        /// <summary>
        /// 使用Md5来哈希文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>哈希后的字符串</returns>
        public static string ParseFile(string path)
        {
            var bufferSize = 1024 * 16;
            var buffer = new byte[bufferSize];
            string fileMd5;
            using (var fileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                HashAlgorithm hashAlgorithm = new MD5CryptoServiceProvider();
                var readLength = 0;
                var output = new byte[bufferSize];
                while ((readLength = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    hashAlgorithm.TransformBlock(buffer, 0, readLength, output, 0);
                }
                hashAlgorithm.TransformFinalBlock(buffer, 0, 0);
                var md5 = BitConverter.ToString(hashAlgorithm.Hash);
                hashAlgorithm.Clear();
                fileMd5 = md5.Replace("-", "");
            }

            return fileMd5.ToUpper();
        }

        /// <summary>
        /// 使用md5来哈希二进制数组
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ParseByte(byte[] bytes)
        {
            var md5 = new MD5CryptoServiceProvider();
            var t = md5.ComputeHash(bytes);
            var sb = new StringBuilder(32);
            for (var i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("X").PadLeft(2, '0'));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 使用Md5来Hash字符串
        /// </summary>
        /// <param name="input">输入的字符串</param>
        /// <returns>哈希后的字符串</returns>
        public static string ParseString(string input)
        {
            var md5 = new MD5CryptoServiceProvider();
            var t = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder(32);
            for (var i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("X").PadLeft(2, '0'));
            }
            return sb.ToString();
        }
    }
}