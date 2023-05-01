
 
using System;
using System.Collections.Generic;
using System.Text;
using Core.Interface.Network;
using Core.Buffer;

namespace Core.NetPackage
{
    
    /// <summary>
    /// Library Frame协议拆包器
    /// 协议格式为 总包长+包体，其中包长为4字节网络字节序的整数，包体可以是普通文本或者二进制数据。
    /// </summary>
    public class FramePacking : IPacking
    {
        
        public BufferBuilder Buffer { get; set; }
        
        public BufferBuilder EncodeBuffer { get; set; }


        public FramePacking()
        {
            Buffer = new BufferBuilder();
            EncodeBuffer = new BufferBuilder();
        }

        /// <summary>
        /// 解包
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <returns></returns>
        public byte[][] Decode(byte[] bytes) {

            Buffer.Push(bytes);
            if(Buffer.Length < 4){ return null; }

            List<byte[]> package = null;
            int totalSize;
            byte[] bodyBuffer;
            while(Buffer.Length >= 4){

                byte[] totSizeBytes = Buffer.Peek(4);
                Array.Reverse(totSizeBytes);
                totalSize = BitConverter.ToInt32(totSizeBytes, 0);
                if(totalSize > Buffer.Length){ break; }

                Buffer.Shift(4);
                bodyBuffer = Buffer.Shift(totalSize - 4);

                if(package == null){ package = new List<byte[]>(); }
                package.Add(bodyBuffer);
            }

            if(package == null){ return null; }

            return package.ToArray();

        }

        /// <summary>
        /// 封包
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <returns></returns>
        public byte[] Encode(byte[] bytes){

            EncodeBuffer.Byte = bytes;
            byte[] totSizeBytes = BitConverter.GetBytes((EncodeBuffer.Length + 4));
            Array.Reverse(totSizeBytes);
            EncodeBuffer.Unshift(totSizeBytes);
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
