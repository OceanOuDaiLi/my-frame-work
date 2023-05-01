

using System;
using Core.Interface.Network;

namespace Core.Network
{
    /// <summary>
    /// 响应参数
    /// </summary>
    public class PackageResponseEventArgs : EventArgs, IPackageResponse
    {
        /// <summary>
        /// 响应的数据包
        /// </summary>
        public IPackage Response { get; protected set; }

        /// <summary>
        /// 构建一个响应参数
        /// </summary>
        /// <param name="package">数据包</param>
        public PackageResponseEventArgs(IPackage package)
        {
            Response = package;
        }
    }
}