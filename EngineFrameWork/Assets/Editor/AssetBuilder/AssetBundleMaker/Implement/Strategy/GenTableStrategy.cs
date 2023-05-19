using FrameWork;
using System.IO;
using Core.Interface.IO;
using System.Collections.Generic;
using Core.Interface.AssetBuilder;

/********************************************************************
Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
Created:	2018 ~ 2023
Filename: 	EncryptionStrategy.cs
Author:		DaiLi.Ou
Descriptions: AssetBundleMaker GenTable Strategy -> NotAssetBundle Files encrypt or generate..
*********************************************************************/
namespace Core.AssetBuilder
{
    public sealed class GenTableStrategy : IBuildStrategy
    {
        public BuildProcess Process => BuildProcess.GenTable;

        void EncryptFile(IBuildContext context, string fileName)
        {
            IDisk encryptDisk = AssetBundlesMaker.IO.Disk(context.ReleasePath, App.AESCrypt);
            IFile dbFile = context.Disk.File(fileName, PathTypes.Relative);
            IFile dbFileEncrypt = encryptDisk.File(fileName + ".tmp", PathTypes.Relative);
            if (dbFile.Exists)
            {
                if (dbFileEncrypt.Exists) dbFileEncrypt.Delete();
                dbFileEncrypt.Create(dbFile.Read());

                dbFile.Delete();
                dbFileEncrypt.Rename(dbFile.Name);
            }
        }

        public void Build(IBuildContext context)
        {
            var copyDir = context.Disk.Directory(context.NoBuildPath, PathTypes.Absolute);
            if (copyDir.Exists())
            {
                copyDir.CopyTo(context.ReleasePath);
            }

            var filter = new List<string>() { ".meta", ".DS_Store" };
            var noBuildFiles = new List<string>();
            copyDir.Walk((file) =>
            {
                if (!filter.Contains(file.Extension))
                {
                    noBuildFiles.Add(AssetBundlesMaker.StandardPath(file.FullName.Substring(context.NoBuildPath.Length).Trim(Path.AltDirectorySeparatorChar, '\\')));
                }
            }, SearchOption.AllDirectories);

            var releaseFiles = context.ReleaseFiles == null ? new List<string>() : new List<string>(context.ReleaseFiles);
            releaseFiles.AddRange(noBuildFiles);

            if (context.IsConfigCrypt)
            {
                EncryptFile(context, "key.ini");
                EncryptFile(context, "hosts.ini");
            }

            context.ReleaseFiles = releaseFiles.ToArray();

            UnityEngine.Debug.Log("### Generate Config's Files Success ###");
        }
    }
}