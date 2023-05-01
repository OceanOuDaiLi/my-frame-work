﻿using System.IO;
using Core.Interface.IO;

namespace Core.IO
{
    /// <summary>
    /// 本地驱动
    /// </summary>
    public sealed class LocalDisk : IDisk
    {
        /// <summary>
        /// 文件加密组件
        /// </summary>
        public IIOCrypt IOCrypt { get; set; }

        /// <summary>
        /// 磁盘路径
        /// </summary>
        private string path;

        /// <summary>
        /// 无效的文件字符
        /// </summary>
        private static readonly char[] INVALID_FILE_NAME_CHARS = new char[] { '/', '\\', '<', '>', ':', '|', '"' };

        /// <summary>
        /// 构建一个根目录为root的磁盘
        /// </summary>
        /// <param name="root">根目录</param>
        /// <param name="ioCrypt">加密组件</param>
        public LocalDisk(string root, IIOCrypt ioCrypt = null)
        {
            path = root;
            IOCrypt = ioCrypt;
        }

        /// <summary>
        /// 磁盘是否是被加密的
        /// </summary>
        public bool IsCrypt
        {
            get { return IOCrypt != null; }
        }

        /// <summary>
        /// 是否是无效的文件名
        /// </summary>
        /// <param name="name">文件名</param>
        /// <returns>是否无效</returns>
        public static bool IsValidFileName(string name)
        {
            for (var i = 0; i < INVALID_FILE_NAME_CHARS.Length; i++)
            {
                if (name.IndexOf(INVALID_FILE_NAME_CHARS[i]) != -1)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 验证是否是无效的路径
        /// </summary>
        /// <param name="path">路径</param>
        public static void GuardValidatePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new IOException("a path can not be null or empty when searching the project");
            }

            if (path[path.Length - 1] == Path.AltDirectorySeparatorChar)
            {
                throw new IOException("all directory paths are expected to not end with a leading slash. ( i.e. the '" + Path.AltDirectorySeparatorChar + "' character )");
            }
        }

        /// <summary>
        /// 标准化路径
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>标准化后的路径</returns>
        public static string NormalizePath(string path)
        {
            if (path[0] != Path.AltDirectorySeparatorChar)
            {
                return Path.AltDirectorySeparatorChar + path;
            }
            return path;
        }

        /// <summary>
        /// 获取磁盘中的一个文件
        /// </summary>
        /// <param name="path">文件地址</param>
        /// <param name="pathType">路径类型</param>
        /// <returns>文件</returns>
        public IFile File(string path, PathTypes pathType = PathTypes.Relative)
        {
            if (pathType == PathTypes.Absolute)
            {
                return new File(path, this);
            }
            else
            {
                return new File(this.path + Path.AltDirectorySeparatorChar + path.Trim(Path.AltDirectorySeparatorChar), this);
            }
        }

        /// <summary>
        /// 获取磁盘中的文件夹
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <param name="pathType">路径类型</param>
        /// <returns>文件夹</returns>
        public IDirectory Directory(string path, PathTypes pathType = PathTypes.Relative)
        {
            if (pathType == PathTypes.Absolute)
            {
                return new Directory(path, this);
            }
            else
            {
                return new Directory(this.path + Path.AltDirectorySeparatorChar + path.Trim(Path.AltDirectorySeparatorChar), this);
            }
        }

        /// <summary>
        /// 获取磁盘文件夹
        /// </summary>
        /// <returns></returns>
        public IDirectory Directory()
        {
            return new Directory(this.path, this);
        }

        /// <summary>
        /// 获取默认根路径
        /// </summary>
        public IDirectory Root
        {
            get { return new Directory(path, this); }
        }
    }
}