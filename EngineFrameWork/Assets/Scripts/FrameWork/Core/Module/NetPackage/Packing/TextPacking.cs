

using System.Collections.Generic;
using Core.Interface.Network;
using Core.Buffer;

namespace Core.NetPackage
{

    /// <summary>
    /// Library 文本协议拆包器
    /// 协议格式为 数据包+换行符(\r\n)，即在每个数据包末尾加上一个换行符表示包的结束
    /// </summary>
    public class TextPacking : IPacking
    {
        public BufferBuilder Buffer { get; set; }

        public BufferBuilder EncodeBuffer { get; set; }

        public TextPacking()
        {
            Buffer = new BufferBuilder();
            EncodeBuffer = new BufferBuilder();
        }

        /// <summary>
        /// 换行符 13:\r 10:\n
        /// </summary>
        private byte[] lineFeed = new byte[] { 13, 10 };

        /// <summary>
        /// 解包
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <returns></returns>
        public byte[][] Decode(byte[] bytes)
        {

            Buffer.Push(bytes);

            int indexOf = 0;
            byte[] bodyBuffer;
            List<byte[]> package = null;

            while (true)
            {
                indexOf = Buffer.IndexOf(lineFeed);
                if (indexOf < 0) { break; }
                if (indexOf == 0) { Buffer.Shift(2); continue; }

                bodyBuffer = Buffer.Shift(indexOf);

                if (package == null) { package = new List<byte[]>(); }
                package.Add(bodyBuffer);
            }

            if (package == null) { return null; }

            return package.ToArray();

        }

        /// <summary>
        /// 封包
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <returns></returns>
        public byte[] Encode(byte[] bytes)
        {
            EncodeBuffer.Byte = bytes;
            EncodeBuffer.Push(lineFeed);
            return EncodeBuffer.Byte;

        }

        /// <summary>
        /// 清空缓存区
        /// </summary>
        public void Clear()
        {
            Buffer.Clear();
        }

    }
}
