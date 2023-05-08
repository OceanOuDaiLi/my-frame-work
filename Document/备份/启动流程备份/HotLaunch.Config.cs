namespace FrameWork.Launch
{
    public partial class HotLaunch
    {
        private const string aotFileName = "aotdlls";                             // hot dlls name.
        private const string hotFileName = "hotupdatedlls";                       // aot dlls name.
        private const string remoteFileName = "RemoteUrl.txt";                    // hosts adress of cdn.


        private string _cdnUrl;                                                   // http request url.
        private string _resUrl;                                                   // http request res url.
        private string _hotDllsUrl;                                               // hot dlls download url.
        private string _aotDllsUrl;                                               // aot dlls download url.

        private string _localVer;                                                 // clr hot fix local version number.      
        private string _serverVer;                                                // clr hot fix server version number.

        public struct UpdateInfo
        {
            public string fileMd5 { get; set; }
            public string fileName { get; set; }
            public int fileByteSize { get; set; }
        }
    }
}