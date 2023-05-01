using UnityEngine;
using UnityEditor;

namespace MoreFun.NextEffect.GPUAnim
{
    ///<summary>
    ///
    ///  调整Material 的Shader
    /// </summary>
    /// 
    [CustomEditor(typeof(GPUAnimPlayer))]
    public class GPUAnimPlayerGUI : Editor
    {
        GPUAnimPlayer myTarget;

        void OnEnable()
        {
            myTarget = target as GPUAnimPlayer;
        }

        public override void OnInspectorGUI()
        {
            if (CheckValidity())
            {
                var rect = GUILayoutUtility.GetRect(100.0f, 20.0f);
                GUI.Label(new Rect(rect.x, rect.y + 2.0f, 60.0f, 18.0f), "循环区间");
                EditorGUI.MinMaxSlider(new Rect(rect.x + 60.0f, rect.y + 2.0f, rect.width - 60.0f, 18.0f), ref myTarget.inScale, ref myTarget.outScale, 0.0f, 1.0f);
                rect = GUILayoutUtility.GetRect(100.0f, 20.0f);

                GUI.Label(new Rect(rect.x, rect.y + 2.0f, 60.0f, 18.0f), "淡出区间");
                EditorGUI.BeginChangeCheck();
                {
                    EditorGUI.MinMaxSlider(new Rect(rect.x + 60.0f, rect.y + 2.0f, rect.width - 60.0f, 18.0f), ref myTarget.fadeInScale, ref myTarget.fadeOutScale, 0.0f, 1.0f);
                }

                if (EditorGUI.EndChangeCheck())
                {
                    myTarget.fadeInScale = Mathf.Min(myTarget.fadeInScale, myTarget.inScale);
                    myTarget.fadeOutScale = Mathf.Max(myTarget.fadeOutScale, myTarget.outScale);
                }

                rect = GUILayoutUtility.GetRect(100.0f, 20.0f);
                GUI.Label(new Rect(rect.x, rect.y + 2.0f, 60.0f, 18.0f), "播放速度");

                myTarget.speed = EditorGUI.Slider(new Rect(rect.x + 60.0f, rect.y + 2.0f, rect.width - 60.0f, 18.0f), myTarget.speed, 0.0f, 2.0f);
                if (Application.isPlaying == true)
                {
                    rect = GUILayoutUtility.GetRect(100.0f, 20.0f);
                    if (GUI.Button(new Rect(rect.x, rect.y + 2.0f, rect.width * 0.25f, 16.0f), "Init", EditorStyles.miniButtonLeft)) myTarget.Init();
                    if (GUI.Button(new Rect(rect.x + rect.width * 0.25f, rect.y + 2.0f, rect.width * 0.25f, 16.0f), "Play", EditorStyles.miniButtonMid)) myTarget.Play();
                    if (GUI.Button(new Rect(rect.x + rect.width * 0.5f, rect.y + 2.0f, rect.width * 0.25f, 16.0f), "Cease", EditorStyles.miniButtonMid)) myTarget.Stop(false);
                    if (GUI.Button(new Rect(rect.x + rect.width * 0.75f, rect.y + 2.0f, rect.width * 0.25f, 16.0f), "Stop", EditorStyles.miniButtonRight)) myTarget.Stop(true);
                }
            }
            else
            {
                GUILayout.Label("资源配置不支持此功能.");
            }
        }

        // 检查有效性
        private bool CheckValidity()
        {
            // 检查获得Editor目标
            if (myTarget == null) myTarget = target as GPUAnimPlayer;
            // 尝试获得MeshRenderer
            var mr = myTarget.GetComponent<MeshRenderer>();
            if (mr == null) return false;
            // 尝试获得材质
            var mat = mr.sharedMaterial;
            if (mat == null) return false;
            // 检查材质是否具备必要属性
            if (mat.IsKeywordEnabled("_GA_CYCLE")) return false;
            if (!mat.HasProperty("_AnimProgress")) return false;
            if (!mat.HasProperty("_Opacity")) return false;
            // 返回值
            return true;
        }
    }
}