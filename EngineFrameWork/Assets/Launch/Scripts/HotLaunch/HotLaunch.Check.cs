using UnityEngine;
using FrameWork.Launch.Utils;

namespace FrameWork.Launch
{
    public partial class HotLaunch
    {
        // Check Version.
        #region Check Update Version.

        async ETTask<bool> CheckVersionPass()
        {
            LogProgress("Asset Update Checking ... ");

            // Get Local Version
            _hasLocalVer = await GetLocalVersion();

            if (!_hasLocalVer)
            {
                return false;
            }

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

            // Get Server Version
            string _serverVerURL = string.Format($"{cdnHosts}/{IOHelper.PlatformToName()}/{VERSION_FILE}");
            await GetServerVersion(_serverVerURL);

            _serverVerCode = _serverVer.Get("Version", "VersionCode");
            _localVerCode = _localVer.Get("Version", "VersionCode");

            LogProgress($"Version Got! [ LocalVersion : {_localVerCode} ] & [ServerVersion : {_serverVerCode}]");

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
            });

            await tTask;

            return true;
        }

        async ETTask GetServerVersion(string _serverVerURL)
        {
            await UnityWebRequestGet(_serverVerURL, (data) =>
            {
                _serverVer = IOHelper.Ini.Load(data);
            });
        }

        #endregion

        #region Check First Pkg Decompress

        bool CheckDecompress()
        {
            bool result = true;

            if (!_assetReleaseDir.File(KEY_FILE).Exists || !_assetReleaseDir.File(AOT_FILE).Exists
                || !_assetReleaseDir.File(HOSTS_FILE).Exists || !_assetReleaseDir.File(HOT_FIX_FILE).Exists
                || !_assetReleaseDir.File(UPDATE_FILE).Exists)
            {
                result = false;
            }

            // todo.整包更新时.删除持久化文件。重新解压。

            return result;
        }

        #endregion
    }
}