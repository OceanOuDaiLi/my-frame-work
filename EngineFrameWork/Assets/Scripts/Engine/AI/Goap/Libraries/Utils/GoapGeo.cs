using UnityEngine;

namespace Goap.Utils
{

    public static class GoapGeo
    {
        public static float CrossProd(Vector2 aA, Vector2 aB, Vector2 aC)
        {
            float x1 = aB.x - aA.x;
            float y1 = aB.y - aA.y;
            float x2 = aC.x - aA.x;
            float y2 = aC.y - aA.y;
            return (x1 * y2 - x2 * y1);
        }

        public static float ScalarProd(Vector2 aA, Vector2 aB, Vector2 aC)
        {
            float x1 = aB.x - aA.x;
            float y1 = aB.y - aA.y;
            float x2 = aC.x - aA.x;
            float y2 = aC.y - aA.y;
            return (x1 * x2 + y1 * y2);
        }

        public static bool IsPointOnEdge(Vector2 aA, Vector2 aB, Vector2 aC)
        {
            float prod1 = CrossProd(aA, aB, aC);
            float prod2 = ScalarProd(aC, aA, aB);
            return (prod1 == 0 && prod2 <= 0);
        }

        /// <summary>
        /// Checks if segments AB and CD intersect.
        /// </summary>
        /// <param name="aA"></param>
        /// <param name="aB"></param>
        /// <param name="aC">.</param>
        /// <param name="aD">.</param>
        /// <returns>true: Is Crossed.</returns>
        public static bool IsEdgeCross(Vector2 aA, Vector2 aB, Vector2 aC, Vector2 aD)
        {
            float prod1 = CrossProd(aA, aB, aC);
            float prod2 = CrossProd(aA, aB, aD);
            float prod3 = CrossProd(aD, aC, aA);
            float prod4 = CrossProd(aD, aC, aB);

            // 如果 C 和 D 位于 AB 的相对两侧，则线段 AB 和 CD 相交
            // A 和 B 位于 CD 的相对两侧。
            bool isCross = (prod1 * prod2 < 0 && prod3 * prod4 < 0);

            //末端检测
            bool cond1 = IsPointOnEdge(aA, aB, aC);
            bool cond2 = IsPointOnEdge(aA, aB, aD);
            bool cond3 = IsPointOnEdge(aC, aD, aA);
            bool cond4 = IsPointOnEdge(aC, aD, aB);

            //检查其中一个段的一端是否属于另一端
            //so 位于射线上的边缘被排除在考虑之外。
            bool cond1A = (cond1 && !(cond2 || cond3 || cond4));
            bool cond2A = (cond2 && !(cond1 || cond3 || cond4));
            bool cond3A = (cond3 && !(cond1 || cond2 || cond4));
            bool cond4A = (cond4 && !(cond1 || cond2 || cond3));

            bool isOneInsideOther = (cond1A || cond2A || cond3A || cond4A);

            return (isCross || isOneInsideOther);
        }

        /// <summary>
        /// Determines if the point is inside the polygon.
        /// </summary>
        /// <param name="aPolygon">Polygon vertices.</param>
        /// <param name="aPoint">Target point.</param>
        /// <param name="aRayLength">Tracing length.</param>
        /// <returns>ture : inside.</returns>
        public static bool IsPointInPolygon(ref Vector2[] aPolygon, Vector2 aPoint, int aRayLength = 1024)
        {
            var first = Vector2.zero;
            var second = Vector2.zero;

            var end = new Vector2(aRayLength, aPoint.y);

            int count = 0;

            if (aPolygon.Length == 0)
                return false;

            if (aPolygon.Length == 1)
                return (Mathf.Abs(aPolygon[0].x - aPoint.x) <= 0.00001f && Mathf.Abs(aPolygon[0].y - aPoint.y) <= 0.00001f);

            if (aPolygon.Length == 2)
            {
                first.x = aPolygon[0].x;
                first.y = aPolygon[0].y;
                second.x = aPolygon[1].x;
                second.y = aPolygon[1].y;
                return IsPointOnEdge(first, second, aPoint);
            }

            for (int i = 1, n = aPolygon.Length; i <= n; i++)
            {
                if (i < n)
                {
                    first.x = aPolygon[i - 1].x;
                    first.y = aPolygon[i - 1].y;
                    second.x = aPolygon[i].x;
                    second.y = aPolygon[i].y;
                }
                else
                {
                    second.x = aPolygon[0].x;
                    second.y = aPolygon[0].y;
                    first.x = aPolygon[aPolygon.Length - 1].x;
                    first.y = aPolygon[aPolygon.Length - 1].y;
                }

                if (IsPointOnEdge(first, second, aPoint))
                {
                    return true;
                }

                if (IsEdgeCross(first, second, aPoint, end))
                {
                    if ((first.y > second.y && second.y < aPoint.y) ||
                        (first.y < second.y && first.y < aPoint.y))
                    {
                        count++;
                    }
                }
            }

            if (count % 2 > 0)
            {
                return true;
            }
            return false;
        }

        public static bool IsPointInConvexPolygon(ref Vector2[] aPolygon, Vector2 aPoint)
        {
            int n = aPolygon.Length;
            bool result = false;
            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                if (((aPolygon[i].y > aPoint.y) != (aPolygon[j].y > aPoint.y)) &&
                    (aPoint.x < (aPolygon[j].x - aPolygon[i].x) * (aPoint.y - aPolygon[i].y) / (aPolygon[j].y - aPolygon[i].y) + aPolygon[i].x))
                {
                    result = !result;
                }
            }
            return result;
        }

        public static Vector2 HalfPointOfEdge(Vector2 aA, Vector2 aB)
        {
            return new Vector2((aA.x + aB.x) * 0.5f, (aA.y + aB.y) * 0.5f);
        }

        public static bool LinesCross(Vector2 aA, Vector2 aB, Vector2 aC, Vector2 aD, bool aIgnoreEqualPoints = false)
        {
            if (aIgnoreEqualPoints &&
                (GoapMath.Equal(aA, aC) || GoapMath.Equal(aA, aD) ||
                GoapMath.Equal(aB, aC) || GoapMath.Equal(aB, aD)))
            {
                return false;
            }

            float d = (aD.y - aC.y) * (aB.x - aA.x) - (aD.x - aC.x) * (aB.y - aA.y);

            if (Equal(d, 0.0f)) return false;

            float na = (aD.x - aC.x) * (aA.y - aC.y) - (aD.y - aC.y) * (aA.x - aC.x);
            float nb = (aB.x - aA.x) * (aA.y - aC.y) - (aB.y - aA.y) * (aA.x - aC.x);
            float ua = na / d;
            float ub = nb / d;

            return (ua >= 0.0f && ua <= 1.0f && ub >= 0.0f && ub <= 1.0f);
        }

        public static bool LinesCross(Vector2 aA, Vector2 aB, Vector2 aC, Vector2 aD, out Vector2 aCrossPoint)
        {
            aCrossPoint = Vector2.zero;
            bool isIntersect = false;
            float d = (aD.y - aC.y) * (aB.x - aA.x) - (aD.x - aC.x) * (aB.y - aA.y);

            if (Equal(d, 0.0f)) return isIntersect;

            float na = (aD.x - aC.x) * (aA.y - aC.y) - (aD.y - aC.y) * (aA.x - aC.x);
            float nb = (aB.x - aA.x) * (aA.y - aC.y) - (aB.y - aA.y) * (aA.x - aC.x);
            float ua = na / d;
            float ub = nb / d;

            if (ua >= 0.0f && ua <= 1.0f && ub >= 0.0f && ub <= 1.0f)
            {
                aCrossPoint.x = aA.x + (ua * (aB.x - aA.x));
                aCrossPoint.y = aA.y + (ua * (aB.y - aA.y));
                isIntersect = true;
            }

            return isIntersect;
        }

        public static bool Equal(float aValueA, float aValueB, float aDiff = 0.00001f)
        {
            return (Mathf.Abs(aValueA - aValueB) <= aDiff);
        }

        public static Vector2 GetNearestPointFromSegment(Vector2 aPoint, Vector2 aA, Vector2 aB)
        {
            return (GoapMath.Distance(aPoint, aA) < GoapMath.Distance(aPoint, aB)) ? aA : aB;
        }

        public static void ExpandSegment(Vector2 aA, Vector2 aB, out Vector2 aC, out Vector2 aD, float aLenght)
        {
            var lenAB = GoapMath.Distance(aA, aB);
            aC.x = aB.x + (aB.x - aA.x) / lenAB * aLenght;
            aC.y = aB.y + (aB.y - aA.y) / lenAB * aLenght;
            aD.x = aA.x + (aA.x - aB.x) / lenAB * aLenght;
            aD.y = aA.y + (aA.y - aB.y) / lenAB * aLenght;
        }

        public static Vector2 ExpandSegment(Vector2 aA, Vector2 aB, float aLenght)
        {
            var lenAB = GoapMath.Distance(aA, aB);
            return new Vector2(
                aB.x + (aB.x - aA.x) / lenAB * aLenght,
                aB.y + (aB.y - aA.y) / lenAB * aLenght
            );
        }

    }
}
