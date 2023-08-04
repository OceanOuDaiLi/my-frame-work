using System;

public class Position
{
    public Position()
    {
        ;
    }
    public Position(int x1, int y1)
    {
        x = x1;
        y = y1;
    }
    public int x;
    public int y;
}

public static class BlockUtils
{
    public static Int32 MySign(Int32 x)
    {
        int t = x >> 31;
        int t2 = -x >> 31;
        return t | -t2;
    }

    public static Int32 MyAbs(Int32 x)
    {
        Int32 t = x >> 31;
        x ^= t;
        x -= t;
        return x;
    }

    public static bool LineSub(int x1, int y1, int x2, int y2, int dx, int dy, bool bThick, Func<int, int, bool> pAct)
    {
        int sx = MySign(x2 - x1);
        if (dy == 0)
        {
            for (int x = x1; x != x2; x += sx)
            {
                if (!pAct(x, y1))
                    return false;
            }
        }
        else
        {
            Position pos = new Position { x = x1, y = y1 };
            if (!pAct(pos.x, pos.y))
                return false;
            int sy = MySign(y2 - y1);
            int e = -dx >> 1;
            pos.x += sx;
            for (; pos.x != x2; pos.x += sx)
            {
                e += dy;
                if (e >= 0)
                {
                    pos.y += sy;
                    e -= dx;
                }
                if (!pAct(pos.x, pos.y))
                    return false;
                if (bThick)
                {
                    int iOtherY = MySign(e + dx / 2) * sy + pos.y;
                    if (!pAct(pos.x, iOtherY))
                        return false;
                }
            }
        }
        return true;
    }

    public static bool TL_Line(int x1, int y1, int x2, int y2, bool bThick, Func<int, int, bool> fun)
    {
        int dx = Math.Abs(x2 - x1) * 2;
        int dy = Math.Abs(y2 - y1) * 2;

        if (dx == dy)
        {
            int sx = MySign(x2 - x1);
            int sy = MySign(y2 - y1);

            while (x1 != x2)
            {
                if (!fun(x1, y1))
                    return false;
                x1 += sx;
                y1 += sy;
            }
        }
        else if (dx > dy)
        {
            if (!LineSub(x1, y1, x2, y2, dx, dy, bThick, fun))
                return false;
        }
        else
        {
            if (!LineSub(y1, x1, y2, x2, dy, dx, bThick, (y, x) => fun(x, y)))
                return false;
        }

        if (!fun(x2, y2))
            return false;

        return true;
    }
}