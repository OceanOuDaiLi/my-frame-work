
using Core.Interface.IO;
using Core.Interface.INI;
using System.Collections.Generic;

namespace FrameWork.Launch
{
    public partial class HotLaunch
    {
        #region File Config

        /// <summary>
        /// AotDlls AssetBundle.
        /// </summary>
        IFile _aotFile = null;
        const string AOT_FILE = "aotdlls";
        /// <summary>
        /// Safety detection file.
        /// </summary>
        IFile _keyFile = null;
        const string KEY_FILE = "key.ini";
        /// <summary>
        /// Net information file.
        /// </summary>
        IFile _hostsFile = null;
        const string HOSTS_FILE = "hosts.ini";
        /// <summary>
        ///  FixDlls AssetBundle.
        /// </summary>
        IFile _hotFixFile = null;
        const string HOT_FIX_FILE = "hotupdatedlls";
        /// <summary>
        /// Asset update file.
        /// </summary>
        IFile _updateFile = null;
        const string UPDATE_FILE = "update-list.ini";
        /// <summary>
        /// Local version file.
        /// </summary>
        IFile _versionFile = null;
        const string VERSION_FILE = "Version.ini";

        const string DECOMPRESS_SUCCESS = "DecompressSuccess";

        #endregion

        #region Disk 

        private IDisk _codeDisk { get; set; }
        private IDisk _assetDisk { get; set; }
        private IDisk _streamingDisk { get; set; }

        private IDirectory _codeReleaseDir { get; set; }
        private IDirectory _assetReleaseDir { get; set; }
        private IDirectory _streamingReleaseDir { get; set; }

        #endregion

        #region HotFix Variables

        bool _hasLocalVer = false;
        IIniResult _hostIni = null;
        IIniResult _localVer = null;
        IIniResult _serverVer = null;

        string _localVerCode = string.Empty;
        string _serverVerCode = string.Empty;

        DownloadData downloadData { get; set; }
        internal class DownloadData
        {
            public DownloadType downType;
            public string zipFile;
            public string updateFile;

            public enum DownloadType
            {
                TOTAL_FILE = 0,
                ZIP = 1,
                TOTAL_PACKAGE = 2,
            }
        }

        #endregion

        #region Decompress Variables

        DecompressInfo decompressInfo { get; set; }

        internal class DecompressInfo
        {
            public long TotalDecompressSize { get; set; }
            public long CurentDecompressSize { get; set; }

            /// <summary>
            /// key :   source file.
            /// value:  dest file.
            /// </summary>
            public Dictionary<IFile, byte[]> DecompressDic { get; set; }

            public DecompressInfo()
            {
                TotalDecompressSize = 0;
                DecompressDic = new Dictionary<IFile, byte[]>();
            }

            public void CollectDecompressSize(long fileSize)
            {
                TotalDecompressSize += fileSize;
            }

            public void Dispose()
            {
                DecompressDic.Clear();
                DecompressDic = null;
            }
        }

        #endregion

        #region Private Util Methods

        string GetBytesString(long bytes)
        {
            if (bytes >= 1073741824L)
            {
                return (bytes / 1073741824L) + "GB";
            }
            else if (bytes >= 1048576L)
            {
                return (bytes / 1048576L) + "MB";
            }
            else if (bytes >= 1024L)
            {
                return (bytes / 1024L) + "KB";
            }
            else
            {
                return bytes + "B";
            }
        }

        void LogProgress(string txt)
        {
            string tips = "{0} \n {1} {2}";
#if UNITY_EDITOR
            tips = string.Format(tips, "<color=#FFFF00>", txt, "</color>");
#else
            tips = string.Format(tips, "### Progress ###", txt, "");
#endif

            UnityEngine.Debug.LogFormat(tips);
        }

        void LogInfo(string txt)
        {
            string tips = "{0} \n {1} {2}";
#if UNITY_EDITOR
            tips = string.Format(tips, "<color=#7BE578>", txt, "</color>");
#else
            tips = string.Format(tips, "### Info ###", txt, "");
#endif

            UnityEngine.Debug.LogFormat(tips);
        }

        void LogError(string txt)
        {
            string tips = "{0} \n {1} {2}";
#if UNITY_EDITOR
            tips = string.Format(tips, "<color=#F80000>", txt, "</color>");
#else
            tips = string.Format(tips, "### Error ###", txt, "");
#endif

            UnityEngine.Debug.LogFormat(tips);

        }

        #endregion
    }
}