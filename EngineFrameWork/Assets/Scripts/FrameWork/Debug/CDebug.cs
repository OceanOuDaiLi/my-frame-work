using FrameWork.Launch;
using UnityEngine;

/// <summary>
/// Todo:  日志文件写入
/// Unity  客户端工程日志输出
/// </summary>
public class CDebug
{
    public static bool EnableLog = true;
    public static bool EnableError;
    public static bool EnableWarning;

    public static void Log(object message)
    {
        if (EnableLog)
        {
            Log(message, null);
        }
    }

    public static void Log(object message, UnityEngine.Object context)
    {
        if (EnableLog)
        {
            Debug.Log(message, context);
        }
    }

    public static void LogError(object message)
    {
        if (EnableLog || EnableError)
        {
            LogError(message, null);
        }
    }

    public static void LogError(object message, UnityEngine.Object context)
    {
        if (EnableLog || EnableError)
        {
            Debug.LogError(message, context);
        }
    }

    public static void LogWarning(object message)
    {
        if (EnableLog || EnableWarning)
        {
            LogWarning(message, null);
        }
    }

    public static void LogWarning(object message, UnityEngine.Object context)
    {
        if (EnableLog || EnableWarning)
        {
            Debug.LogWarning(message, context);
        }
    }

    public static void LogFormat(string format, params object[] args)
    {
        if (EnableLog)
        {
            Debug.LogFormat(format, args);
        }
    }

    public static void LogFormat(UnityEngine.Object context, string format, params object[] args)
    {
        if (EnableLog)
        {
            Debug.LogFormat(context, format, args);
        }
    }

    public static void LogFormat(LogType logType, LogOption logOptions, UnityEngine.Object context, string format, params object[] args)
    {
        if (EnableLog)
        {
            Debug.LogFormat(logType, logOptions, context, format, args);
        }
    }

    public static void Assert(bool condition)
    {
        if (EnableLog)
        {
            Debug.Assert(condition);
        }
    }

    public static void LogProgress(string txt)
    {
        string tips = "{0} {1} {2}";
#if UNITY_EDITOR
        tips = string.Format(tips, "<color=#1FD4BB>", txt, "</color>");
#else
        tips = string.Format(tips, "", txt, "");
#endif

        Log(tips, null);
    }

    //#region FightEngine Log
    ///// <summary>
    ///// 战斗 -> 日志输出
    ///// </summary>
    //public static bool EnableFightLog;
    //public static bool FightError;
    //public static bool FightWarning;
    //public static bool BTreeStartUp;

    //public static void FLog(object message)
    //{
    //    if (EnableFightLog)
    //    {
    //        FLog(message, null);
    //    }
    //}

    //public static void FLog(object message, UnityEngine.Object context)
    //{
    //    if (EnableFightLog)
    //    {
    //        Debug.Log(message, context);
    //    }
    //}

    //public static void FError(object message)
    //{
    //    if (EnableFightLog || FightError)
    //    {
    //        FError(message, null);
    //    }
    //}

    //public static void FError(object message, UnityEngine.Object context)
    //{
    //    if (EnableFightLog || FightError)
    //    {
    //        Debug.LogError(message, context);
    //    }
    //}

    //public static void BTLogWarning(object message)
    //{
    //    if (EnableFightLog || FightWarning)
    //    {
    //        BTLogWarning(message, null);
    //    }
    //}

    //public static void BTLogWarning(object message, UnityEngine.Object context)
    //{
    //    if (EnableFightLog || FightWarning)
    //    {
    //        Debug.LogWarning(message, context);
    //    }
    //}

    //public static void BTLogFormat(string format, params object[] args)
    //{
    //    if (EnableFightLog)
    //    {
    //        Debug.LogFormat(format, args);
    //    }
    //}

    //public static void BTLogFormat(UnityEngine.Object context, string format, params object[] args)
    //{
    //    if (EnableFightLog)
    //    {
    //        Debug.LogFormat(context, format, args);
    //    }
    //}

    //public static void BTLogFormat(LogType logType, LogOption logOptions, UnityEngine.Object context, string format, params object[] args)
    //{
    //    if (EnableFightLog)
    //    {
    //        Debug.LogFormat(logType, logOptions, context, format, args);
    //    }
    //}

    //public static void BTLogStartUp(string title, string txt)
    //{
    //    if (EnableFightLog || BTreeStartUp)
    //    {
    //        Debug.LogFormat($"<color=#FFC400> {title}： {txt}</color>");
    //    }
    //}
    //#endregion
}