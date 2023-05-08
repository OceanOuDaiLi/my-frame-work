
using System;

namespace Core.AutoUpdate
{
    /// <summary>
    /// 更新文件字段
    /// </summary>
    public sealed class UpdateFileField
    {
        /// <summary>
        /// 文件MD5
        /// </summary>
        public string MD5 { get; private set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public long Size { get; private set; }

        /// <summary>
        /// 构建一个更新文件字段
        /// </summary>
        /// <param name="str">字段数据</param>
        public UpdateFileField(string str)
        {
            var lst = str.Split('\t');

            if (lst.Length < 3)
            {
                throw new Exception("update data field data error");
            }

            Path = lst[0];
            MD5 = lst[1];
            Size = long.Parse(lst[2]);
        }

        /// <summary>
        /// 构建一个更新文件字段
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="md5">文件md5</param>
        /// <param name="size">文件大小</param>
        public UpdateFileField(string path, string md5, long size)
        {
            Path = path;
            MD5 = md5;
            Size = size;
        }

        /// <summary>
        /// 将更新文件字段转为string格式
        /// </summary>
        /// <returns>string化的更新文件字段</returns>
        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}\n", Path, MD5, Size);
        }
    }
}