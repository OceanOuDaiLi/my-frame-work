using cfg;
using System.IO;
using SimpleJSON;
using UnityEngine;
using System.Collections.Generic;

namespace Model
{
    public class ConfigMgr
    {
        private Dictionary<string, object> m_tables = new Dictionary<string, object>();

        public T Load<T>() where T : ConfigBase, new()
        {
            string szConfig = typeof(T).Name;
            if (m_tables.ContainsKey(szConfig) == true)
                return (T)m_tables[szConfig];

            T insConfig = new T();
            insConfig.load(LoadByteBuf(szConfig));
            m_tables[szConfig] = insConfig;
            return insConfig;
        }

        private static JSONNode LoadByteBuf(string szFile)
        {
            return JSON.Parse(File.ReadAllText(Application.dataPath + "/ABAssets/NotAssetBundle/config/" + szFile + ".json", System.Text.Encoding.UTF8));
        }

        public virtual void OnDispose()
        {
            // Profiler.Sample Dispose Cost.
            m_tables.Clear();
        }
    }
}
