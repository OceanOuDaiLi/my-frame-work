
using System;
using Core.Buffer;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class MapBlock
{
    struct TCellHeader
    {
        public Int32 dwType;                    //file identifier
        public Int32 dwVersion;                 //file format version
        public Int32 dwLength;                  //entire file length
        public Int32 dwReserved;                //for reserved use
        public Int32 dwCellWidth;               //MapCell m_iWidth
        public Int32 dwCellHeight;              //MapCell Height
    };

    static byte[][] s_vByteArray;
    public int m_nWidth = 0;
    public int m_nHeight = 0;

    private bool m_bNotBlock;
    private int m_nHeaderSize;
    private List<Byte> m_vBlockData;

    private int m_DestX;
    private int m_DestY;

    static int s_nExtraLen = 1;

    public void Dispose()
    {
        m_vBlockData = null;
        s_vByteArray = null;
    }

    public MapBlock()
    {
        InitConvertData();
        m_vBlockData = new List<byte>();
        m_nHeaderSize = 24;                     //sizeof(TCellHeader);
    }

    ~MapBlock()
    {
        m_vBlockData = null;
    }

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

    private void InitConvertData()
    {
        if (s_vByteArray != null)
            return;

        s_vByteArray = new byte[256][];
        for (int i = 0; i < 256; i++)
        {
            s_vByteArray[i] = new byte[8];

            for (int j = 0; j < 8; j++)
            {
                s_vByteArray[i][j] = (byte)((i >> (7 - j)) & 1);
                s_vByteArray[i][j] *= 255;
            }
        }
    }

    private bool IsValid(int x, int y)
    {
        return (x >= 0)
            && (y >= 0)
            && (x < m_nWidth)
            && (y < m_nHeight);
    }

    private int Offset(int x, int y)
    {
        return (x + s_nExtraLen) + ((y + s_nExtraLen) * (m_nWidth + 2));
    }

    private bool JudgeIsBlock(int x, int y)
    {
        int nIndex = Offset(x, y);
        return m_vBlockData[nIndex] != 0;
    }


    public byte[] StructureToByteArray(object obj)
    {
        int size = Marshal.SizeOf(obj);
        byte[] bytes = new byte[size];
        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(obj, ptr, false);
        Marshal.Copy(ptr, bytes, 0, size);
        Marshal.FreeHGlobal(ptr);

        return bytes;
    }


    public bool LoadBlockFile(byte[] byteData)
    {
        BufferBuilder insBuf = new BufferBuilder();
        insBuf.Push(byteData);

        byte[] tHeader = insBuf.Shift(m_nHeaderSize);
        var header = ByteArrayToStructure<TCellHeader>(tHeader);

        m_nHeight = header.dwCellHeight;
        m_nWidth = header.dwCellWidth;

        int nMaxLen = m_nHeight * m_nWidth;
        if (insBuf.Length * 8 != nMaxLen)
        {

            CDebug.LogError("map error" + insBuf.Length * 8 + "|" + nMaxLen);
            return false;
        }

        int nWidthCount = 0;

        m_vBlockData.Clear();
        for (int i = 0; i < m_nWidth + 2; i++)
        {
            m_vBlockData.Add(0xff);
        }

        foreach (byte nByte in insBuf.Byte)
        {
            if (nWidthCount == 0)
            {
                m_vBlockData.Add(0xff);

            }
            byte[] tmp = s_vByteArray[nByte];
            foreach (byte bit in tmp)
            {
                m_vBlockData.Add(bit);
            }

            nWidthCount++;
            //尾部添加地图最大行尾添加 0xff
            if (nWidthCount == (m_nWidth / 8))
            {
                m_vBlockData.Add(0xff);
                nWidthCount = 0;
            }
        }

        return true;
    }

    public bool IsBlock(int x, int y)
    {
        if (!IsValid(x, y)) return true;
        return JudgeIsBlock(x, y);
    }

    public bool Do(int x, int y)
    {
        if (JudgeIsBlock(x, y) ^ m_bNotBlock)
            return true;

        m_DestX = x;
        m_DestY = y;

        return false;
    }

    public void SetBlock(int x, int y, byte value)
    {
        //CC_ASSERT(IsValid(x, y));
        m_vBlockData[Offset(x, y)] = value;
    }

    public bool HaveBlock(int x1, int y1, int x2, int y2)
    {
        if (!IsValid(x1, y1) || !IsValid(x2, y2))
            return true;

        m_bNotBlock = true;
        return !BlockUtils.TL_Line(x1, y1, x2, y2, true, (x, y) =>
        {
            return !JudgeIsBlock(x, y);
        });
    }

    public void MaxBlock(int currentX, int currentY, int destX, int destY, out int nOutX, out int nOutY)
    {

        CDebug.Assert(!IsBlock(currentX, currentY));
        CDebug.Assert(IsBlock(destX, destY));

        if (!IsValid(destX, destY))
        {
            nOutX = currentX;
            nOutY = currentY;
            return;
        }

        m_bNotBlock = false;
        BlockUtils.TL_Line(destX, destY, currentX, currentY, true, (x, y) =>
        {
            return Do(x, y);
        });

        CDebug.Assert(!IsBlock(m_DestX, m_DestY));
        nOutX = m_DestX;
        nOutY = m_DestY;
    }

    public byte[] GetBlockData()
    {
        return m_vBlockData.ToArray();
    }
}
