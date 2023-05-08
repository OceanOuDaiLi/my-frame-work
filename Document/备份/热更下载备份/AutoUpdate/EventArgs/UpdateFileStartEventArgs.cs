
using System;

namespace Core.AutoUpdate
{
    /// <summary>
    /// 文件启动更新前事件
    /// </summary>
    public sealed class UpdateFileStartEventArgs : EventArgs
    {
        /// <summary>
        /// 需要更新的文件列表
        /// </summary>
        public UpdateFile UpdateFile { get; private set; }

        /// <summary>
        /// 创建一个文件启动更新事件
        /// </summary>
        /// <param name="list"></param>
        public UpdateFileStartEventArgs(UpdateFile updateFile)
        {
            UpdateFile = updateFile;
        }
    }
}