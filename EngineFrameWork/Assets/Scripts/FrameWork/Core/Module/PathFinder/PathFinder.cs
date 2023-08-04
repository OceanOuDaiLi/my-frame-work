
using System;
using UnityEngine;
using System.Collections.Generic;

public class PathFinder
{
    public class Link
    {
        public int h;                           // 步数
        public int x;                           // 位置
        public int y;                           // 位置
        public int f;                           // 价值
        public Link next;                       // 下一个节点
        public Link father;                     // 上一个节点
    }

    private int m_iEndX;
    private int m_iEndY;
    private int m_iWidth;
    private int m_iHeight;
    private byte[] m_pbtMap;                    // 地图
    private ushort[] m_pdwDisMap;               // 地图上各点到目标点的距离

    private Link[] m_pBuf;                      // 一个大的缓冲区
    private Link m_pSorted;                     // 排序后的链表
    private MapBlock m_pBlock;

    static int ALLOC_NODE = 50000;
    static int s_maxUsedNode = 0;

    public PathFinder()
    {
        m_pBuf = new Link[ALLOC_NODE];
        for (int i = 0; i < m_pBuf.Length; i++)
        {
            m_pBuf[i] = new Link();
        }
        m_pBlock = new MapBlock();
    }

    // ... 其他成员变量和方法的定义

    private int Judge(int x, int y)
    {
        int dx = Math.Abs(m_iEndX - x) * 7;
        int dy = Math.Abs(m_iEndY - y) * 10;
        double distance = Math.Sqrt((dx * dx + dy * dy));
        return (int)distance;
    }

    public void ReLoad(MapBlock block, int width, int height)
    {
        if (m_pdwDisMap != null)
        {
            m_pdwDisMap = null;
        }

        // Expand the size by one row and one column
        m_iWidth = width + 2;
        m_iHeight = height + 2;
        int iLen = m_iWidth * m_iHeight;
        m_pdwDisMap = new ushort[iLen];

        m_pBlock = block;
        m_pbtMap = block.GetBlockData();

    }

    public void AddTile(Link newNode)
    {
        Link last = m_pSorted;
        Link q2 = null;

        while (newNode.f > last.f)
        {
            q2 = last.next;
            if (newNode.f <= q2.f)
            {
                break;
            }
            last = q2.next;
        }

        if (last.f >= newNode.f)
        {
            newNode.next = last;

            if (last == m_pSorted)
            {
                m_pSorted = newNode;
            }
            else
            {
                q2.next = newNode;
            }
        }
        else
        {
            newNode.next = q2;
            last.next = newNode;
        }
    }

    public bool ForAddTile(int nIndex, Link root, int h, int x, int y)
    {
        var pNode = m_pBuf[nIndex];
        pNode.father = root;
        pNode.h = h;
        pNode.x = x;
        pNode.y = y;
        pNode.f = h + Judge(x, y);

        AddTile(pNode);
        return true;
    }

    // ... C++ AddTile函数的C#版本的实现
    public bool FindBasePath(Vector2Int start, Vector2Int end, out Vector2Int[] path, out int len, int max_h)
    {
        // VM_START

        bool ret = false;
        //Array.Clear(m_pdwDisMap, 0xFFFF, m_iWidth * m_iHeight);  //fill map to default value "ff"
        for (int k = 0; k < m_pdwDisMap.Length; k++)
        {
            m_pdwDisMap[k] = 0xFFFF;
        }

        int iStartX = start.x + 1;
        int iStartY = start.y + 1;
        m_iEndX = end.x + 1;
        m_iEndY = end.y + 1;

        CDebug.Assert(start.x >= 0 && start.x < m_iWidth - 2);
        CDebug.Assert(end.x >= 0 && end.x < m_iWidth - 2);
        CDebug.Assert(start.y >= 0 && start.y < m_iHeight - 2);
        CDebug.Assert(end.y >= 0 && end.y < m_iHeight - 2);

        int iUsed = 2;

        Link root = m_pBuf[0];
        root.h = -1;
        root.f = int.MaxValue;
        root.father = null;
        root.next = null;

        m_pSorted = m_pBuf[1];
        m_pSorted.x = iStartX;
        m_pSorted.y = iStartY;
        m_pSorted.h = 0;
        m_pSorted.father = null;
        m_pSorted.f = Judge(iStartX, iStartY);
        m_pSorted.next = root;

        for (; ; )
        {
            root = m_pSorted;
            m_pSorted = m_pSorted.next;

            if (root.h < 0 || root.h > max_h)
                break;

            int x = root.x;
            int y = root.y;
            if (x == m_iEndX && y == m_iEndY)
            {
                ret = true;
                break;                                                  // succ get to dest
            }

            ushort h = (ushort)(root.h + 10);
            int offset = x + (y - 1) * m_iWidth;

            if (m_pbtMap[offset] == 0 && h < m_pdwDisMap[offset])       // if not obstacle and have no best node ,abort current action
            {
                m_pdwDisMap[offset] = h;                                // save best distance
                ForAddTile(iUsed++, root, h, x, y - 1); // up
            }
            h += 2;
            offset++;
            if (m_pbtMap[offset] == 0 && h < m_pdwDisMap[offset])       // if not obstacle and have no best node ,abort current action
            {
                m_pdwDisMap[offset] = h;                                // save best distance
                ForAddTile(iUsed++, root, h, x + 1, y - 1);             // up right
            }
            offset -= 2;
            if (m_pbtMap[offset] == 0 && h < m_pdwDisMap[offset])       // if not obstacle and have no best node ,abort current action
            {
                m_pdwDisMap[offset] = h;                                // save best distance
                ForAddTile(iUsed++, root, h, x - 1, y - 1);             // up left
            }

            offset += m_iWidth;
            h -= 5;
            if (m_pbtMap[offset] == 0 && h < m_pdwDisMap[offset])       // if not obstacle and have no best node ,abort current action
            {
                m_pdwDisMap[offset] = h;                                // save best distance
                ForAddTile(iUsed++, root, h, x - 1, y);                 // left
            }

            offset += 2;
            if (m_pbtMap[offset] == 0 && h < m_pdwDisMap[offset])       // if not obstacle and have no best node ,abort current action
            {
                m_pdwDisMap[offset] = h;                                // save best distance
                ForAddTile(iUsed++, root, h, x + 1, y);                 // right
            }

            offset += m_iWidth - 1;
            h += 3;
            if (m_pbtMap[offset] == 0 && h < m_pdwDisMap[offset])       // if not obstacle and have no best node ,abort current action
            {
                m_pdwDisMap[offset] = h;                                // save best distance
                ForAddTile(iUsed++, root, h, x, y + 1);                 // down
            }

            offset++;
            h += 2;
            if (m_pbtMap[offset] == 0 && h < m_pdwDisMap[offset])       /* if not obstacle and have no best node ,abort current action */
            {
                m_pdwDisMap[offset] = h;                                /* save best distance */
                ForAddTile(iUsed++, root, h, x + 1, y + 1);             //down right
            }

            offset -= 2;
            if (m_pbtMap[offset] == 0 && h < m_pdwDisMap[offset])       /* if not obstacle and have no best node ,abort current action */
            {
                m_pdwDisMap[offset] = h;                                /* save best distance */
                ForAddTile(iUsed++, root, h, x - 1, y + 1);             //down left
            }

            if (iUsed >= ALLOC_NODE - 10)
                break;
        }

        // ... 循环结束后的代码
        if (s_maxUsedNode < iUsed)
            s_maxUsedNode = iUsed;

        // save success node to path list
        Link p = root;
        len = 0;
        while (p != null)
        {
            len++;
            p = p.father;
        }
        p = root;
        path = new Vector2Int[len];
        int i;
        for (i = len - 1; p != null; i--)
        {
            path[i] = new Vector2Int();
            path[i].x = p.x - 1;
            path[i].y = p.y - 1;
            p = p.father;
        }
        CDebug.Assert(i == -1);

        return ret;
    }

    public bool IsBlocked(Vector2Int pos)
    {
        return m_pBlock.IsBlock(pos.x, pos.y);
    }

    public bool HaveBlock(Vector2Int posStart, Vector2Int posEnd)
    {
        return m_pBlock.HaveBlock(posStart.x, posStart.y, posEnd.x, posEnd.y);
    }

    public bool FindPath(Vector2Int startGrid, Vector2Int endGrid, int dropStep, int maxStep, List<Vector2Int> pointList)
    {
        // Check if start and end points are blocked
        if (IsBlocked(startGrid))
        {
            return false;
        }

        if (IsBlocked(endGrid))
        {
            int endX;
            int endY;
            m_pBlock.MaxBlock(startGrid.x, startGrid.y, endGrid.x, endGrid.y, out endX, out endY);

            endGrid.x = endX;
            endGrid.y = endY;
        }

        // Check if start and end points are the same
        if (startGrid.x == endGrid.x && startGrid.y == endGrid.y)
        {
            return false;
        }

        // Check if there are no obstacles between start and end points
        if (!HaveBlock(startGrid, endGrid))
        {
            int dx = endGrid.x - startGrid.x;
            int dy = endGrid.y - startGrid.y;

            if (Math.Abs(dx) <= dropStep && Math.Abs(dy) <= dropStep)
            {
                return false;
            }

            pointList.Add(startGrid);

            if (dropStep == 0)
            {
                pointList.Add(endGrid);
            }
            else
            {
                int signx = Math.Sign(dx);
                int signy = Math.Sign(dy);

                if (dx * signx >= dy * signy)
                {
                    dy -= dropStep * signx * dy / dx;
                    dx -= dropStep * signx;
                }
                else
                {
                    dx -= dropStep * signy * dx / dy;
                    dy -= dropStep * signy;
                }

                Vector2Int end = new Vector2Int();
                end.x = startGrid.x + dx;
                end.y = startGrid.y + dy;

                pointList.Add(end);
            }
        }
        else
        {
            int iPathLen;
            Vector2Int[] ptPath;

            // Find base path
            if (!FindBasePath(startGrid, endGrid, out ptPath, out iPathLen, maxStep * 3))
            {
                return false;
            }

            if (dropStep != 0)
            {
                if (iPathLen > dropStep + 1)
                {
                    iPathLen -= dropStep;
                }
                else
                {
                    return false;
                }
            }

            pointList.Add(ptPath[0]);

            if (!HaveBlock(ptPath[0], ptPath[iPathLen - 1]))
            {
                pointList.Add(ptPath[iPathLen - 1]);
            }
            else
            {
                int iStart, iEndS, iEndE, iEndM;
                iStart = 0;
                while (iStart < iPathLen - 1)
                {
                    iEndS = iStart + 1;
                    iEndE = iPathLen - 1;

                    if (iStart == 0 || HaveBlock(ptPath[iStart], ptPath[iEndE]))
                    {
                        while (iEndS < iEndE)
                        {
                            iEndM = (iEndS + iEndE) >> 1;
                            if (HaveBlock(ptPath[iStart], ptPath[iEndM]))
                            {
                                iEndE = iEndM - 1;
                            }
                            else
                            {
                                iEndS = iEndM + 1;
                            }
                        }

                        if (HaveBlock(ptPath[iStart], ptPath[iEndE]))
                        {
                            iEndE--;
                        }

                        if (!HaveBlock(ptPath[iEndE - 1], ptPath[iEndE + 1]))
                        {
                            int iStart2 = iEndE + 1;
                            iEndS = iEndE - 2;
                            iEndE = iStart + 1;

                            while (iEndS > iEndE)
                            {
                                iEndM = (iEndS + iEndE) >> 1;
                                if (HaveBlock(ptPath[iStart2], ptPath[iEndM]))
                                {
                                    iEndE = iEndM + 1;
                                }
                                else
                                {
                                    iEndS = iEndM - 1;
                                }
                            }

                            if (HaveBlock(ptPath[iStart2], ptPath[iEndE]))
                            {
                                iEndE++;
                            }

                            while (HaveBlock(ptPath[iStart], ptPath[iEndE]))
                            {
                                iEndE++;
                            }
                        }
                    }
                    CDebug.Assert(iEndE > iStart);
                    pointList.Add(ptPath[iEndE]);
                    iStart = iEndE;
                }
            }
        }

        return true;
    }

    public void Dispose()
    {
        m_pBuf = null;
        m_pSorted = null;
        m_pdwDisMap = null;
        m_pbtMap = null;
        m_pBlock = null;
    }
}