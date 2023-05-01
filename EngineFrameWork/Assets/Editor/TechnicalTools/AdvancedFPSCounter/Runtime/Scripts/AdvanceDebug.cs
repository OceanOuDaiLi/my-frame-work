using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using CodeStage.AdvancedFPSCounter;
using CodeStage.AdvancedFPSCounter.CountersData;

public class AdvanceDebug : MonoBehaviour
{
    [SerializeField] AFPSCounter counter;
    [SerializeField] Text fpsTxt;

    [SerializeField] Text logTxt;

    public void OnClickFPSCounter()
    {
        if (counter.enabled)
        {
            fpsTxt.text = "Show Debug";
        }
        else
        {
            fpsTxt.text = "Dispare Debug";
        }
        counter.enabled = !counter.enabled;
    }

    bool showLog = false;
    public void OnClickLog()
    {
        if (showLog)
        {
            logTxt.text = "Enable Log";
        }
        else
        {
            logTxt.text = "Disable Log";
        }
        showLog = !showLog;
        Debug.unityLogger.logEnabled = showLog;
    }

    /*
     * 若使用Lua，使用反射调用
     * 
    public void OnClickLuaGC()
    {
        System.Reflection.Assembly cSharp = null;
        cSharp = AppDomain.CurrentDomain.GetAssemblies().First(assembly => assembly.GetName().Name == "Assembly-CSharp");
        System.Type appType = cSharp.GetType("FTX.Application.Main");

        var mainMethod = appType.GetMethod("TestLuaGC");
        if (mainMethod == null)
        {
            UnityEngine.Debug.LogError($"[Main::TestLuaGC] Func is null");
        }
        mainMethod.Invoke(null, null);
    }
    */
    public void OnClickCSharpGC()
    {
        System.GC.Collect();
    }

    public void OnClickLuaAndCSharpGC()
    {
        var last = AFPSCounter.Instance.memoryCounter.LastMonoValue;
        Debug.unityLogger.logEnabled = true;
        Debug.Log("### CSharp GC ###");
        //OnClickLuaGC();
        OnClickCSharpGC();
        Resources.UnloadUnusedAssets();
        Debug.unityLogger.logEnabled = false;
        StartCoroutine(SubMonoValue(last));
    }

    IEnumerator SubMonoValue(long lastValue)
    {
        yield return new WaitForSeconds(2);

        var subMono = Math.Floor(Math.Abs((AFPSCounter.Instance.memoryCounter.LastMonoValue - lastValue) / (float)MemoryCounterData.MemoryDivider));
        Debug.unityLogger.logEnabled = true;
        Debug.Log("## Total Sub Mono: " + subMono + "/Mb");
        Debug.unityLogger.logEnabled = false;
    }
}
