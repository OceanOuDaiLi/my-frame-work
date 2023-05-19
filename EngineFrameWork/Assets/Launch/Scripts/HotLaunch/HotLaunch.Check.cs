using UnityEngine;
using FrameWork.Launch.Utils;

namespace FrameWork.Launch
{
    public partial class HotLaunch
    {
        // Check Version.
        async ETTask<bool> CheckVersionPass()
        {
            LogProgress("Asset Update Checking ... ");

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
            LogProgress($"LocalVerCode : {_localVerCode}");
            LogProgress($"ServerVerCode : {_serverVerCode}");
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
            LogProgress("_serverVerURL. " + _serverVerURL);
            await UnityWebRequestGet(_serverVerURL, (data) =>
            {
                _serverVer = IOHelper.Ini.Load(data);
                LogProgress("Get Server Version Successed. - 1");
            });
        }

        // Check Decompress
        bool CheckDecompress()
        {
            bool result = true;

            if (!_assetReleaseDir.File(KEY_FILE).Exists || !_assetReleaseDir.File(AOT_FILE).Exists
                || !_assetReleaseDir.File(HOSTS_FILE).Exists || !_assetReleaseDir.File(HOT_FIX_FILE).Exists
                || !_assetReleaseDir.File(UPDATE_FILE).Exists)
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
    }
}