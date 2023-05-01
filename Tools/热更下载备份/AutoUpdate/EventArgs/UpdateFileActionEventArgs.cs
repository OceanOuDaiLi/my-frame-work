using System;
using UnityEngine.Networking;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com

	Created:	2018 ~ 2023
	Filename: 	UpdateFileActionEventArgs.cs
	Author:		DaiLi.Ou

	Descriptions: Asset Update Action.
*********************************************************************/
namespace Core.AutoUpdate
{
    public sealed class UpdateFileActionEventArgs : EventArgs
    {
        /// <summary>
        /// 文件请求对象
        /// </summary>
        public UnityWebRequestAsyncOperation WebRequestAsyncOp { get; set; }

        /// <summary>
        /// 文件更新路径
        /// </summary>
        public UpdateFileField UpdateFileField { get; set; }

        /// <summary>
        /// 创建一个自动更新文件更新事件
        /// </summary>
        /// <param name="filePath">文件更新路径</param>
        /// <param name="request">文件请求对象</param>
        public UpdateFileActionEventArgs(UpdateFileField updateFileField, UnityWebRequestAsyncOperation webRequestAsyncOp)
        {
            WebRequestAsyncOp = webRequestAsyncOp;
            UpdateFileField = updateFileField;
        }
    }
}