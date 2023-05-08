// 开发日志 Bound位置不对 需要处理

using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading;
using System.Collections;
using System.Text;

namespace GPUAnim
{

    ///<summary>
    /// 
    /// 烘焙
    /// 
    /// </summary>
    public class GPUAnimEditor : EditorWindow
    {

        public static Vector2 SIZE = new Vector2(800, 480);
        public static StringBuilder StateMsg = new StringBuilder();

        private Thread __ExportThread;
        private Object __Asset;
        private string __RootPath;


        // 窗口唤出方法
        //[MenuItem("美术工具/GPU动画烘培工具")]
        static void Init()
        {
            GPUAnimEditor window = (GPUAnimEditor)GetWindow(typeof(GPUAnimEditor));
            window.minSize = SIZE;
            window.maxSize = SIZE;
            window.titleContent = new GUIContent("GPUAnimEditor");
            window.Show();
        }



        // 关闭时销毁临时资源
        void OnDestroy()
        {

        }


        // 模型导入器提取
        ModelImporter ImporterExtractor(Object asset)
        {
            // 输入资源为空时返回null
            if (asset == null) return null;
            // 不能提取ModelImporter时返回null
            var modelImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(asset)) as ModelImporter;
            if (modelImporter == null) return null;
            // 返回importer
            return modelImporter;
        }

        Vector2 scrollPos;

        // GUI
        void OnGUI()
        {

            //-------------------- line 0--------------------

            float lineY = 100;
            float lineH = 50;


            GUI.Label(new Rect(10, lineY, 70, 20), "选择文件夹:");

            __Asset = EditorGUI.ObjectField(new Rect(80, lineY, 200, 20), __Asset, typeof(Object), false);


            GUI.Label(new Rect(300, lineY, 70, 20), "已选文件夹:");

            if (__Asset != null)
            {
                __RootPath = AssetDatabase.GetAssetPath(__Asset);
                GUI.Label(new Rect(370, lineY, 400F, 20), AssetDatabase.GetAssetPath(__Asset));


            }
            else
            {
                __RootPath = null;
                StateMsg.Clear();
                StateMsg.Append("暂无... ");
            }

            //--------------------- Line 1 ----------------------

            GUI.Label(new Rect(250, lineY + lineH, 300, 20), "------------------  导出进度  ---------------");

            //------------------------ line2 ------------------------ 

            GUI.BeginGroup(new Rect(100, lineY + lineH * 2 - 30, 600, 180));

            EditorGUILayout.BeginVertical();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(600), GUILayout.Height(160));

            GUILayout.TextArea(StateMsg.ToString(), GUILayout.MinWidth(600), GUILayout.ExpandHeight(true), GUILayout.MaxHeight(100000));

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            GUI.EndGroup();

            //---------------------- line 4---------------------------------

            if (GUI.Button(new Rect(400 - 50, lineY + lineH * 5, 100, 30F), "导出"))
            {
                if (__Asset != null && !string.IsNullOrEmpty(__RootPath))
                {
                    this.Export();
                }
            }

        }

        private void Export()
        {
            // 遍历获得文件列表 
            string[] list = Directory.GetFiles(__RootPath, "*.fbx", SearchOption.AllDirectories);

            if (list == null || list.Length == 0)
            {

                GPUAnimEditor.NewLog = string.Format("当前目录 {0} 没有可导出的文件", __RootPath);

                return;
            }

            GPUAnimEditor.NewLog = string.Format("__RootPath = {0} \n fbx文件总数：{1}", __RootPath, list.Length);


            // 启动新线程导出

            ExportThread exportThread = new ExportThread(list);

            EditorCoroutineRunner.StartEditorCoroutine(exportThread.ExportCoroutine());
            //exportThread.ExportAll();

            //if (__ExportThread != null)
            //{

            //    __ExportThread.Interrupt();
            //}

            //__ExportThread = new Thread(new ThreadStart(exportThread.ExportAll));
            //__ExportThread.Name = "ExportThread";
            //__ExportThread.Start();

        }

        public static string AppandLog
        {
            set
            {
                if (StateMsg == null)
                {
                    StateMsg = new StringBuilder();
                }
                StateMsg.Append(value);
            }
        }

        public static string InsertLog

        {
            set
            {
                if (StateMsg == null)
                {
                    StateMsg = new StringBuilder();
                }

                StateMsg.Insert(0, value);
            }
        }

        public static string NewLog
        {
            set
            {
                if (StateMsg == null)
                {
                    StateMsg = new StringBuilder();
                }

                StateMsg.Clear();
                StateMsg.Append(value);
            }
        }

    }



    class ExportThread
    {
        public string[] paths;



        public ExportThread(string[] paths)
        {
            this.paths = paths;
        }

        // 执行导出 
        public async void ExportAll()
        {
            GPUAnimEditor.AppandLog = "\n";

            for (int i = 0; i < paths.Length; i++)
            {

                this.Bake(paths[i]);

            }


        }

        private List<Object> assetList;
        private List<string> assetPathList;

        public void EThread()
        {
            GPUAnimEditor.AppandLog = "\n";

            assetList = new List<Object>();
            assetPathList = new List<string>();


            for (int i = 0; i < paths.Length; i++)
            {
                Object asset = Resources.Load(paths[i]);

                if (asset == null)
                {
                    asset = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(paths[i]);

                }

                if (asset != null)
                {
                    //string msg = GPUAnimUtil.BakeASync(asset, paths[i]);
                    //GPUAnimEditor.StateMsg += string.Format("导出{0}\n {1}\n", paths[i], msg);
                    //GPUAnimEditor.StateMsg += string.Format("导出 {0}      结束.\n", paths[i]);
                    assetList.Add(asset);
                    assetPathList.Add(paths[i]);
                }
            }
            new Thread(new ThreadStart(DoExport)).Start();
        }

        public void DoExport()
        {
            for (int i = 0; i < this.assetList.Count; i++)
            {
                string msg = GPUAnimUtil.BakeASync(this.assetList[i], this.assetPathList[i]);

                GPUAnimEditor.InsertLog = string.Format("导出{0}\n {1}\n", paths[i], msg);

                GPUAnimEditor.InsertLog = string.Format("导出 {0}      结束.\n", paths[i]);

            }
        }


        public void Bake(string path)
        {
            bool Result = true;

            string msg = GPUAnimUtil.Bake(path, ref Result);


            if (Result)
            {
                GPUAnimEditor.InsertLog = "导出成功!\n";
            }
            else
            {
                GPUAnimEditor.InsertLog = "导出失败!\n";
            }

            GPUAnimEditor.InsertLog = string.Format("导出 {0}\n {1}\n", path, msg);

        }


        // 协程
        public IEnumerator ExportCoroutine()
        {
            GPUAnimEditor.InsertLog = "\n";



            for (int i = 0; i < paths.Length; i++)
            {

                // 只支持    yield return null;


                this.Bake(paths[i]);

                GPUAnimEditor.InsertLog = string.Format("------ {0}/{1} {2} ------ \n", i + 1, paths.Length, System.DateTime.Now.ToLongTimeString());

                yield return null;
            }

            GPUAnimEditor.InsertLog = string.Format("导出结束,总数{0} {1}\n" , paths.Length, System.DateTime.Now.ToLongTimeString() );
        }
    }

}