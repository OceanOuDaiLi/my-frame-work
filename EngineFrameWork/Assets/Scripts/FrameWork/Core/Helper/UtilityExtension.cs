using System;
using System.Text;
using UnityEngine;

public static class UtilityExtension
{
    /// <summary>
    /// 获取系统当前时间
    /// </summary>
    /// <returns></returns>
    public static string GetYearMonthDayHour()
    {
        return string.Format($"{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}_{DateTime.Now.Hour}_{DateTime.Now.Minute}");
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

    public static string VersionAdd(string oldVersio)
    {
        float newVer = float.Parse(oldVersio);
        newVer++;

        return string.Format("{0}.0", newVer);
    }
}
