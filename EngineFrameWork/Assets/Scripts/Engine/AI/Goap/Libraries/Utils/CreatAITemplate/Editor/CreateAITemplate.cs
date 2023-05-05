using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Goap.Extensions;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor.ProjectWindowCallback;

/// <summary>
/// Author：Daili.OU
/// Created Time：2021/10/22
/// Descriptions：Editor Tools, Creating AI template scripts, prefabs and so on..
/// </summary>
namespace Goap.AI
{
    public enum AiScriptType
    {
        State,
        Sense,
        Event
    }

    [InitializeOnLoad]
    public class CreateAITemplate : AssetPostprocessor
    {
        static float updateRatio = 0.0f;
        static string SCRIPT_FLODER_NAME = "AIStates";
        static string OBJECT_FLODER_NAME = "AIObjects";
        static string LOCAL_KEY = "LocalKey_Creat_AI_Tmplate";


        /// <summary>
        /// Auto Create AI Tempate Scripts and prefabs.
        /// </summary>
        /// <param name="_current"></param>
        public static void CreateScriptsTemplate(GoapAIScenario _current, string copyEnum)
        {
            CreateAIFolderEndNameEditAction folderEndNameEditAction = ScriptableObject.CreateInstance<CreateAIFolderEndNameEditAction>();

            folderEndNameEditAction.overAction = (fileName) =>
            {
                string title = string.Format("创建{0}", fileName);
                int len = _current.actions.Length;
                PlayerPrefs.SetString(LOCAL_KEY, string.Format("{0}/{1}", GetSelectPathOrFallback(), OBJECT_FLODER_NAME));

                //CreateAIEvent
                EditorUtility.DisplayProgressBar(title, "创建事件脚本", 1 / 4);
                CreateAIEvent("", _current.name, _current);

                //Createing State Scripts and State GameObject.
                EditorUtility.DisplayProgressBar(title, string.Format("创建AI{0}脚本", AiScriptType.State.ToString()), 2 / 4);
                for (int i = 0; i < len; i++)
                {
                    CreateAIScript(SCRIPT_FLODER_NAME, _current.actions[i].name, AiScriptType.State, _current.name, _current.actions[i]);
                    CreateAIGameObject(OBJECT_FLODER_NAME, _current.actions[i].name, _current.actions[i]);
                }

                //Createing CharacterScripts
                EditorUtility.DisplayProgressBar(title, string.Format("创建AI{0}脚本", AiScriptType.Sense.ToString()), 3 / 4);
                string enumTitle = (_current != null) ? _current.name : "AICondition";
                CreateAISense("", _current.name, copyEnum, enumTitle, _current);

                EditorUtility.DisplayProgressBar(title, string.Format("创建AI{0}脚本", AiScriptType.Sense.ToString()), 1);
                EditorUtility.DisplayProgressBar("Waiting...", "等待脚本编译结束", updateRatio);
            };

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                                folderEndNameEditAction,
                                _current.name,//string.Format("{0}/{1}", GetSelectPathOrFallback(), _current.name)
                                null, null);
        }

        /// <summary>
        /// Create AI Scripts by Type.
        /// </summary>
        /// <param name="parentFolderName"></param>
        /// <param name="fileName"></param>
        /// <param name="sType"></param>
        internal static void CreateAIScript(string parentFolderName, string fileName, AiScriptType sType, string senseName, GoapAIScenarioAction action)
        {
            string pathName = string.Empty;
            string localPath = GetSelectPathOrFallback();

            if (!string.IsNullOrEmpty(parentFolderName))
            {
                string parentFolderPath = Path.Combine(localPath, parentFolderName);
                if (!Directory.Exists(parentFolderPath))
                    CreateFolder(parentFolderName);

                pathName = string.Format("{0}/{3}/{1}{2}.cs", localPath, fileName, sType.ToString(), parentFolderName);
            }
            else
                pathName = string.Format("{0}/{1}{2}.cs", localPath, fileName, sType.ToString());

            string resourceFile = string.Format(@"Assets\Scripts\Engine\AI\Goap\Libraries\Utils\CreatAITemplate\Editor\TacticsAI{0}.cs", sType.ToString());
            string fullPath = Path.GetFullPath(pathName);
            StreamReader streamReader = new StreamReader(resourceFile);
            string text = streamReader.ReadToEnd();
            streamReader.Close();
            string fileNameWithoutExtension = fileName;

            text = Regex.Replace(text, "TacticsAI", fileNameWithoutExtension);

            text = Regex.Replace(text, "//Sense", string.Format("{0}Sense sense = null;", senseName));
            text = Regex.Replace(text, "//Isense", string.Format("this.sense = sense as {0}Sense;", senseName));

            //SetRegister
            string eventStr = string.Empty;
            //RegisterImplement
            string eventApplayStr = string.Empty;
            //SetDispatch
            string eventDispatchStr = string.Empty;
            //RemoveEvent
            string removeStr = string.Empty;
            for (int i = 0; i < action.events.Length; i++)
            {
                if (action.events[i].goapEventType.Equals(GoapEventType.Register))
                {
                    eventStr += "\n";
                    eventStr += string.Format("            App.Instance.On({0}Event.{1}, {2});", senseName, action.events[i].eventName.ToUpper(), action.events[i].eventName);
                    eventStr += "\n";

                    eventApplayStr += "\n";
                    eventApplayStr += string.Format("        void {0}(object sender, object accepter, IEventArgs e)  \n   {1}\n\n{2}", action.events[i].eventName, "     {", "        }");
                    eventApplayStr += "\n";

                    removeStr += "\n";
                    removeStr += string.Format("            App.Instance.Off({0}Event.{1}, null, this);", senseName, action.events[i].eventName.ToUpper(), action.events[i].eventName);
                    removeStr += "\n";
                }
                else if (action.events[i].goapEventType.Equals(GoapEventType.Dispatch))
                {
                    eventDispatchStr += "\n";
                    eventDispatchStr += string.Format("        void Dispatch_{0}() \n    {1} \n            App.Instance.Trigger({2}Event.{3});   \n    {4}"
                        , action.events[i].eventName
                        , "    {"
                        , senseName
                        , action.events[i].eventName.ToUpper()
                        , "    }");
                    eventDispatchStr += "\n";


                }
            }
            eventDispatchStr += "\n";

            //SetCommands
            string commandStr = string.Empty;
            string tmpDescr = string.Empty;
            for (int i = 0; i < action.commands.Length; i++)
            {
                tmpDescr = action.commands[i].commandDescr;
                tmpDescr = tmpDescr.Replace("\n", "\n        /// ");
                commandStr += "\n";
                commandStr += string.Format("{0}\n{1} 时调用\n{2}\n{3}\n{4}\n"
                    , "        /// <summary>"
                    , "        /// " + action.commands[i].commandLife.ToString()
                    , "        /// " + GetCommanTypeStr(action.commands[i].excuteType, action.commands[i].delayStartTm.ToString())
                    , "        /// 指令描述：" + tmpDescr
                    , "        /// </summary>"
                    );
                commandStr += string.Format("        void Excute{0}() \n    {1} \n            //Do sth..;   \n    {2}"
                        , action.commands[i].name
                        , "    {"
                        , "    }");
                commandStr += "\n";
            }

            if (action.commands.Length < 1)
            {
                commandStr = "//This action don't have any commands";
            }

            text = Regex.Replace(text, "//SetRegister", eventStr);
            text = Regex.Replace(text, "//RegisterImplement", eventApplayStr);
            text = Regex.Replace(text, "//SetDispatch", eventDispatchStr);
            text = Regex.Replace(text, "//SetCommands", commandStr);
            text = Regex.Replace(text, "//RemoveEvent", removeStr);

            bool encoderShouldEmitUTF8Identifier = true;
            bool throwOnInvalidBytes = false;
            UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
            bool append = false;

            StreamWriter streamWriter = new StreamWriter(fullPath, append, encoding);
            streamWriter.Write(text);
            streamWriter.Close();

            AssetDatabase.ImportAsset(pathName);
            AssetDatabase.Refresh();
        }

        static string GetCommanTypeStr(CommandExcuteType t, string dyTm = "")
        {
            string str = string.Empty;
            switch (t)
            {
                case CommandExcuteType.ImmeDiately:
                    str = "执行方式: 立即执行";
                    break;
                case CommandExcuteType.Delay:
                    str = string.Format("执行方式: 延迟 {0}s 执行", dyTm);
                    break;
                case CommandExcuteType.Customer:
                    str = "执行方式: 自定义条件执行";
                    break;
                default:
                    break;
            }

            return str;
        }

        /// <summary>
        /// Create AI State Prefab.
        /// </summary>
        /// <param name="parentFolderName"></param>
        /// <param name="fileName"></param>
        /// <param name="ac"></param>
        internal static void CreateAIGameObject(string parentFolderName, string fileName, GoapAIScenarioAction ac)
        {
            string pathName = string.Empty;
            string localPath = GetSelectPathOrFallback();

            string parentFolderPath = Path.Combine(localPath, parentFolderName);
            if (!Directory.Exists(parentFolderPath))
                CreateFolder(parentFolderName);

            GameObject obj = new GameObject();
            obj.transform.position = Vector3.zero;

            pathName = string.Format("{0}/{1}/{2}.prefab", localPath, parentFolderName, fileName);

            //PrefabUtility.SaveAsPrefabAsset(obj, pathName);
            PrefabUtility.CreatePrefab(pathName, obj);

            AssetDatabase.Refresh();

            GameObject.DestroyImmediate(obj);

            ac.state = AssetDatabase.LoadAssetAtPath(pathName, typeof(Object)) as GameObject;
        }

        /// <summary>
        /// Create AI Scene Script
        /// </summary>
        /// <param name="parentFolderName"></param>
        /// <param name="fileName"></param>
        /// <param name="copyEnum"></param>
        /// <param name="enumTitle"></param>
        /// <param name="_current"></param>
        /// <param name="sType"></param>
        internal static void CreateAISense(string parentFolderName, string fileName, string copyEnum, string enumTitle, GoapAIScenario _current, AiScriptType sType = AiScriptType.Sense)
        {
            string localPath = GetSelectPathOrFallback();
            string pathName = string.Format("{0}/{1}{2}.cs", localPath, fileName, sType.ToString());

            string resourceFile = string.Format(@"Assets\Scripts\Engine\AI\Goap\Libraries\Utils\CreatAITemplate\Editor\TacticsAI{0}.cs", sType.ToString());
            string fullPath = Path.GetFullPath(pathName);
            StreamReader streamReader = new StreamReader(resourceFile);
            string text = streamReader.ReadToEnd();
            streamReader.Close();
            string fileNameWithoutExtension = fileName;

            //Class  Name       Replace.
            text = Regex.Replace(text, "TacticsAI", fileNameWithoutExtension);
            //Enum   Content    Replace.
            text = Regex.Replace(text, "public enum TemplateSense { ", copyEnum);
            text = Regex.Replace(text, "TemplateSense", enumTitle);


            //Init StateDic
            //AddDic
            string dicStr = string.Empty;
            string conditionName = string.Empty;
            for (int i = 0; i < _current.conditions.list.Length; i++)
            {
                conditionName = _current.conditions.list[i].name.RemoveSpaces();
                //StateDic[TacticsExample.Initialization] = true;
                dicStr += string.Format("stateDic[{0}.{1}] = {2};", enumTitle, conditionName, i == 0 ? "true" : "false");
                if (i != _current.conditions.list.Length - 1)
                    dicStr += "\n            ";
            }
            text = Regex.Replace(text, "//AddDic", dicStr);


            //Condition Function Replace.
            string funcStr = string.Empty;

            List<string> conditions = new List<string>();
            for (int i = 0, n = _current.conditions.list.Length; i < n; i++)
            {
                conditionName = _current.conditions.list[i].name.RemoveSpaces();
                conditions.Add(conditionName);

                string func = string.Format("bool Judge{0}() {1} return stateDic[{2}.{3}]; {4}", conditionName, "{", enumTitle, conditionName, "}");
                funcStr += func;
                funcStr += "\n";
                funcStr += "        ";                                     //just for code resign...
            }
            text = Regex.Replace(text, "//JudgeFunc", funcStr);

            //WorldState Content Replace.
            string worldStr = string.Empty;
            for (int i = 0; i < conditions.Count; i++)
            {
                worldStr += string.Format("aWorldState.Set({0}{1}{2}, Judge{3}());", '"', conditions[i], '"', conditions[i]);
                worldStr += "\n";
                worldStr += "                ";                             //just for code resign...
            }
            text = Regex.Replace(text, "//SetState", worldStr);

            bool encoderShouldEmitUTF8Identifier = true;
            bool throwOnInvalidBytes = false;
            UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
            bool append = false;

            StreamWriter streamWriter = new StreamWriter(fullPath, append, encoding);
            streamWriter.Write(text);
            streamWriter.Close();

            AssetDatabase.ImportAsset(pathName);
            AssetDatabase.Refresh();
        }

        internal static void CreateAIEvent(string parentFolderName, string fileName, GoapAIScenario _current, AiScriptType sType = AiScriptType.Event)
        {
            string localPath = GetSelectPathOrFallback();
            string pathName = string.Format("{0}/{1}{2}.cs", localPath, fileName, sType.ToString());

            string resourceFile = string.Format(@"Assets\Scripts\Engine\AI\Goap\Libraries\Utils\CreatAITemplate\Editor\TacticsAI{0}.cs", sType.ToString());
            string fullPath = Path.GetFullPath(pathName);
            StreamReader streamReader = new StreamReader(resourceFile);
            string text = streamReader.ReadToEnd();
            streamReader.Close();
            string fileNameWithoutExtension = fileName;

            //Class  Name       Replace.
            text = Regex.Replace(text, "TacticsAI", fileNameWithoutExtension);

            string eventList = string.Empty;
            for (int i = 0; i < _current.events.list.Length; i++)
            {
                eventList += "\n";
                eventList += string.Format("        public static readonly string {0} = {1}{2}{3};", _current.events.list[i].eventName.ToUpper(), '"',_current.events.list[i].eventName.ToLower(), '"');
            }
            eventList += "\n";
            text = Regex.Replace(text, "//SetEvent", eventList);

            bool encoderShouldEmitUTF8Identifier = true;
            bool throwOnInvalidBytes = false;
            UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
            bool append = false;

            StreamWriter streamWriter = new StreamWriter(fullPath, append, encoding);
            streamWriter.Write(text);
            streamWriter.Close();

            AssetDatabase.ImportAsset(pathName);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Create a Floder..
        /// </summary>
        /// <param name="floderName"></param>
        internal static void CreateFolder(string floderName)
        {
            AssetDatabase.CreateFolder(GetSelectPathOrFallback(), floderName);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Get curent user stay path on Project.
        /// </summary>
        /// <returns></returns>
        internal static string GetSelectPathOrFallback()
        {
            string path = "Assets";
            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }
            return path;
        }

        /// <summary>
        /// Doing 'Add Component' for Prefabs.
        /// Can only add components after the script has compiled.
        /// </summary>
        [UnityEditor.Callbacks.DidReloadScripts]
        static void AllScriptsReloaded()
        {
            string value = PlayerPrefs.GetString(LOCAL_KEY);
            if (!string.IsNullOrEmpty(value))
            {
                string[] allPath = AssetDatabase.FindAssets("t:Prefab", new string[] { value });
                string path = string.Empty;
                GameObject tmp;
                for (int i = 0; i < allPath.Length; i++)
                {
                    path = AssetDatabase.GUIDToAssetPath(allPath[i]);
                    tmp = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                    if (tmp != null)
                    {
                        GameObject instanceRoot = PrefabUtility.InstantiatePrefab(tmp) as GameObject;
                        Assembly asmb = System.Reflection.Assembly.Load("Assembly-CSharp");
                        System.Type stateScr = asmb.GetType(string.Format("{0}{1}State", "Goap.AI.", instanceRoot.name));
                        if (stateScr == null)
                        {
                            ZDebug.LogError("Empty Scripts");
                            continue;
                        }

                        instanceRoot.AddComponent(stateScr);

                        //PrefabUtility.SaveAsPrefabAsset(instanceRoot, path);
                        PrefabUtility.CreatePrefab(path, instanceRoot);

                        GameObject.DestroyImmediate(instanceRoot);
                    }

                    updateRatio = i / allPath.Length;
                }

                //For Unity 2017. Reset scenario action state prefab.
                string assetPath = value.Remove(value.LastIndexOf("/"));
                GoapAIScenario scenario = (GoapAIScenario)AssetDatabase.LoadAssetAtPath(assetPath + ".asset", typeof(object));
                if (scenario != null)
                {
                    for (int i = 0; i < scenario.actions.Length; i++)
                    {
                        string localPath = GetSelectPathOrFallback();
                        string pathName = string.Format("{0}/{1}/{2}.prefab", localPath, OBJECT_FLODER_NAME, scenario.actions[i].name);
                        scenario.actions[i].state = AssetDatabase.LoadAssetAtPath(pathName, typeof(Object)) as GameObject;
                    }
                }

                AssetDatabase.Refresh();
                EditorUtility.ClearProgressBar();
                PlayerPrefs.SetString(LOCAL_KEY, string.Empty);
            }
        }
    }

    internal class CreateMVCSScriptEndNameEditAction : CreateAIAssetEndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            Object obj = CreateScriptAssetFromTemplate(pathName, resourceFile);
            ProjectWindowUtil.ShowCreatedAsset(obj);
        }

        internal static Object CreateScriptAssetFromTemplate(string pathName, string resourceFile)
        {
            string fullPath = Path.GetFullPath(pathName);
            StreamReader streamReader = new StreamReader(resourceFile);
            string text = streamReader.ReadToEnd();
            streamReader.Close();

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
            text = Regex.Replace(text, "TacticsAI", fileNameWithoutExtension);
            bool encoderShouldEmitUTF8Identifier = true;
            bool throwOnInvalidBytes = false;
            UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
            bool append = false;

            StreamWriter streamWriter = new StreamWriter(fullPath, append, encoding);
            streamWriter.Write(text);
            streamWriter.Close();

            AssetDatabase.ImportAsset(pathName);
            AssetDatabase.Refresh();
            return AssetDatabase.LoadAssetAtPath(pathName, typeof(Object));
        }
    }

    internal class CreateAIFolderEndNameEditAction : CreateAIAssetEndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            fileName = pathName.Remove(0, pathName.LastIndexOf("/") + 1);       //For unity2017 reset file name.
            string guid = AssetDatabase.CreateFolder(pathName, fileName);
            Object obj = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(Object));
            ProjectWindowUtil.ShowCreatedAsset(obj);
            if (overAction != null) overAction(fileName);
        }
    }

    internal class CreateAIAssetEndNameEditAction : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
        }

        public string fileName = string.Empty;
        public System.Action<string> overAction = null;
    }
}