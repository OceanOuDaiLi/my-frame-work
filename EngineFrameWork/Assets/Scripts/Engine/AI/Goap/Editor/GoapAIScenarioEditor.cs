using UnityEngine;
namespace Goap.AI
{
    [UnityEditor.CustomEditor(typeof(GoapAIScenario))]
    public class GoapAIScenarioEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Open AI Editor Window", GUILayout.MinHeight(40.0f)))
            {
                GoapAIWorkbench.OpenScenario(target.name);
            }
        }
    }
}