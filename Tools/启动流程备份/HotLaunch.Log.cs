using UnityEngine;

/// <summary>
/// Todo: 日志文件写入
/// </summary>
namespace FrameWork.Launch
{
    public partial class HotLaunch
    {
        bool enableLog = false;

        void LogProgress(string txt) 
        {
            if (enableLog)
                Debug.LogFormat($"<color=#FFFF00> {txt} </color>");
        }

        void LogInfo(string txt)
        {
            if (enableLog)
                Debug.LogFormat($"<color=#7BE578> {txt} </color>");
        }


        void LogError(string txt) 
        {
            if (enableLog)
                Debug.LogFormat($"<color=#F80000> {txt} </color>");
        }
    }
}