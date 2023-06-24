using System.IO;
using FrameWork.Launch.Utils;

namespace FrameWork.Launch
{
    public partial class HotLaunch
    {
        // Check Version.
        #region Check Update Version.

        async ETTask<bool> CheckHotFixVersion()
        {
            LogProgress("Asset Update Checking ... ");

            // Asyns Read HostsFile. 
            await GetLocalHostFile();

            // Get Server Version
            hostsUrl = string.Format($"{cdnHosts}{Path.AltDirectorySeparatorChar}{IOHelper.PlatformToName()}");
            await GetServerVersion(string.Format($"{hostsUrl}{Path.AltDirectorySeparatorChar}{VERSION_FILE}"));

            // Get Local Version
            _hasLocalVer = await GetLocalVersion();
            if (!_hasLocalVer)
            {
                return false;
            }

            bool result = Utility.CompareVersion(_localVerCode, _serverVerCode) >= 0;
            LogProgress($"Version Got! [ LocalVersion : {_localVerCode} ] & [ServerVersion : {_serverVerCode}] || NeedHotFix: {!result}");

            return result;
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
            });

            await tTask;
            tTask = null;

            return true;
        }

        async ETTask GetLocalHostFile()
        {
            ETTask tTask = ETTask.Create(true);
            _hostsFile.ReadAsync((data) =>
            {
                _hostIni = IOHelper.Ini.Load(data);
                cdnHosts = _hostIni.Get("Hosts", "CdnUrl");

                tTask.SetResult();
            });
            await tTask;
            tTask = null;
        }

        async ETTask GetServerVersion(string _serverVerURL)
        {
            await UnityWebRequestGet(_serverVerURL, (data) =>
            {
                _serverVer = IOHelper.Ini.Load(data);
                _serverVerCode = _serverVer.Get("Version", "VersionCode");
                _localVerCode = _localVer.Get("Version", "VersionCode");
            });
        }

        #endregion

        #region Check First Pkg Decompress

        bool CheckDecompress()
        {
            bool result = true;

            if (!_assetReleaseDir.File(KEY_FILE).Exists || !_assetReleaseDir.File(AOT_FILE).Exists
                || !_assetReleaseDir.File(HOSTS_FILE).Exists || !_assetReleaseDir.File(HOT_FIX_FILE).Exists
                || !_assetReleaseDir.File(UPDATE_FILE).Exists || !_assetReleaseDir.File(VERSION_FILE).Exists)
            {
                result = false;
            }

            // mark.如需要整包更新时(整包构建时).删除持久化文件,重新解压.

            return result;
        }

        #endregion
    }
}