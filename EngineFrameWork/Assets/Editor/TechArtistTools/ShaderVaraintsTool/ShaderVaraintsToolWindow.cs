using UnityEditor;
using UnityEngine;

namespace TerrainXEditor.ShaderVaraintsTool
{
    [System.Reflection.Obfuscation(Exclude = true)]
    class ShaderVaraintsToolWindow : EditorWindow
    {
        [MenuItem("公共工具/TATools/ShaderVaraints收集工具")]
        private static void ShowWindow()
        {
            ShaderVaraintsToolWindow window = GetWindow(typeof(ShaderVaraintsToolWindow), false, "ShaderVaraints收集工具") as ShaderVaraintsToolWindow;
            window.minSize = new Vector2(600, 400);
            window.Show();
        }

        private void OnGUI()
        {
            if (GUILayout.Button("开始收集"))
            {
                ShaderVaraintsCollecter.Instance.OnlyCollectCG = false;
                ShaderVaraintsCollecter.Instance.Collect();
            }

            if (GUILayout.Button("只收集CG Shader变体"))
            {
                ShaderVaraintsCollecter.Instance.OnlyCollectCG = true;
                ShaderVaraintsCollecter.Instance.Collect();
            }
        }
    }
}
