
using TMPro;
using UnityEngine;
using FrameWork.Launch;
using System.Collections;
using FrameWork.Launch.Utils;

public class GameStart : MonoBehaviour
{
    public bool SkipHotStart = false;
    public bool ExperienceStart = false;

    [Space(10)]
    [SerializeField] TMP_InputField Ip_InputField;
    [SerializeField] TMP_InputField Port_InputField;
    [SerializeField] TMP_InputField ServerSvn_InputField;

    [SerializeField] TMP_Text CurSvnVer_Text;
    [SerializeField] TMP_Text CurEngineVer_Text;
    [SerializeField] TMP_Text UpdteProgress_Text;

    private const string PlayerPrefsKey_CurSvn = "PlayerPrefsKey_CurSvn";

    private void Awake()
    {
        GameObject hotLaunchObj = new GameObject("HotLaunch");
        hotLaunchObj.AddComponent<HotLaunch>();

        string curSvnVer = PlayerPrefs.GetString(PlayerPrefsKey_CurSvn);
        CurSvnVer_Text.text = string.IsNullOrEmpty(curSvnVer) ? "0" : curSvnVer;

    }

    private IEnumerator Start()
    {
        while (HotLaunch.Instance == null)
        {
            yield return AotYielders.EndOfFrame;
        }
#if UNITY_EDITOR
        HotLaunch.Instance.UnityEditor = SkipHotStart;
        HotLaunch.Instance.IsExperience = ExperienceStart;
#else
        HotLaunch.Instance.UnityEditor = false;             // 体验服流程
        HotLaunch.Instance.IsExperience = true;

        // HotLaunch.Instance.UnityEditor = false;          // 正式服流程
        // HotLaunch.Instance.IsExperience = false;
        // HotLaunch.Instance.IsReleasePkg = true;        
#endif

        string enterIp = Ip_InputField.text;
        string curSvnVer = CurSvnVer_Text.text;
        string enterPort = Port_InputField.text;
        string enterSvnVer = ServerSvn_InputField.text;
        HotLaunch.Instance.InitExperience(enterIp, enterPort, curSvnVer, enterSvnVer, DownloadSuccess, CleaningSuccess);

        HotLaunch.Instance.Launch();
    }

    public void OnClickUpdate()
    {
        if (HotLaunch.Instance == null)
        {
            return;
        }
        HotLaunch.Instance.OnClickUpdate(ShowUpdateProgress);
    }

    public void OnClickEnter()
    {
        if (HotLaunch.Instance == null)
        {
            return;
        }
        HotLaunch.Instance.OnClickEnter();
    }

    public void OnClickClear()
    {
        if (HotLaunch.Instance == null)
        {
            return;
        }
        HotLaunch.Instance.OnClickClear(ShowCleaningProgress);
    }

    private void DownloadSuccess(string svnVersion)
    {
        PlayerPrefs.SetString(PlayerPrefsKey_CurSvn, svnVersion);
        PlayerPrefs.Save();
        CurSvnVer_Text.text = svnVersion;
        UpdteProgress_Text.text = $"Asset Update Successed.";
    }

    private void ShowUpdateProgress(float progress)
    {
        UpdteProgress_Text.text = $"Asset Updateing ...  {Mathf.Floor(progress * 100)} %";
    }

    private void CleaningSuccess()
    {
        UpdteProgress_Text.text = $"Asset Cleaning Successed.";
        PlayerPrefs.SetString(PlayerPrefsKey_CurSvn, "0");
        PlayerPrefs.Save();
        CurSvnVer_Text.text = "0";

        string enterIp = Ip_InputField.text;
        string curSvnVer = CurSvnVer_Text.text;
        string enterPort = Port_InputField.text;
        string enterSvnVer = ServerSvn_InputField.text;
        HotLaunch.Instance.Rest(enterIp, enterPort, curSvnVer, enterSvnVer);
    }

    private void ShowCleaningProgress(float progress)
    {
        UpdteProgress_Text.text = $"Asset Cleaninging ...  {Mathf.Floor(progress * 100)} %";
    }
}
