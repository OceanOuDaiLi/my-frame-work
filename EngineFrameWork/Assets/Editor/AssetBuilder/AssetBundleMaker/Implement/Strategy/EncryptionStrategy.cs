using FrameWork;
using Core.Interface.IO;
using Core.Interface.AssetBuilder;
using Core.Crypt;
using SevenZip.Buffer;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2018 ~ 2023
	Filename: 	EncryptionStrategy.cs
	Author:		DaiLi.Ou
	Descriptions: AssetBundleMaker Encryption Strategy -> AssetBundle Encrypt.
*********************************************************************/
namespace Core.AssetBuilder
{
    public class EncryptionStrategy : IBuildStrategy
    {
        public BuildProcess Process => BuildProcess.Encryption;

        public void Build(IBuildContext context)
        {
            if (context.IsAssetCrypt == false && context.IsCodeCrypt == false) return;

            IDisk encryptAESDisk = AssetBundlesMaker.IO.Disk(context.ReleasePath, App.AESCrypt);
            IDisk encryptTEADisk = AssetBundlesMaker.IO.Disk(context.ReleasePath, App.TEACrypt);

            string[] releaseFiles = context.ReleaseFiles;
            IFile releaseFile = null;
            IFile encryptFile = null;
            for (int i = 0; i < releaseFiles.Length; i++)
            {
                // 针对游戏资产AB加密
                bool isCodeAsset = releaseFiles[i].Contains("aotdlls") || releaseFiles[i].Contains("hotupdatedlls");
                releaseFile = context.Disk.File(releaseFiles[i], PathTypes.Relative);
                if (context.IsAssetCrypt && !isCodeAsset)
                {
                    encryptFile = encryptAESDisk.File(releaseFiles[i] + ".tmp", PathTypes.Relative);
                }

                // 针对代码资产AB加密
                if (context.IsCodeCrypt && isCodeAsset)
                {
                    encryptFile = encryptTEADisk.File(releaseFiles[i] + ".tmp", PathTypes.Relative);
                }

                byte[] data = releaseFile.Read();
                encryptFile.Create(data);

                releaseFile.Delete();
                encryptFile.Rename(releaseFile.Name);
            }

            UnityEngine.Debug.Log("### Encrypt Assets Success ###");
        }
    }
}
