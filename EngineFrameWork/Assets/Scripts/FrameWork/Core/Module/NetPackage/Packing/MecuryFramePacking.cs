
 
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
    public class WebSocketFramePacking : IPacking
    {

        byte[] EncodeKey = new byte[] { 83, 109, 127, 78, 63, 215, 150, 239 };
        byte[] DecodeKey = new byte[] { 83, 109, 127, 78, 63, 215, 150, 239 };
        List<byte[]> _package = new List<byte[]>();

        public BufferBuilder Buffer { get; set; }
        
        public BufferBuilder EncodeBuffer { get; set; }


        public WebSocketFramePacking()
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
            
            if (bytes.Length <= 4)
            {
                return null;
            }

            _package.Clear();

            MercuryDecode(ref bytes, bytes.Length);
            _package.Add(bytes);

            return _package.ToArray();
            
 /*           Buffer.Push(bytes);
            if(Buffer.Length < 4){ return null; }

            
            int totalSize;
            byte[] bodyBuffer;
            while(Buffer.Length >= 4){

                byte[] totSizeBytes = Buffer.Byte;
                MercuryDecode(ref totSizeBytes, Buffer.Length);

                totalSize = Buffer.Length;
                if(totalSize > Buffer.Length){ break; }

                bodyBuffer = totSizeBytes;

                if(package == null){ package = new List<byte[]>(); }
                package.Add(bodyBuffer);

                Buffer.Clear();
            }

            if(package == null){ return null; }

            return package.ToArray();*/

        }

        private void MercuryDecode(ref byte[] message, int len)
        {
            byte keyIdx = 0;
            byte t = 0;
            byte last = (byte)(message[0] & 0xff);
            message[0] = (byte)(last ^ DecodeKey[keyIdx]);
            for (int i = 1; i < len; i++)
            {
                keyIdx = (byte)(i & 0x7);
                DecodeKey[keyIdx] = (byte)(((DecodeKey[keyIdx] + last) ^ i) & 0xff);
                t = (byte)((((message[i] & 0xff) - last) ^ DecodeKey[keyIdx]) & 0xff);
                last = (byte)(message[i] & 0xff);
                message[i] = t;
            }
        }

        public void MercuryEncode(ref byte[] message, int len)
        {
            byte keyIdx = 0;
            byte last = (byte)((message[0] ^ EncodeKey[keyIdx]) & 0xff);
            message[0] = last;
            for (int i = 1; i < len; i++)
            {
                keyIdx = (byte)(i & 0x7);
                EncodeKey[keyIdx] = (byte)(((EncodeKey[keyIdx] + last) ^ i) & 0xff);
                last = (byte)((((message[i] ^ EncodeKey[keyIdx]) & 0xff) + last) & 0xff);
                message[i] = last;
            }
        }

        /// <summary>
        /// 封包
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <returns></returns>
        public byte[] Encode(byte[] bytes){
            MercuryEncode(ref bytes, bytes.Length);
/*            EncodeBuffer.Byte = bytes;
            byte[] totSizeBytes = BitConverter.GetBytes((EncodeBuffer.Length + 4));
            Array.Reverse(totSizeBytes);
            EncodeBuffer.Unshift(totSizeBytes);*/
            return bytes;
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
