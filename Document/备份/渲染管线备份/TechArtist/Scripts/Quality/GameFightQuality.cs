
using UnityEngine;
using TechArtist.BlitBloom;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TechArtist.QualitySetting
{
    public class GameFightQuality : MonoBehaviour
    {
        public Volume volume;
        public static GameFightQuality Instance;

        private ModelQuality modelQuality;
        private ShaderQuality shaderQuality;
        private RenderFeatureQuality renderFeatureQuality;
        private DynamicResolutionSetting dynamicResolutionSetting;

        private ModelAnalysisMgr analysisMgr = null;

        #region Private Methods

        private void GetModelQuality()
        {
            //modelQuality
            if (analysisMgr.IsHighEndModels())
            {
                modelQuality = ModelQuality.QUALITY_HIGH;
            }
            else if (analysisMgr.IsMidEndModels())
            {
                modelQuality = ModelQuality.QUALITY_MEDIUM;
            }
            else if (analysisMgr.IsLowEndModels())
            {
                modelQuality = ModelQuality.QUALITY_LOWEST;
            }
            else
            {
                modelQuality = ModelQuality.QUALITY_LOWEST;
            }

            Debug.Log("---------------------- 当前机型品质 ---------------------- ：" + (int)modelQuality);
#if UNITY_EDITOR
            modelQuality = ModelQuality.QUALITY_HIGH;
#endif
        }

        private void SetdynamicResolutionQuality()
        {
            //dynamicResolutionSetting.Init(modelQuality);  //Command Dynamic Resolution.

            //Start DynamicResolution Render Feature.
            //DynamicResolutionManager.EnterFight = true;   //Command Dynamic Resolution.  
        }

        private void SetShaderQuality()
        {
            //Set SSR by Quality.
            shaderQuality.RenderSSR((modelQuality));

            //Set Shaders' KeyWorld of Quality.
            shaderQuality.SetShaderQuality((int)modelQuality);
        }

        private void SetRenderFeatureToggle()
        {
            //Set Post-Processing Quality
            renderFeatureQuality.SetRenderFeatureToggle(modelQuality);
        }

        private void SetLoadBuff()
        {
            //UpGrade Profiler - PrefabManager - Loading efficiency
            QualitySettings.asyncUploadTimeSlice = 8;
            QualitySettings.asyncUploadBufferSize = 64;
            Application.backgroundLoadingPriority = ThreadPriority.High;
        }

        private void SetPhysicalClothQuality()
        {
            switch (modelQuality)
            {
                case ModelQuality.QUALITY_HIGH:
                case ModelQuality.QUALITY_MEDIUM:
                    AI.Physics.PhysicsRim.physicalClothQuality = AI.Physics.PhysicsClothQuality.High;
                    break;
                case ModelQuality.QUALITY_LOW:
                case ModelQuality.QUALITY_LOWEST:
                default:
                    AI.Physics.PhysicsRim.physicalClothQuality = AI.Physics.PhysicsClothQuality.Low;
                    break;
            }
        }

        public void UpdateDynamicResolution(bool decrease)
        {
            dynamicResolutionSetting.UpdateQuality(modelQuality, decrease);
        }

        #endregion

        #region Unity Calls
        private void Awake()
        {
            Instance = this;

            analysisMgr = ModelAnalysisMgr.Instance;

            shaderQuality = new ShaderQuality();
            renderFeatureQuality = new RenderFeatureQuality(volume);
            dynamicResolutionSetting = new DynamicResolutionSetting();

            GetModelQuality();

            SetShaderQuality();

            SetdynamicResolutionQuality();

            SetRenderFeatureToggle();

            SetLoadBuff();

            SetPhysicalClothQuality();
        }

        private void OnDestroy()
        {
            Instance = null;

            //ReSet Render Scale by Quality
            shaderQuality.RenderScale(1);
            shaderQuality.SetShaderQuality((int)modelQuality);

            //DynamicResolutionManager.ExitGameFight();         //Command Dynamic Resolution

            QualitySettings.asyncUploadTimeSlice = 2;
            QualitySettings.asyncUploadBufferSize = 4;
            Application.backgroundLoadingPriority = ThreadPriority.Normal;

            shaderQuality = null;

            renderFeatureQuality.OnDispose();
            renderFeatureQuality = null;

            System.GC.Collect();
        }
        #endregion
    }

    public class ShaderQuality
    {
        private EMaterialQuality _currQuality = EMaterialQuality.QUALITY_HIGH;

        public void SetShaderQuality(int value)
        {
            EMaterialQuality quality;
            switch (value)
            {
                case 0:
                    quality = EMaterialQuality.QUALITY_HIGH;
                    break;
                case 1:
                    quality = EMaterialQuality.QUALITY_MEDIUM;
                    break;
                case 2:
                    quality = EMaterialQuality.QUALITY_LOW;
                    break;
                case 3:
                    quality = EMaterialQuality.QUALITY_LOWEST;
                    break;
                default:
                    quality = EMaterialQuality.QUALITY_HIGH;
                    break;
            }
            SetMaterialQuality(quality);
        }

        public void SetMaterialQuality(EMaterialQuality quality)
        {

            switch (_currQuality)
            {
                case EMaterialQuality.QUALITY_HIGH:
                    Shader.DisableKeyword(ShaderQualityKeywords.QUALITY_HIGH);
                    break;
                case EMaterialQuality.QUALITY_MEDIUM:
                    Shader.DisableKeyword(ShaderQualityKeywords.QUALITY_MEDIUM);
                    break;
                case EMaterialQuality.QUALITY_LOW:
                    Shader.DisableKeyword(ShaderQualityKeywords.QUALITY_LOW);
                    break;
                case EMaterialQuality.QUALITY_LOWEST:
                    Shader.DisableKeyword(ShaderQualityKeywords.QUALITY_LOWEST);
                    break;
                default:
                    Shader.DisableKeyword(ShaderQualityKeywords.QUALITY_LOW);
                    break;
            }

            _currQuality = quality;

            switch (_currQuality)
            {
                case EMaterialQuality.QUALITY_HIGH:
                    Shader.EnableKeyword(ShaderQualityKeywords.QUALITY_HIGH);
                    break;
                case EMaterialQuality.QUALITY_MEDIUM:
                    Shader.EnableKeyword(ShaderQualityKeywords.QUALITY_MEDIUM);
                    break;
                case EMaterialQuality.QUALITY_LOW:
                    Shader.EnableKeyword(ShaderQualityKeywords.QUALITY_LOW);
                    break;
                case EMaterialQuality.QUALITY_LOWEST:
                    Shader.EnableKeyword(ShaderQualityKeywords.QUALITY_LOWEST);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 场景动态分辨率 / SSR 设置
        /// 开启SSR，意味着关闭场景动态分辨率。
        /// 关闭SSR，意味着开启场景动态分辨率。
        /// </summary>
        /// <param name="quality"></param>
        public void RenderSSR(ModelQuality quality)
        {
            bool showSSR = false;
            switch (quality)
            {
                case ModelQuality.QUALITY_HIGH:
                    showSSR = true;
                    RenderScale(1.0f);
                    break;
                case ModelQuality.QUALITY_MEDIUM:
                    showSSR = true;
                    RenderScale(.75f);
                    break;
                case ModelQuality.QUALITY_LOW:
                case ModelQuality.QUALITY_LOWEST:
                    showSSR = false;
                    RenderScale(.75f);
                    break;
                default:
                    break;
            }

            Debug.Log("---------------------- 当前SSR开启状态 ---------------------- ：" + showSSR.ToString());
            var universalRenderPipelineAsset = UniversalRenderPipeline.asset;
            universalRenderPipelineAsset.supportsCameraDepthTexture = showSSR;
        }

        public void RenderScale(float value)
        {
            var universalRenderPipelineAsset = UniversalRenderPipeline.asset;
            // 设置场景分辨率，只影响场景;
            universalRenderPipelineAsset.renderScale = value;
        }
    }

    public class RenderFeatureQuality
    {
        //todo: Volume 根据机型 适配效果。
        VolumeProfile profile;

        public RenderFeatureQuality(Volume volume)
        {
            profile = volume.sharedProfile;
        }

        /// <summary>
        /// 开启/关闭 后处理
        /// </summary>
        /// <param name="_quality"></param>
        public void SetRenderFeatureToggle(ModelQuality _quality)
        {
            switch (_quality)
            {
                case ModelQuality.QUALITY_HIGH:
                    //粒子发光效果
                    SelectiveBloomRenderFeature.active = true;
                    break;
                case ModelQuality.QUALITY_MEDIUM:
                case ModelQuality.QUALITY_LOW:
                case ModelQuality.QUALITY_LOWEST:
                    //粒子发光效果
                    SelectiveBloomRenderFeature.active = false;
                    break;
                default:
                    break;
            }

            Debug.Log("---------------------- 当前BloomRende开启状态 ---------------------- ：" + SelectiveBloomRenderFeature.active.ToString());
        }

        public void OnDispose()
        {
            profile = null;
            SelectiveBloomRenderFeature.active = false;
        }
    }

}