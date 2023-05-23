using System.IO;
using Core.Hash;
using FrameWork.Launch;
using Core.Interface.IO;
using Core.Interface.AssetBuilder;

/********************************************************************
Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
Created:	2018 ~ 2023
Filename: 	AutoUpdateGenPathStrategy.cs
Author:		DaiLi.Ou
Descriptions: AssetBundleMaker AutoUpdateGenPath Strategy
              Creaeing UpdateList.ini,Which using for recode file gen path with md5 and file length.
*********************************************************************/
namespace Core.AutoUpdate
{
    public sealed class AutoUpdateGenPathStrategy : IBuildStrategy
    {

        public BuildProcess Process
        {
            get { return BuildProcess.GenPath; }
        }

        public void Build(IBuildContext context)
        {
            BuildListFile(context);
        }

        private void BuildListFile(IBuildContext context)
        {
            var lst = new UpdateFile();

            //不进行更新文件加密
            IFile file;
            for (var i = 0; i < context.ReleaseFiles.Length; i++)
            {
                file = context.Disk.File(context.ReleasePath + Path.AltDirectorySeparatorChar + context.ReleaseFiles[i], PathTypes.Absolute);
                lst.Append(context.ReleaseFiles[i], Md5.ParseFile(file.FullName), file.Length);
            }
            var store = IOHelper.UpdateFileStore;
            store.Save(context.ReleasePath, lst);

            //更新文件加密
            //lst = new UpdateFile();
            //for (var i = 0; i < context.EncryptionFiles.Length; i++)
            //{
            //    file = context.Disk.File(context.EncryptionPath + Path.AltDirectorySeparatorChar + context.EncryptionFiles[i], PathTypes.Absolute);
            //    lst.Append(context.EncryptionFiles[i], Md5.ParseFile(file.FullName), file.Length);
            //}
            //store.Save(context.EncryptionPath, lst);

            UnityEngine.Debug.Log("### Generate UpdateFile Success ###");
        }
    }
}