using System;
using UnityEngine;
using System.Reflection;

namespace FrameWork.Launch.Utils
{
    public class Utility
    {
        /// <summary>
        /// v1 > v2 : 1 
        /// v1 < v2 : -1
        /// v1 = v2 : 0
        /// </summary>
        /// <param name="version1"></param>
        /// <param name="version2"></param>
        /// <returns></returns>
        public static int CompareVersion(string version1, string version2)
        {
            int n = version1.Length, m = version2.Length;
            int i = 0, j = 0;
            while (i < n || j < m)
            {
                int x = 0;
                for (; i < n && version1[i] != '.'; ++i)
                {
                    x = x * 10 + version1[i] - '0';
                }
                ++i; // 跳过点号
                int y = 0;
                for (; j < m && version2[j] != '.'; ++j)
                {
                    y = y * 10 + version2[j] - '0';
                }
                ++j; // 跳过点号
                if (x != y)
                {
                    return x > y ? 1 : -1;
                }
            }
            return 0;
        }

        /// <summary>
        /// n : 相差n个版本
        /// 1 : 相差一个版本
        /// </summary>
        /// <param name="version1"></param>
        /// <param name="version2"></param>
        /// <returns></returns>
        public static int SubVersion(string version1, string version2)
        {
            return Math.Abs(ParseVersion(version1) - ParseVersion(version2));
        }

        private static int ParseVersion(string version)
        {
            string[] splitChar = version.Split('.');
            string qianStr = splitChar[0];
            string baiStr = splitChar[1];
            string shiStr = splitChar[2];
            string geStr = splitChar[3];

            int qian = 0, bai = 0, shi = 0, ge = 0;
            int.TryParse(geStr, out ge);
            int.TryParse(shiStr, out shi);
            int.TryParse(baiStr, out bai);
            int.TryParse(qianStr, out qian);
            int total = qian * 1000 + bai * 100 + shi * 10 + ge;

            return total;
        }

        public static void ResolveVolumeManager()
        {
            Debug.Log("[Utility::ResolveVolumeManager] Start");

            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Static;

            #region 清理缓存的types

            var coreUtilsType = Type.GetType("UnityEngine.Rendering.CoreUtils,Unity.RenderPipelines.Core.Runtime");
            if (coreUtilsType == null)
            {
                Debug.Log("[Utility::ResolveVolumeManager] 无需处理CoreUtils,你可能不在URP环境下");
                return;
            }

            coreUtilsType.GetField("m_AssemblyTypes", bindingFlags)?.SetValue(null, null);

            #endregion

            #region VolumeManager处理

            var volumeMgrType = Type.GetType("UnityEngine.Rendering.VolumeManager,Unity.RenderPipelines.Core.Runtime");
            if (volumeMgrType == null)
            {
                Debug.Log("[Utility::ResolveVolumeManager] 无需处理VolumeManager,你可能不在URP环境下");
                return;
            }

            bindingFlags = BindingFlags.Public | BindingFlags.Static;
            var mgrInstanceField = volumeMgrType.GetProperty("instance", bindingFlags);
            if (mgrInstanceField == null)
            {
                Debug.LogError("[Utility::ResolveVolumeManager] 无法找到VolumeManager的instance");
                return;
            }

            var mgrInstance = mgrInstanceField.GetValue(null);
            bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
            var reloadBaseTypesMethod = volumeMgrType.GetMethod("ReloadBaseTypes", bindingFlags);
            if (reloadBaseTypesMethod == null)
            {
                Debug.Log($"[Utility::ResolveVolumeManager] reloadBaseTypesMethod is null");
                return;
            }

            // Mgr的stack重置
            bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var stackProperty = volumeMgrType.GetProperty("stack", bindingFlags);
            if (stackProperty == null)
            {
                Debug.Log($"[Utility::ResolveVolumeManager] stackProperty is null");
                return;
            }

            bindingFlags = BindingFlags.Public | BindingFlags.Instance;
            var createStackMethod = volumeMgrType.GetMethod("CreateStack", bindingFlags);
            if (createStackMethod == null)
            {
                Debug.LogError("[Utility::ResolveVolumeManager] 无法找到VolumeManager的CreateStack方法");
                return;
            }

            reloadBaseTypesMethod.Invoke(mgrInstance, null);
            stackProperty.SetValue(mgrInstance, createStackMethod.Invoke(mgrInstance, null));

            #endregion

            Debug.Log($"[Utility::ResolveVolumeManager] Finished ");
        }
    }


}
