using Core.IO;
using Core.INI;
using UnityEngine;
namespace FrameWork.Launch
{
    public class IOHelper
    {
        private static IO _io;
        public static IO IO
        {
            get
            {
                return _io ?? (_io = new IO());
            }
        }

        private static LocalDisk _codeCryptDisk;
        public static LocalDisk CodeCryptDisk
        {
            get
            {
                return _codeCryptDisk ?? (_codeCryptDisk = new LocalDisk(Application.persistentDataPath, TEACrypt));
            }
        }

        private static LocalDisk _assetCryptDisk;
        public static LocalDisk AssetCryptDisk
        {
            get
            {
                return _assetCryptDisk ?? (_assetCryptDisk = new LocalDisk(Application.persistentDataPath, AESCrypt));
            }
        }

        private static LocalDisk _assetDisk;
        public static LocalDisk AssetDisk
        {
            get
            {
                return _assetDisk ?? (_assetDisk = new LocalDisk(Application.persistentDataPath));
            }
        }

        private static LocalDisk _streamingDisk;
        public static LocalDisk StreamingDisk
        {
            get
            {
                return _streamingDisk ?? (_streamingDisk = new LocalDisk(Application.streamingAssetsPath));
            }
        }

        /// <summary>
        /// AES256 
        /// Files
        /// De/Encrypter
        /// </summary>
        private static IOAESCrypt _aesCrypt;
        public static IOAESCrypt AESCrypt
        {
            get
            {
                return _aesCrypt ?? (_aesCrypt = new IOAESCrypt("oioVWaFDx/8tHx1pdESMvKnqIGS65NecNPHQs7OYX+8=", "YxTRe5eV2/vP2ggJ2stXwbvfhBdQcLmLMZF2Y9/PsCI="));
            }
        }

        /// <summary>
        /// XXTEA 
        /// Files
        /// De/Encrypter
        /// </summary>
        private static IOTEACrypt _teaCrypt;
        public static IOTEACrypt TEACrypt
        {
            get
            {
                return _teaCrypt ?? (_teaCrypt = new IOTEACrypt("oioVWaFDx/8tHx1pdESMvKnqIGS65NecNPHQs7OYX+8="));
            }
            set
            {
                _teaCrypt = value;
            }
        }

        private static IniLoader _ini;
        public static IniLoader Ini
        {
            get
            {
                return _ini ?? (_ini = new IniLoader());
            }
        }


        public static string PlatformToName(RuntimePlatform? platform = null)
        {
            if (platform == null)
            {
                platform = Application.platform;
            }
            switch (platform)
            {
                case RuntimePlatform.LinuxPlayer:
                    return "Linux";
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                //return "Win";
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "IOS";
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    return "OSX";
                default:
                    Debug.LogError("Undefined Platform , [File Helper] = > PlatformToName");
                    return "Android";
            }
        }

        private static UpdateFileStore _updateFileStore;
        public static UpdateFileStore UpdateFileStore
        {
            get
            {
                return _updateFileStore ?? (_updateFileStore = new UpdateFileStore());
            }
        }

    }
}