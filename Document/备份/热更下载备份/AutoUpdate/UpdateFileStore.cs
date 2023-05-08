using System.IO;
using FrameWork;
using System.Text;
using Core.Interface.IO;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com

	Created:	2018 ~ 2023
	Filename: 	UpdateFileStore.cs
	Author:		DaiLi.Ou

	Descriptions: Update Files' Container.
*********************************************************************/
namespace Core.AutoUpdate
{
    public sealed class UpdateFileStore
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public const string FILE_NAME = "update-list.ini";

        /// <summary>
        /// 磁盘
        /// </summary>
        private IDisk disk;

        /// <summary>
        /// 磁盘
        /// </summary>
        private IDisk Disk
        {
            get { return disk ?? (disk = App.AssetDisk); }
        }

        /// <summary>
        /// 从字节流加载更新文件
        /// </summary>
        /// <param name="bytes">字节流</param>
        /// <returns>更新文件数据</returns>
        public UpdateFile LoadFromBytes(byte[] bytes)
        {
            var file = new UpdateFile();
            file.Parse(new UTF8Encoding(false).GetString(bytes));
            return file;
        }

        /// <summary>
        /// 获取UpdateList文件
        /// </summary>
        /// <param name="path">存储路径(绝对路径)</param>
        /// <returns></returns>
        public IFile GetFile(string path)
        {
            return Disk.File(path + Path.AltDirectorySeparatorChar + FILE_NAME, PathTypes.Absolute);
        }

        /// <summary>
        /// 从文件加载更新文件
        /// </summary>
        /// <param name="path">文件路径(绝对路径)</param>
        /// <returns>更新文件数据</returns>
        public UpdateFile LoadFromPath(string path)
        {
            var file = GetFile(path);
            if (file.Exists) return LoadFromBytes(file.Read());
            else return null;
        }

        /// <summary>
        /// 保存一个更新文件
        /// </summary>
        /// <param name="path">存储路径(绝对路径)</param>
        /// <param name="updateFile">更新文件数据</param>
        public void Save(string path, UpdateFile updateFile)
        {
            var file = GetFile(path);
            file.Delete();
            file.Create(Encoding.UTF8.GetBytes(updateFile.Data));
        }

        /// <summary>
        /// 追加一个文件片段
        /// </summary>
        /// <param name="path">存储路径(绝对路径)</param>
        /// <param name="updateField">文件片段</param>
        public void Append(string path, UpdateFileField updateField)
        {
            var file = GetFile(path);
            Append(file, updateField);
        }

        /// <summary>
        /// 追加一个文件片段
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="updateField">文件片段</param>
        public void Append(IFile file, UpdateFileField updateField)
        {
            file.Append(Encoding.UTF8.GetBytes(updateField.ToString()));
        }
    }
}
