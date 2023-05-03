using System.IO;

namespace FrameWork.Launch
{
    public partial class HotLaunch
    {
        void Init()
        {
            // init local disk.
            _streamingDisk = IOHelper.StreamingDisk;
            _codeDisk = IsCodeCrypt ? IOHelper.CodeCryptDisk : IOHelper.AssetDisk;
            _assetDisk = IsCodeCrypt ? IOHelper.AssetCryptDisk : IOHelper.AssetDisk;
            // init local directory.
            _codeReleaseDir = _codeDisk.Directory(IOHelper.PlatformToName());
            _assetReleaseDir = _assetDisk.Directory(IOHelper.PlatformToName());
            _streamingReleaseDir = _streamingDisk.Directory(IOHelper.PlatformToName());

            if (!_assetReleaseDir.Exists())
            {
                _assetReleaseDir.Create();
            }

            // init local files.
            _aotFile = _assetReleaseDir.File(AOT_FILE);
            _keyFile = _assetReleaseDir.File(KEY_FILE);
            _hostsFile = _assetReleaseDir.File(HOSTS_FILE);
            _hotFixFile = _assetReleaseDir.File(HOT_FIX_FILE);
            _updateFile = _assetReleaseDir.File(UPDATE_FILE);
            _versionFile = _assetReleaseDir.File(VERSION_FILE);
        }
    }
}