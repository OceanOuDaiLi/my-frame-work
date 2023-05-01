using FrameWork;
using UnityEngine;
using FrameWork.Launch.Utils;

namespace FrameWork.Launch
{
    public partial class HotLaunch
    {
        // 1.Check Decompress.
        bool CheckDecompressPass()
        {
            ShowTips("Asset Decompress Checking ... ");

            bool result = true;
            if (!_assetReleaseDir.Exists())
            {
                result = false;
            }

            if (!_assetReleaseDir.File(KEY_FILE).Exists)
            {
                result = false;
            }

            int decomSuccess = PlayerPrefs.GetInt(DECOMPRESS_SUCCESS);
            if (decomSuccess < 1)
            {
                result = false;
            }

            return result;
        }

        // 2. Check Version.
        async ETTask<bool> CheckVersionPass()
        {
            ShowTips("Asset Update Checking ... ");

            // Asyns Read HostsFile. 
            ETTask tTask = ETTask.Create(true);
            _hostsFile.ReadAsync((data) =>
            {
                _hostIni = IOHelper.Ini.Load(data);
                tTask.SetResult();
                tTask = null;
            });
            await tTask;

            string cdnHosts = _hostIni.Get("Hosts", "CdnUrl");

            // Request Get Server Version.ini
            string _serverVerURL = string.Format($"{cdnHosts}/{IOHelper.PlatformToName()}/{VERSION_FILE}");
            await GetServerVersion(_serverVerURL);
            // Read Get Local Version.ini
            _hasLocalVer = await GetLocalVersion();

            if (!_hasLocalVer)
            {
                return false;
            }

            _serverVerCode = _serverVer.Get("Version", "VersionCode");
            _localVerCode = _localVer.Get("Version", "VersionCode");
            return Utility.CompareVersion(_localVerCode, _serverVerCode) >= 0;
        }

        async ETTask<bool> GetLocalVersion()
        {
            if (!_versionFile.Exists)
            {
                return false;
            }

            ETTask tTask = ETTask.Create(true);
            _versionFile.ReadAsync((data) =>
            {
                _localVer = IOHelper.Ini.Load(data);
                tTask.SetResult();
                tTask = null;
                LogProgress("Get Local Version Successed. -2");
            });

            await tTask;

            return true;
        }

        async ETTask GetServerVersion(string _serverVerURL)
        {
            await UnityWebRequestGet(_serverVerURL, (data) =>
            {
                _serverVer = IOHelper.Ini.Load(data);
                LogProgress("Get Server Version Successed. - 1");
            });
        }
    }
}