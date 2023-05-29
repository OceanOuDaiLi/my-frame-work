
using System;
using UnityEngine;

public class ServerLaunch : MonoBehaviour
{
    void Start()
    {
        //#if UNITY_EDITOR
        //        System.Reflection.Assembly cSharp = null;
        //        cSharp = AppDomain.CurrentDomain.GetAssemblies().First(assembly => assembly.GetName().Name == "Assembly-CSharp");
        //        System.Type appType = cSharp.GetType("Server.Http.HttpServer");

        //        var mainMethod = appType.GetMethod("OnStart");
        //        if (mainMethod == null)
        //        {
        //            UnityEngine.Debug.LogError($"[HotLaunch::HttpServer] OnStart is null");
        //            return;
        //        }

        //        mainMethod.Invoke(null, null);
        //#endif

        string serverType = GetCommandLineArg("LaunchServerType");
    }

    string GetCommandLineArg(string name)
    {
        string[] args = Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name)
            {
                if (args.Length > i + 1)
                {
                    return args[i + 1];
                }
            }
        }

        return "";
    }
}
