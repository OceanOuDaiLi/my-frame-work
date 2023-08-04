
using System;
using Core.Buffer;
using System.Collections.Generic;
using System.Runtime.InteropServices;


public struct MaskInfo
{
    public string szFileName;
    public float x;
    public float y;
    public float width;
    public float height;
    public float yPos;
}
public class MapMask
{
    struct TMaskDrawHeader
    {
        public UInt32 dwType;                //file identifier
        public UInt32 dwVersion;         //file format version
        public UInt32 dwLength;          //entire file length
        public UInt32 dwReserved;			//for reserved use
        public int count;
    };

    struct TMaskDraw
    {
        public ushort dwLeft;
        public ushort dwTop;
        public ushort dwWidth;      //mask picture width
        public ushort dwHeight;        //mask picture height
        public short iYPos;            //base position in Y coordinate of mask.
    };



    int m_nHeaderSize = 20;

    int m_nItemSize = 266;

    Dictionary<string, List<MaskInfo>> m_dicCacheMask;

    public MapMask()
    {
        m_dicCacheMask = new Dictionary<string, List<MaskInfo>>();
    }

    // 解包结构体的函数
    T ByteArrayToStructure<T>(byte[] bytes) where T : struct
    {
        IntPtr ptr = Marshal.AllocHGlobal(bytes.Length);
        try
        {
            Marshal.Copy(bytes, 0, ptr, bytes.Length);
            return (T)Marshal.PtrToStructure(ptr, typeof(T));
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }

    public List<MaskInfo> LoadMaskInfo(byte[] byteData, string szMapID, int nMapWidth, int nMapHeight)
    {
        if (m_dicCacheMask.ContainsKey(szMapID))
        {
            return m_dicCacheMask[szMapID];
        }

        UInt32 MAPMASKDRAW_ID = 0x444b4d;	            //"MKD"
        BufferBuilder insBuf = new BufferBuilder();
        insBuf.Push(byteData);

        byte[] tHeader = insBuf.Shift(m_nHeaderSize);//sizeof(TMaskDrawHeader)
        List<MaskInfo> vecMapInfo = new List<MaskInfo>();

        TMaskDrawHeader pMaskHeader = ByteArrayToStructure<TMaskDrawHeader>(tHeader);

        CDebug.Assert(pMaskHeader.dwType == MAPMASKDRAW_ID);
        CDebug.Assert(pMaskHeader.dwVersion == 1);
        CDebug.Assert(pMaskHeader.dwLength == byteData.Length);

        //if (pMaskHeader.dwType != MAPMASKDRAW_ID)
        //{
        //    return vecMapInfo;
        //}
        //if (pMaskHeader.dwVersion != 1)
        //{
        //    return vecMapInfo;
        //}
        //if (pMaskHeader.dwLength != byteData.Length)
        //{
        //    return vecMapInfo;
        //}

        List<int> vList = new List<int>();
        string szMaskDir = $"map/{szMapID}/mask/";
        byte[] byteBody = insBuf.Byte;
        int nPreIndex = 0;


        for (int i = 0; i < byteBody.Length - 5; i++)
        {
            if (byteBody[i] == '.' && byteBody[i + 1] == 'p' && byteBody[i + 2] == 'n' && byteBody[i + 3] == 'g' && byteBody[i + 4] == 0)
            {
                //都按个数算
                vList.Add(i + 4 + 1 - nPreIndex);
                nPreIndex = i + 4 + 1;
            }
        }
        //item  = TMaskDraw + string
        foreach (var nLen in vList)
        {
            byte[] tBody = insBuf.Shift(10);//sizeof(TMaskDraw)
            TMaskDraw pItem = ByteArrayToStructure<TMaskDraw>(tBody);
            byte[] byteName = insBuf.Shift(nLen - 10);

            string szFileName = SeekFileName(byteName);
            szFileName = szFileName.Replace(@".png", "");
            //szFileName = szFileName.Substring(szFileName.IndexOf(@"\") + 1);//str.Substring(index + 1);//IO.Path.GetFileName(pItem.path);

            //            szFileName = System.IO.Path.ChangeExtension(szFileName, ".png");

            MaskInfo insMInfo = new MaskInfo();
            insMInfo.szFileName = szFileName;
            insMInfo.x = (float)pItem.dwLeft;
            insMInfo.y = (float)(nMapHeight - pItem.dwTop);
            insMInfo.width = (float)pItem.dwWidth;
            insMInfo.height = (float)pItem.dwHeight;
            insMInfo.yPos = (float)(nMapHeight - pItem.iYPos);

            vecMapInfo.Add(insMInfo);
        }
        m_dicCacheMask[szMapID] = vecMapInfo;
        return vecMapInfo;
    }

    private string SeekFileName(byte[] bytes)
    {
        int nSubIndex = 0;
        for (int i = bytes.Length - 1; i > 0; i--)
        {
            if (bytes[i] == 92)
            {
                nSubIndex = i;
                break;
            }
        }

        string szFileName = System.Text.Encoding.Default.GetString(bytes, nSubIndex + 1, bytes.Length - nSubIndex - 1 - 1);
        return szFileName;
    }

    public void Dispose()
    {
        ;
    }
}
