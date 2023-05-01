/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2018 ~ 2023
	Filename: 	BuildProcess.cs
	Author:		DaiLi.Ou
	Descriptions: AssetBundle Build Process PipeLine.
*********************************************************************/
namespace Core.Interface.AssetBuilder
{
    public enum BuildProcess
    {
        Setup = 1,

        Clear = 10,

        Precompiled = 20,

        Build = 30,

        Scanning = 40,

        Encryption = 50,

        GenTable = 60,

        GenPath = 70,

        SplitBundle = 80,

        ZipBundle = 90,

        GenVersion = 100,

        Complete = 110,
    }
}
