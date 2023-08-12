
using System;
using UnityEngine;
using System.Collections.Generic;

public class SceneMap
{
    MapBlock m_insBlock;
    PathFinder m_insPF;
    MapMask m_insMask;

    public int m_nWidth;
    public int m_nHeight;
    public List<MaskInfo> m_vecMaskInfo;

    private int m_nGridSize = 32;

    public SceneMap()
    {
        m_insBlock = new MapBlock();
        m_insPF = new PathFinder();
        m_insMask = new MapMask();
    }

    public void Dispose()
    {
        m_insBlock.Dispose();
        m_insPF.Dispose();
        m_insMask.Dispose();
        m_insBlock = null;
        m_insPF = null;
        m_insMask = null;

        m_vecMaskInfo.Clear();
        m_vecMaskInfo = null;
    }

    public void LoadMapBlock(byte[] mapData, byte[] mapMaskData, string szMapName)
    {
        // map cel.
        if ((m_insBlock.LoadBlockFile(mapData) == false))
        {
            CDebug.LogError($"LoadMapBlock failed {szMapName}");
            return;
        }
        m_insPF.ReLoad(m_insBlock, m_insBlock.m_nWidth, m_insBlock.m_nHeight);

        // map mask.
        m_nWidth = m_insBlock.m_nWidth * m_nGridSize;
        m_nHeight = m_insBlock.m_nHeight * m_nGridSize;
        m_vecMaskInfo = m_insMask.LoadMaskInfo(mapMaskData, szMapName, m_nWidth, m_nHeight);
    }

    public bool IsBlock(int x, int y)
    {
        return m_insBlock.IsBlock(x, y);
    }

    public void SetGridSize(int nSize)
    {
        m_nGridSize = nSize;
    }

    public List<Vector2Int> FindPath(Vector2Int vStart, Vector2Int vEnd, int nDropStep, int nMaxStep)
    {
        List<Vector2Int> vecPath = new List<Vector2Int>();
        m_insPF.FindPath(vStart, vEnd, nDropStep, nMaxStep, vecPath);

        return vecPath;
    }

    public Vector2Int Pixel2Grid(float fPixelX, float fPixelY)
    {
        Vector2Int ret = new Vector2Int();
        ret.x = (int)Math.Floor(fPixelX / m_nGridSize);
        ret.y = (int)Math.Floor((m_nHeight - fPixelY) / m_nGridSize);

        return ret;
    }

    public Vector2 Grid2Pixel(int nGridX, int nGridY)
    {
        Vector2 ret = new Vector2();
        ret.x = nGridX * m_nGridSize + m_nGridSize / 2;
        ret.y = m_nHeight - nGridY * m_nGridSize - m_nGridSize / 2;

        return ret;
    }

    public int DegreeToDir(float offsetX, float offsetY)
    {
        if (offsetX == 0 && offsetY == 0)
        {
            return -1;
        }

        int nDir = -1;

        float fDegree = (float)(Math.Atan2(offsetY, offsetX) * 180 / Math.PI);
        if (fDegree >= -15 && fDegree < 15)
            nDir = 6;
        else if (fDegree >= 15 && fDegree < 75)
            nDir = 5;
        else if (fDegree >= 75 && fDegree < 105)
            nDir = 4;
        else if (fDegree >= 105 && fDegree < 165)
            nDir = 3;
        else if (fDegree >= 165 || fDegree < -165)
            nDir = 2;
        else if (fDegree >= -165 && fDegree < -105)
            nDir = 1;
        else if (fDegree >= -105 && fDegree < -75)
            nDir = 0;
        else if (fDegree >= -75 && fDegree < -15)
            nDir = 7;

        return nDir;
    }

    public int NextTurnDir(int nFromDir, int nToDir)
    {
        // 如果当前未有方向，直接转向
        if (nFromDir == -1)
            return nToDir;

        int nMaxDir = 8;
        int nCurTurn = 1;
        int nStep = Math.Abs(nToDir - nFromDir);

        if (nStep < 2)
            return nToDir;

        if (nFromDir < nToDir)
            nCurTurn = 1;
        else
            nCurTurn = -1;

        if (nStep > nMaxDir / 2)
            nCurTurn *= -1;

        int nNextDir = nFromDir + nCurTurn;
        if (nNextDir < 0)
            nNextDir = nMaxDir - 1;

        if (nNextDir >= nMaxDir)
            nNextDir = 0;

        return nNextDir;
    }

}