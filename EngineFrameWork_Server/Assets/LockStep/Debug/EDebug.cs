using UnityEngine;

/// <summary>
/// Todo:  日志文件写入
/// </summary>
public class EDebug
{
    #region Engine Log

    /// <summary>
    /// Unity 工程日志输出
    /// </summary>
    public static bool EnableLog = true;
    public static bool EnableError;
    public static bool EnableWarning;
    public static bool EnableZeusLogStartUp;

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
}