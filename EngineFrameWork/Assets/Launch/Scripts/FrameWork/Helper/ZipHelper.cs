using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using Core.Utils.SevenZip;
using Core.Utils.SevenZip.Compression.LZMA;
using ICSharpCode.SharpZipLib.Zip.Compression;
using UnityEngine;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2018 ~ 2023
	Filename: 	ZipHelper.cs
	Author:		DaiLi.Ou
	Descriptions: Servel Optional compression method.
*********************************************************************/
public static class ZipHelper
{
    #region Way 1 # 7Zip LZMA # 压缩/解压缩 

    public static bool Compress(MemoryStream msInput, MemoryStream msOutput)
    {
        if (msInput == null || msInput.Length == 0)
        {
            return false;
        }

        msInput.Seek(0, SeekOrigin.Begin);

        var coder = new Encoder();
        coder.WriteCoderProperties(msOutput);
        msOutput.Write(BitConverter.GetBytes(msInput.Length), 0, 8);
        coder.Code(msInput, msOutput, msInput.Length, -1, null);
        msOutput.Flush();

        return true;
    }

    public static bool Decompress(MemoryStream msInput, MemoryStream msOutput)
    {
#if UNITY_EDITOR
        return DecompressCS(msInput, msOutput);
#else
        var ret = FrameWork.Launch.Utils.RuntimeApi.Decompress(msInput.GetBuffer(), (int)msInput.Length);
        msOutput.Write(ret, 0, ret.Length);
        return ret.Length > 0;
#endif
    }

    private static bool DecompressCS(MemoryStream msInput, MemoryStream msOutput)
    {
        if (msInput == null || msInput.Length == 0)
        {
            return false;
        }

        msInput.Seek(0, SeekOrigin.Begin);

        var coder = new Decoder();
        var properties = new byte[5];
        msInput.Read(properties, 0, 5);

        var fileLengthBytes = new byte[8];
        msInput.Read(fileLengthBytes, 0, 8);
        var fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

        msOutput.Capacity += (int)fileLength;
        coder.SetDecoderProperties(properties);

        coder.Code(msInput, msOutput, msInput.Length, fileLength, null);
        msOutput.Flush();

        return true;
    }



    #endregion

    #region Way 2 # ICSharpCode.SharpZipLib.Zip # 压缩/解压缩 [可选压缩格式]

    public static byte[] Zip(byte[] content)
    {
        //return content;
        Deflater compressor = new Deflater();
        compressor.SetLevel(Deflater.BEST_COMPRESSION);

        compressor.SetInput(content);
        compressor.Finish();

        using (MemoryStream bos = new MemoryStream(content.Length))
        {
            var buf = new byte[1024];
            while (!compressor.IsFinished)
            {
                int n = compressor.Deflate(buf);
                bos.Write(buf, 0, n);
            }
            return bos.ToArray();
        }
    }

    public static byte[] Unzip(byte[] content)
    {
        return Unzip(content, 0, content.Length);
    }

    public static byte[] Unzip(byte[] content, int offset, int count)
    {
        //return content;
        Inflater decompressor = new Inflater();
        decompressor.SetInput(content, offset, count);

        using (MemoryStream bos = new MemoryStream(content.Length))
        {
            var buf = new byte[1024];
            while (!decompressor.IsFinished)
            {
                int n = decompressor.Inflate(buf);
                bos.Write(buf, 0, n);
            }
            return bos.ToArray();
        }
    }

    #endregion

    #region Way 3 # System.IO.Compression # 压缩/解压缩 [可选压缩格式]

    public static byte[] Squash(byte[] content)
    {
        using (MemoryStream ms = new MemoryStream())
        using (DeflateStream stream = new DeflateStream(ms, CompressionMode.Compress, true))
        {
            stream.Write(content, 0, content.Length);
            return ms.ToArray();
        }
    }

    public static byte[] Expand(byte[] content)
    {
        return Expand(content, 0, content.Length);
    }

    public static byte[] Expand(byte[] content, int offset, int count)
    {
        using (MemoryStream ms = new MemoryStream())
        using (DeflateStream stream = new DeflateStream(new MemoryStream(content, offset, count), CompressionMode.Decompress, true))
        {
            byte[] buffer = new byte[1024];
            while (true)
            {
                int bytesRead = stream.Read(buffer, 0, 1024);
                if (bytesRead == 0)
                {
                    break;
                }
                ms.Write(buffer, 0, bytesRead);
            }
            return ms.ToArray();
        }
    }

    #endregion
}

public class SevenZipProgress : ICodeProgress
{
    public void SetProgress(long inSize, long outSize)
    {
        UnityEngine.Debug.Log("解压进度： " + outSize / inSize);
    }

}
