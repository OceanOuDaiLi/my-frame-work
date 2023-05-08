using System.Collections;

namespace FrameWork.Launch
{
    public partial class HotLaunch
    {
        private bool getResUrl = false;
        private bool getUrlsDown = false;

        private bool VersionCompare()
        {
            LogProgress("version check pass : " + _localVer + " >= " + _serverVer);
            return float.Parse(_localVer) >= float.Parse(_serverVer);
        }

        private bool GetLocalVersion()
        {
            _remoteFile = IOHelper.LoadINIFile(remoteFileName);
            if (!_remoteFile.Exists) { return false; }

            return true;
        }

        private IEnumerator GetUrls()
        {
            //1. Get cdn url.
            _cdnUrl = System.Text.Encoding.UTF8.GetString(_remoteFile.Read());

            string resHost = string.Format("{0}/{1}/HybridCLR.txt", _cdnUrl, IOHelper.PlatformToName());
            yield return RequestGet(resHost, ResponseGetClrResHost);

            while (!getResUrl)
            {
                yield return null;
            }

            //4. Get urls. 
            string root = string.Format("{0}/{1}/{2}", _cdnUrl, IOHelper.PlatformToName(), _resUrl);
            //-> aotdll url.
            _aotDllsUrl = string.Format("{0}/{1}.bytes", root, aotFileName);
            //-> hotdll url.
            _hotDllsUrl = string.Format("{0}/{1}.bytes", root, hotFileName);


            getUrlsDown = true;
            LogProgress("ClrResHost Got... -> _aotDllsUrl - > " + _aotDllsUrl);
        }

        private IEnumerator GetServerVersion()
        {
            //if (!string.IsNullOrEmpty(_resUrl))
            //{
            //    yield return RequestGet(_clrVersionUrl, ResponseGetServerVersion);
            //}
            //else
            //{
            //    //无需热更
            //    _serverVer = "0";
            //    _gotServerVersion = true;
            //}

            yield return null;
        }

    }
}