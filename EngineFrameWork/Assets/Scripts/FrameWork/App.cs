using Core;
using Core.IO;
using Core.INI;
using Core.Hash;
using Core.Json;
using Core.Time;
using Core.Crypt;
using Core.Network;
using Core.Resources;
using Core.Interface;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com

	Created:	2018 ~ 2023
	Filename: 	App.cs
	Author:		DaiLi.Ou

	Descriptions: Manager Application Life Time.
*********************************************************************/
namespace FrameWork
{
    public class App
    {
        private static Core.Application _instance;
        public static Core.Application Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }
                return null;
            }
            internal set
            {
                _instance = value;
            }
        }

        #region Utility Classes
        private static Env _env;
        public static Env Env
        {
            get
            {
                if (_env == null)
                {
                    _env = new Env();
                    _env.SetDebugLevel(DebugLevels.Auto);
                    //#warning 配置环境目录
#if UNITY_EDITOR
                    _env.SetResourcesBuildPath("ABAssets/AssetBundle");
                    _env.SetResourcesNoBuildPath("ABAssets/NotAssetBundle");
                    _env.SetReleasePath("StreamingAssets");
                    _env.IsAssetCrypt = false;
#endif
                }
                return _env;
            }
        }

        /// <summary>
        /// AES256 
        /// Tcp Service
        /// Request/Response 
        /// De/Encrypter
        /// </summary>
        private static NetCrypt _crypt;
        public static NetCrypt Crypt
        {
            get
            {
                if (_crypt == null)
                {
                    _crypt = new NetCrypt();
                    _crypt.SetAdapter(new HMacAes256());
                }
                return _crypt;
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

        private static Network _net;
        public static Network Net
        {
            get
            {
                if (_net == null)
                {
                    _net = new Network();
                }
                return _net;
            }
        }

        private static Json _json;
        public static Json Json
        {
            get
            {
                return _json ?? (_json = new Json(new TinyJsonAdapter()));
            }
        }

        private static TimeSystem _time;
        public static TimeSystem Time
        {
            get
            {
                if (_time == null)
                {
                    _time = new TimeSystem();
                }
                return _time;
            }
        }

        private static LocalDisk _assetDisk;
        public static LocalDisk AssetDisk
        {
            get
            {
                return _assetDisk ?? (_assetDisk = new LocalDisk(Env.AssetPath));
            }
        }

        private static LocalDisk _assetCryptDisk;
        public static LocalDisk AssetCryptDisk
        {
            get
            {
                return _assetCryptDisk ?? (_assetCryptDisk = new LocalDisk(Env.AssetPath, AESCrypt));
            }
        }

        private static AssetBundleLoader _assetBundleLoader;
        public static AssetBundleLoader AssetBundleLoader
        {
            get
            {
                return _assetBundleLoader ?? (_assetBundleLoader = new AssetBundleLoader());
            }
        }

        private static Resources _res;
        public static Resources Res
        {
            get
            {
                if (_res == null)
                {
                    _res = new Resources();
                    _res.SetHosted(ResHosted);
                }
                return _res;
            }
        }

        private static ResourcesHosted _resHosted;
        public static ResourcesHosted ResHosted
        {
            get
            {
                if (_resHosted == null)
                {
                    _resHosted = new ResourcesHosted();
                    Instance.Load(_resHosted);
                }
                return _resHosted;
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

        private static Hash _hash;
        public static Hash Hash
        {
            get
            {
                if (_hash == null)
                {
                    _hash = new Hash();
                    //这里配置加密因子
                    _hash.SetFactor(16);
                }
                return _hash;
            }
        }

        #endregion
    }
}