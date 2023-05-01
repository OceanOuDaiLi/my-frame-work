// 开发日志 Bound位置不对 需要处理

using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading;

namespace TheNextMoba.ArtTools.Anim
{

    ///<summary>
    /// 
    /// 
    /// </summary>
    public class GPUAnimMain
    {


        private static Thread _Thread;

        // 选择将导出所有目录下的文件 
        [MenuItem("Assets/右键选择导出GPUAnim", false)]
        private static void SelectedAssets()
        {

            string[] guids = Selection.assetGUIDs;//获取当前选中的asset的GUID

            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);//通过GUID获取路径
                Debug.LogError("选中  " + assetPath);
                GPUAnimMain.Export(assetPath);
            }
        }



        private static void Export(string root)
        {
            if (string.IsNullOrEmpty(root))
            {
                Debug.LogError("IsNullOrEmpty ");
                return;
            }

            // 遍历获得文件列表 
            FileInfo[] infos = new DirectoryInfo(root).GetFiles("*.fbx", SearchOption.AllDirectories);
            Debug.LogError("GetFiles --- \n" + infos.Length);

            // 启动新线程导出

            ExportThread exportThread = new ExportThread(infos);

            if (_Thread != null  )
            {

                _Thread.Interrupt();
            }
            _Thread = new Thread(new ThreadStart(exportThread.ExportAll));
            _Thread.Name = "ExportAll";
            _Thread.Start();

        }



    }


    class ExportThread
    {
        public string[] paths;


        public ExportThread(FileInfo[] infos)
        {
            this.paths = new string[infos.Length];

            for (int i = 0; i < infos.Length; i++)
            {
                this.paths[i] = infos[i].FullName;
            }
        }



        public ExportThread(string[] paths)
        {
            this.paths = paths;
        }

        public void ExportAll()
        {

            string msg = "";
            for (int i = 0; i < paths.Length; i++)
            {
                msg += paths[i] + " \n";
                Debug.LogError(" i = " + i + "  =>> " + paths[i]);
                Thread.Sleep(10000);
            }
            Debug.Log(msg);
        }

    }
}