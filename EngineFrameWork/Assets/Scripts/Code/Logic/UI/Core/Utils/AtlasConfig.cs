using System.Collections.Generic;

namespace UI
{
    public class AtlasConfig
    {
        public static Dictionary<string, List<string>> PrefabAtlasDependenceDic = new Dictionary<string, List<string>>()
        {
            {"login" , new List<string>(){"login"  , "common"  } },
            {"main" , new List<string>(){"main"  , "common"  } },
            {"namepanel" , new List<string>(){} },
            {"userinfo" , new List<string>(){"userinfo"  , "common"  } },
            {"yaoling" , new List<string>(){"yaoling"  } },

        };


    }
}