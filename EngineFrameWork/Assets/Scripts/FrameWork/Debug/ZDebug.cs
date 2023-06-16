using UnityEngine;

/// <summary>
/// Todo:  日志文件写入
/// </summary>
public class ZDebug
{
    #region CSProject Log

    /// <summary>
    /// Unity 工程日志输出
    /// </summary>
    public static bool EnableLog;
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
    #endregion

    #region BehaviourTree Log
    /// <summary>
    /// 战斗 -> 日志输出
    /// </summary>
    public static bool EnableBTreeLog;
    public static bool BTreeError;
    public static bool BTreeWarning;
    public static bool BTreeStartUp;

    public static void BTLog(object message)
    {
        if (EnableBTreeLog)
        {
            BTLog(message, null);
        }
    }

    public static void BTLog(object message, UnityEngine.Object context)
    {
        if (EnableBTreeLog)
        {
            Debug.Log(message, context);
        }
    }

    public static void BTLogError(object message)
    {
        if (EnableBTreeLog || BTreeError)
        {
            BTLogError(message, null);
        }
    }

    public static void BTLogError(object message, UnityEngine.Object context)
    {
        if (EnableBTreeLog || BTreeError)
        {
            Debug.LogError(message, context);
        }
    }

    public static void BTLogWarning(object message)
    {
        if (EnableBTreeLog || BTreeWarning)
        {
            BTLogWarning(message, null);
        }
    }

    public static void BTLogWarning(object message, UnityEngine.Object context)
    {
        if (EnableBTreeLog || BTreeWarning)
        {
            Debug.LogWarning(message, context);
        }
    }

    public static void BTLogFormat(string format, params object[] args)
    {
        if (EnableBTreeLog)
        {
            Debug.LogFormat(format, args);
        }
    }

    public static void BTLogFormat(UnityEngine.Object context, string format, params object[] args)
    {
        if (EnableBTreeLog)
        {
            Debug.LogFormat(context, format, args);
        }
    }

    public static void BTLogFormat(LogType logType, LogOption logOptions, UnityEngine.Object context, string format, params object[] args)
    {
        if (EnableBTreeLog)
        {
            Debug.LogFormat(logType, logOptions, context, format, args);
        }
    }

    public static void BTLogStartUp(string title, string txt)
    {
        if (EnableBTreeLog || BTreeStartUp)
        {
            Debug.LogFormat($"<color=#FFC400> {title}： {txt}</color>");
        }
    }
    #endregion
}