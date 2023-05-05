using UnityEngine;

namespace Goap.Extensions
{
    public static class RectExtension
    {
        public static Rect ScaleSizeBy(this Rect aRect, float aScale)
        {
            return aRect.ScaleSizeBy(aScale, aRect.center);
        }

        public static Rect ScaleSizeBy(this Rect aRect, float aScale, Vector2 aPivotPoint)
        {
            var result = aRect;

            result.x -= aPivotPoint.x;
            result.y -= aPivotPoint.y;

            result.xMin *= aScale;
            result.yMin *= aScale;
            result.xMax *= aScale;
            result.yMax *= aScale;

            result.x += aPivotPoint.x;
            result.y += aPivotPoint.y;

            return result;
        }
    }
}
