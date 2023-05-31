using UnityEngine;

/// <summary>
/// Todo:  ��־�ļ�д��
/// CS -> Share ģ�飬��־�����
/// </summary>
public class SDebug
{
    /// <summary>
    /// Unity ������־���
    /// </summary>
    public static bool EnableLog;

    public static void Info(object message)
    {
        if (EnableLog)
        {
            Info(message, null);
        }
    }

    public static void Info(object message, UnityEngine.Object context)
    {
        if (EnableLog)
        {
            Debug.Log(message, context);
        }
    }

    public static void LogError(object message)
    {
        if (EnableLog)
        {
            LogError(message, null);
        }
    }

    public static void LogError(object message, UnityEngine.Object context)
    {
        if (EnableLog)
        {
            Debug.LogError(message, context);
        }
    }

    public static void LogWarning(object message)
    {
        if (EnableLog)
        {
            LogWarning(message, null);
        }
    }

    public static void LogWarning(object message, UnityEngine.Object context)
    {
        if (EnableLog)
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
}