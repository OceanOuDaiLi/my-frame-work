using System.IO;
using Core.Interface.IO;
using System.Collections.Generic;
using Core.Interface.AssetBuilder;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2018 ~ 2023
	Filename: 	ScanningStrategy.cs
	Author:		DaiLi.Ou
	Descriptions: AssetBundleMaker Scanning Strategy -> Flite .mate/.de_store files. recodeing bundle files.
*********************************************************************/
namespace Core.AssetBuilder
{
    public sealed class ScanningStrategy : IBuildStrategy
    {
        public BuildProcess Process => BuildProcess.Scanning;

        public void Build(IBuildContext context)
        {
            var filter = new List<string>() { ".meta", ".DS_Store" };
            var releaseDir = context.Disk.Directory(context.ReleasePath, PathTypes.Absolute);
            var releaseFile = new List<string>();
            releaseDir.Walk((file) =>
            {
                if (!filter.Contains(file.Extension))
                {
                    releaseFile.Add(AssetBundlesMaker.StandardPath(file.FullName.Substring(context.ReleasePath.Length).Trim(Path.AltDirectorySeparatorChar, '\\')));
                }
            }, SearchOption.AllDirectories);

            context.ReleaseFiles = releaseFile.ToArray();
        }
    }
}