using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEditor.ProjectWindowCallback;

namespace CreateMVCSTemplate
{
    public class CreateMVCSTemplate
    {

        public enum MVCS
        {
            Root,
            Context,
            View,
            Mediator
        }

        [MenuItem("Assets/Create/程序工具/Create UI-MVCS Floder")]
        public static void CreateMVCSTEvent()
        {
            CreateMVCSFolderEndNameEditAction folderEndNameEditAction = ScriptableObject.CreateInstance<CreateMVCSFolderEndNameEditAction>();
            folderEndNameEditAction.overAction = (fileName) =>
            {
                string title = string.Format("创建{0}", fileName);

                EditorUtility.DisplayProgressBar(title, string.Format("创建{0}{1}", fileName, MVCS.Root.ToString()), 1 / 4.0f);
                CreateMVCSScript("", fileName, MVCS.Root);
                EditorUtility.DisplayProgressBar(title, string.Format("创建{0}{1}", fileName, MVCS.Context.ToString()), 2 / 4.0f);
                CreateMVCSScript("", fileName, MVCS.Context);
                EditorUtility.DisplayProgressBar(title, string.Format("创建{0}{1}", fileName, MVCS.View.ToString()), 3 / 4.0f);
                CreateMVCSScript("View", fileName, MVCS.View);
                EditorUtility.DisplayProgressBar(title, string.Format("创建{0}{1}", fileName, MVCS.Mediator.ToString()), 4 / 4.0f);
                CreateMVCSScript("View", fileName, MVCS.Mediator);
                EditorUtility.ClearProgressBar();
            };

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, folderEndNameEditAction,
                    GetSelectPathOrFallback() + "/New MVCS Floder", null, null);
        }

        /// <summary>
        /// 使用EndNameEditAction方法创建文件
        /// </summary>
        public static void CreateMVCSFolderEndNameEditor()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<CreateMVCSFolderEndNameEditAction>(),
GetSelectPathOrFallback() + "/New MVCS Floder", null, null);
        }
        /// <summary>
        /// 使用EndNameEditAction方法创建脚本
        /// </summary>
        public static void CreateMVCSScriptEndNameEditor()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<CreateMVCSScriptEndNameEditAction>(),
    GetSelectPathOrFallback() + "/NewMVCSScript.cs", null, @"Assets\Editor\TechnicalTools\CreatMVCSTemplate\Editor\MVCSNameRoot.cs");
        }

        /// <summary>
        /// 直接创建文件 
        /// </summary>
        /// <param name="fileName">创建的文件名</param>
        public static void CreateMVCSFolder(string fileName)
        {
            AssetDatabase.CreateFolder(GetSelectPathOrFallback(), fileName);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 直接创建脚本
        /// </summary>
        /// <param name="parentFolderName">创建脚本的额外文件夹 为空表示不需要</param>
        /// <param name="fileName">文件名</param>
        /// <param name="MVCS">MVCS框架类型</param>
        public static void CreateMVCSScript(string parentFolderName, string fileName, MVCS mVCS)
        {
            string pathName = string.Empty;
            string localPath = GetSelectPathOrFallback();

            if (!string.IsNullOrEmpty(parentFolderName))
            {
                string parentFolderPath = Path.Combine(localPath, parentFolderName);
                if (!Directory.Exists(parentFolderPath))
                    CreateMVCSFolder(parentFolderName);
                pathName = string.Format("{0}/{3}/{1}{2}.cs", localPath, fileName, mVCS.ToString(), parentFolderName);
            }
            else
                pathName = string.Format("{0}/{1}{2}.cs", localPath, fileName, mVCS.ToString());

            string resourceFile = string.Format(@"Assets\Editor\TechnicalTools\CreatMVCSTemplate\MVCSName{0}.cs", mVCS.ToString());
            //获取要创建资源的绝对路径
            string fullPath = Path.GetFullPath(pathName);
            //读取本地的模板文件
            StreamReader streamReader = new StreamReader(resourceFile);
            string text = streamReader.ReadToEnd();
            streamReader.Close();
            //获取文件名，不含扩展名
            //string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
            string fileNameWithoutExtension = fileName;

            //将模板类中的类名替换成你创建的文件名
            text = Regex.Replace(text, "MVCSName", fileNameWithoutExtension);
            bool encoderShouldEmitUTF8Identifier = true; //参数指定是否提供 Unicode 字节顺序标记
            bool throwOnInvalidBytes = false;//是否在检测到无效的编码时引发异常
            UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
            bool append = false;
            //写入文件
            StreamWriter streamWriter = new StreamWriter(fullPath, append, encoding);
            streamWriter.Write(text);
            streamWriter.Close();
            //刷新资源管理器
            AssetDatabase.ImportAsset(pathName);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 取得要创建文件的路径
        /// </summary>
        /// <returns></returns>
        internal static string GetSelectPathOrFallback()
        {
            string path = "Assets";
            //遍历选中的资源以获得路径
            //Selection.GetFiltered是过滤选择文件或文件夹下的物体，assets表示只返回选择对象本身
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
    }

    /// <summary>
    /// 创建脚本的EndNameEditAction方法
    /// </summary>
    class CreateMVCSScriptEndNameEditAction : CreateMVCSAssetEndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            Object obj = CreateScriptAssetFromTemplate(pathName, resourceFile);
            ProjectWindowUtil.ShowCreatedAsset(obj);
        }

        internal static Object CreateScriptAssetFromTemplate(string pathName, string resourceFile)
        {
            //获取要创建资源的绝对路径
            string fullPath = Path.GetFullPath(pathName);
            //读取本地的模板文件
            StreamReader streamReader = new StreamReader(resourceFile);
            string text = streamReader.ReadToEnd();
            streamReader.Close();
            //获取文件名，不含扩展名
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);

            //将模板类中的类名替换成你创建的文件名
            text = Regex.Replace(text, "MVCSName", fileNameWithoutExtension);
            bool encoderShouldEmitUTF8Identifier = true; //参数指定是否提供 Unicode 字节顺序标记
            bool throwOnInvalidBytes = false;//是否在检测到无效的编码时引发异常
            UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
            bool append = false;
            //写入文件
            StreamWriter streamWriter = new StreamWriter(fullPath, append, encoding);
            streamWriter.Write(text);
            streamWriter.Close();
            //刷新资源管理器
            AssetDatabase.ImportAsset(pathName);
            AssetDatabase.Refresh();
            return AssetDatabase.LoadAssetAtPath(pathName, typeof(Object));
        }
    }

    /// <summary>
    /// 创建文件的EndNameEditAction方法
    /// </summary>
    class CreateMVCSFolderEndNameEditAction : CreateMVCSAssetEndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            string guid = AssetDatabase.CreateFolder(Path.GetDirectoryName(pathName), Path.GetFileName(pathName));
            Object obj = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(Object));
            ProjectWindowUtil.ShowCreatedAsset(obj);
            fileName = Path.GetFileName(pathName);
            if (overAction != null) overAction(fileName);
        }
    }

    /// <summary>
    /// 创建资源的EndNameEditAction方法
    /// </summary>
    class CreateMVCSAssetEndNameEditAction : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
        }

        public string fileName = string.Empty;
        public System.Action<string> overAction = null;
    }
}