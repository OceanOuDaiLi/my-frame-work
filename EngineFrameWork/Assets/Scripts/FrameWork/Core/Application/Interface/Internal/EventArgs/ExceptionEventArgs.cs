

namespace Core.Interface
{
    /// <summary>
    /// 异常事件
    /// </summary>
    public class ExceptionEventArgs : System.EventArgs
    {
        /// <summary>
        /// 异常
        /// </summary>
        public System.Exception Exception { get; protected set; }

        public ExceptionEventArgs(System.Exception ex)
        {
            Exception = ex;
        }
    }
}