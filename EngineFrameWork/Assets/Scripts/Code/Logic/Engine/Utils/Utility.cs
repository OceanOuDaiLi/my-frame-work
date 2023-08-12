using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class Utility
{

    public static T FindMaximumValue<T>(this List<T> list, Func<T, float> funcValue, Predicate<T> filter)
    {
        float max = float.NegativeInfinity;
        T result = default(T);
        foreach (T e in list)
        {
            if (filter != null && !filter(e)) continue;

            float tmp = funcValue(e);
            if (tmp > max)
            {
                result = e;
                max = tmp;
            }
        }
        return result;
    }

    public static T FindMinimalValue<T>(this List<T> list, Func<T, float> funcValue, Predicate<T> filter)
    {
        float min = float.PositiveInfinity;
        T result = default(T);
        foreach (T e in list)
        {
            if (!filter(e)) continue;

            float tmp = funcValue(e);
            if (tmp < min)
            {
                result = e;
                min = tmp;
            }
        }
        return result;
    }

    public static void SetLayerRecursively(this GameObject gameObject, int newLayer)
    {
        gameObject.layer = newLayer;
        foreach (Transform t in gameObject.transform)
        {
            SetLayerRecursively(t.gameObject, newLayer);
        }
    }

    public static Transform FindRecursively(this Transform transform, string name)
    {
        Transform result = transform.Find(name);
        if (result != null) return result;

        foreach (Transform t in transform)
        {
            result = FindRecursively(t, name);
            if (result != null) return result;
        }
        return null;
    }

    public static void FindAllContainsRecursively(this Transform transform, string name, ref List<Transform> ls)
    {
        foreach (Transform t in transform)
        {
            if (t.name.Contains(name)) ls.Add(t);
            FindAllContainsRecursively(t, name, ref ls);
        }
    }

    public static void SetPositionX(this Transform transform, float x)
    {
        Vector3 pos = transform.position;
        pos.x = x;
        transform.position = pos;
    }

    public static void SetPositionY(this Transform transform, float y)
    {
        Vector3 pos = transform.position;
        pos.y = y;
        transform.position = pos;
    }

    public static void SetPositionZ(this Transform transform, float z)
    {
        Vector3 pos = transform.position;
        pos.z = z;
        transform.position = pos;
    }

    public static void AddPositionX(this Transform transform, float x)
    {
        Vector3 pos = transform.position;
        pos.x += x;
        transform.position = pos;
    }

    public static void AddPositionY(this Transform transform, float y)
    {
        Vector3 pos = transform.position;
        pos.y += y;
        transform.position = pos;
    }

    public static void AddPositionZ(this Transform transform, float z)
    {
        Vector3 pos = transform.position;
        pos.z += z;
        transform.position = pos;
    }

    public static void SetAnchoredPositionX(this RectTransform transform, float x)
    {
        Vector3 pos = transform.anchoredPosition;
        pos.x = x;
        transform.anchoredPosition = pos;
    }

    public static void SetAnchoredPositionY(this RectTransform transform, float y)
    {
        Vector3 pos = transform.anchoredPosition;
        pos.y = y;
        transform.anchoredPosition = pos;
    }

    public static void SetAnchoredPositionZ(this RectTransform transform, float z)
    {
        Vector3 pos = transform.anchoredPosition;
        pos.z = z;
        transform.anchoredPosition = pos;
    }

    public static void AddAnchoredPositionX(this RectTransform transform, float x)
    {
        Vector3 pos = transform.anchoredPosition;
        pos.x += x;
        transform.anchoredPosition = pos;
    }

    public static void AddAnchoredPositionY(this RectTransform transform, float y)
    {
        Vector3 pos = transform.anchoredPosition;
        pos.y += y;
        transform.anchoredPosition = pos;
    }

    public static void AddAnchoredPositionZ(this RectTransform transform, float z)
    {
        Vector3 pos = transform.anchoredPosition;
        pos.z += z;
        transform.anchoredPosition = pos;
    }

    public static void AddSizeDeltaX(this RectTransform transform, float x)
    {
        Vector2 sizeDelta = transform.sizeDelta;
        sizeDelta.x += x;
        transform.sizeDelta = sizeDelta;
    }

    public static void AddSizeDeltaXWithClamp(this RectTransform transform, float x, float min, float max)
    {
        Vector2 sizeDelta = transform.sizeDelta;
        sizeDelta.x += x;
        sizeDelta.x = Mathf.Clamp(sizeDelta.x, min, max);
        transform.sizeDelta = sizeDelta;
    }

    public static void AddSizeDeltaY(this RectTransform transform, float y)
    {
        Vector2 sizeDelta = transform.sizeDelta;
        sizeDelta.y += y;
        transform.sizeDelta = sizeDelta;
    }

    public static void AddSizeDeltaYWithClamp(this RectTransform transform, float y, float min, float max)
    {
        Vector2 sizeDelta = transform.sizeDelta;
        sizeDelta.y += y;
        sizeDelta.y = Mathf.Clamp(sizeDelta.y, min, max);
        transform.sizeDelta = sizeDelta;
    }

    public static void SetAnchorMinX(this RectTransform transform, float x)
    {
        Vector2 anchorMin = transform.anchorMin;
        anchorMin.x = x;
        transform.anchorMin = anchorMin;
    }

    public static void SetAnchorMinY(this RectTransform transform, float y)
    {
        Vector2 anchorMin = transform.anchorMin;
        anchorMin.y = y;
        transform.anchorMin = anchorMin;
    }

    public static void SetAnchorMaxX(this RectTransform transform, float x)
    {
        Vector2 anchorMax = transform.anchorMax;
        anchorMax.x = x;
        transform.anchorMax = anchorMax;
    }

    public static void SetAnchorMaxY(this RectTransform transform, float y)
    {
        Vector2 anchorMax = transform.anchorMax;
        anchorMax.y = y;
        transform.anchorMax = anchorMax;
    }

    public static void AddAnchorMinX(this RectTransform transform, float x)
    {
        Vector2 anchorMin = transform.anchorMin;
        anchorMin.x += x;
        transform.anchorMin = anchorMin;
    }

    public static void AddAnchorMinY(this RectTransform transform, float y)
    {
        Vector2 anchorMin = transform.anchorMin;
        anchorMin.y += y;
        transform.anchorMin = anchorMin;
    }

    public static void AddAnchorMaxX(this RectTransform transform, float x)
    {
        Vector2 anchorMax = transform.anchorMax;
        anchorMax.x += x;
        transform.anchorMax = anchorMax;
    }

    public static void AddAnchorMaxY(this RectTransform transform, float y)
    {
        Vector2 anchorMax = transform.anchorMax;
        anchorMax.y += y;
        transform.anchorMax = anchorMax;
    }

    public static void SetX(this Vector2 vector2, float x)
    {
        Vector2 newVector = vector2;
        newVector.x = x;
        vector2 = newVector;
    }

    public static void SetY(this Vector2 vector2, float y)
    {
        Vector2 newVector = vector2;
        newVector.y = y;
        vector2 = newVector;
    }

    /// <summary>
    /// 判断浮点数是否在范围内
    /// </summary>
    /// <param name="f"></param>
    /// <param name="min">最小值，包含</param>
    /// <param name="max">最大值，不包含</param>
    /// <returns></returns>
    public static bool Between(this float f, float min, float max)
    {
        return f >= min && f < max;
    }

    /// <summary>
    /// 一维轴的移动
    /// </summary>
    /// <param name="f"></param>
    /// <param name="target">移动目标位置</param>
    /// <param name="speed">移动速度</param>
    /// <param name="time">移动时间</param>
    /// <returns></returns>
    public static float Move(this float f, float target, float speed, float time)
    {
        float dist = target - f;
        float moveDelta = 0;
        if (dist > 0) moveDelta = Mathf.Clamp(speed * time, 0, dist);
        else moveDelta = Mathf.Clamp(-speed * time, dist, 0);

        return f + moveDelta;
    }

    /// <summary>  
    /// Unix时间戳转为C#格式时间  
    /// </summary>  
    /// <param name="timeStamp">Unix时间戳格式,例如1482115779</param>  
    /// <returns>C#格式时间</returns>
    public static DateTime GetDateTime(this double timeStamp)
    {
        DateTime dtStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dtStart = dtStart.AddSeconds(timeStamp).ToLocalTime();
        return dtStart;
    }

    /// <summary>  
    /// DateTime时间格式转换为Unix时间戳格式  
    /// </summary>
    /// <returns>Unix时间戳格式</returns>  
    public static double GetTimeStamp(this DateTime time)
    {
        return (TimeZoneInfo.ConvertTimeToUtc(time) - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
    }

    public static string ToString(this byte[] bytes)
    {
        return Encoding.UTF8.GetString(bytes);
    }

    public static byte[] ToBytes(this string str)
    {
        return Encoding.UTF8.GetBytes(str);
    }

    public static float ClampAngle(this float angle, float min, float max)
    {
        angle = Mathf.Repeat(angle, 360);
        min = Mathf.Repeat(min, 360);
        max = Mathf.Repeat(max, 360);
        bool inverse = false;
        var tmin = min;
        var tangle = angle;
        if (min > 180)
        {
            inverse = !inverse;
            tmin -= 180;
        }
        if (angle > 180)
        {
            inverse = !inverse;
            tangle -= 180;
        }
        var result = !inverse ? tangle > tmin : tangle < tmin;
        if (!result)
            angle = min;

        inverse = false;
        tangle = angle;
        var tmax = max;
        if (angle > 180)
        {
            inverse = !inverse;
            tangle -= 180;
        }
        if (max > 180)
        {
            inverse = !inverse;
            tmax -= 180;
        }

        result = !inverse ? tangle < tmax : tangle > tmax;
        if (!result)
            angle = max;
        return angle;
    }

    /// <summary>
    /// 比较两个版本的大小，当this的版本号大于other时返回1，相等返回0，小于返回-1
    /// </summary>
    /// <param name="thisVersion">this的版本号</param>
    /// <param name="otherVersion">other的版本号</param>
    /// <returns>当this的版本号大于other时返回1，相等返回0，小于返回-1</returns>
    public static int VersionCompare(this string thisVersion, string otherVersion)
    {
        int result = 0;

        string[] versionThisAry = thisVersion.Split('.');
        string[] versionOtherAry = otherVersion.Split('.');

        int aryLength = Mathf.Max(versionOtherAry.Length, versionThisAry.Length);
        int v1 = 0;
        int v2 = 0;
        for (int i = 0; i < aryLength; i++)
        {
            v1 = i >= versionOtherAry.Length ? 0 : (string.IsNullOrEmpty(versionOtherAry[i]) ? 0 : int.Parse(versionOtherAry[i]));
            v2 = i >= versionThisAry.Length ? 0 : (string.IsNullOrEmpty(versionThisAry[i]) ? 0 : int.Parse(versionThisAry[i]));
            if (v2 > v1)
            {
                result = 1;
                break;
            }
            else if (v2 < v1)
            {
                result = -1;
                break;
            }
        }

        return result;
    }
}
