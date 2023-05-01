using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace TechArtist.QualitySetting
{
    public class DynamicResolutionSetting
    {
        //动态分辨率RenderScale
        private float _dynamicScale = 1;

        //设备启动时原始分辨率
        private static bool _inited = false;
        private static int _rawWidth = 1920;
        private static int _rawHeight = 1080;

        //控制场景的分辨率。实际等于：_commonScale * renderScale
        private static float _rawRenderScale = 1.0f;
        private static float _mediumRenderScale = 0.88f;
        private static float _lowRenderScale = 0.75f;
        private static float _lowestRenderScale = 0.64f;

        //private static float _lowestRenderScale = 0.70f;    //0.79*0.70=0.5530，1080*0.5530=597

        //控制UI和场景的统一分辨率。为避免UI变糊，UI分辨率和场景分开，同时限制下最低分辨率，避免低分辨率设备太糊。
        private static float _commonScale = 0.79f;
        private static int _minHeightHigh = 1080;
        private static int _minHeightMedium = 900;
        private static int _minHeightLow = 720;
        private static int _minHeightLowest = 640;

        public void Init(ModelQuality quality)
        {
            if (!_inited)
            {
                _inited = true;

                _rawWidth = Screen.currentResolution.width;
                _rawHeight = Screen.currentResolution.height;

                SetRenderLevel(quality);
                Debug.Log($"Screen resolution Init: {_rawWidth}, {_rawHeight}");
            }
        }

        public void UpdateQuality(ModelQuality quality, bool decrease)
        {
            if (decrease)
            {
                _dynamicScale = Mathf.Clamp(_dynamicScale - 0.05f, 0.8f, 1.0f);
                return;
            }
            _dynamicScale = Mathf.Clamp(_dynamicScale + 0.05f, 0.8f, 1.0f);

            switch (quality)
            {
                case ModelQuality.QUALITY_HIGH:
                    SetRenderScale(_rawRenderScale * _dynamicScale);
                    break;
                case ModelQuality.QUALITY_MEDIUM:
                    SetRenderScale(_mediumRenderScale * _dynamicScale);
                    break;
                case ModelQuality.QUALITY_LOW:
                    SetRenderScale(_lowRenderScale * _dynamicScale);
                    break;
                case ModelQuality.QUALITY_LOWEST:
                    SetRenderScale(_lowestRenderScale * _dynamicScale);
                    break;
                default:
                    SetRenderScale(_mediumRenderScale * _dynamicScale);
                    break;
            }
        }

        public void SetRenderLevel(ModelQuality quality)
        {
            switch (quality)
            {
                case ModelQuality.QUALITY_HIGH:
                    SetScreenResolution(_minHeightHigh);
                    SetRenderScale(_rawRenderScale);
                    DynamicResolutionManager.ExpectTargetFPS = 59.5f;
                    break;
                case ModelQuality.QUALITY_MEDIUM:
                    SetScreenResolution(_minHeightMedium);
                    SetRenderScale(_mediumRenderScale);
                    DynamicResolutionManager.ExpectTargetFPS = 29.5f;
                    break;
                case ModelQuality.QUALITY_LOW:
                    SetScreenResolution(_minHeightLow);
                    SetRenderScale(_lowRenderScale);
                    DynamicResolutionManager.ExpectTargetFPS = 29.5f;
                    break;
                case ModelQuality.QUALITY_LOWEST:
                    SetScreenResolution(_minHeightLowest);
                    SetRenderScale(_lowestRenderScale);
                    DynamicResolutionManager.ExpectTargetFPS = 29.5f;
                    break;
                default:
                    SetScreenResolution(_minHeightMedium);
                    SetRenderScale(_mediumRenderScale);
                    DynamicResolutionManager.ExpectTargetFPS = 29.5f;
                    break;
            }
        }

        public void SetRenderScale(float scale)
        {
            var universalRenderPipelineAsset = UniversalRenderPipeline.asset;
            universalRenderPipelineAsset.renderScale = scale;

#if UNITY_EDITOR
            scale = 2;
            UniversalRenderPipeline.asset.renderScale = 2;
#endif
            Debug.Log($"Screen resolution RenderScale: {scale}");
        }

        //width为-1时，则会按照设备分辨率的ratio来计算得到
        public void SetScreenResolution(int height = 1080, int width = -1)
        {
            Debug.Log($"Screen resolution before: {Screen.currentResolution.width}, {Screen.currentResolution.height}");

            //不要超过设备自身分辨率
            if (height > _rawHeight)
            {
                height = _rawHeight;
            }

            if (width == -1)
            {
                float ratio = (float)_rawWidth / (float)_rawHeight;
                width = (int)(height * ratio);
            }
            Screen.SetResolution(width, height, true);

            Debug.Log($"Screen resolution set: {width}, {height}");

            //Todo:
            //IPHONE 刘海屏适配动态分辨率
            //Example Code:

            /*
            Invoke.DelayTime(0.5f, time =>
            {
                #if UNITY_IPHONE

                            iPhoneX（2436x1125px）横屏时，Screen.safeArea返回的值为Rect（132，63，2172，1062）。如图：
                            Rect safeArea = Screen.safeArea;
                            var left = safeArea.xMin;
                            var right = Screen.width - safeArea.xMax;
                            var notchIos = System.Math.Max(left, right) - 24;
                            _paddingPerX = (notchIos - _uiPaddingX) / Screen.width;
                            _paddingPerX = Mathf.Max(0, _paddingPerX);
                            AnchorMin = new Vector2(_paddingPerX, 0);
                            AnchorMax = new Vector2(1 - _paddingPerX, 1);

                #endif
            });
             */
        }
    }
}