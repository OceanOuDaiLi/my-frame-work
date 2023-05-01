using UnityEngine;

namespace MoreFun.NextEffect.GPUAnim
{
    public class GPUAnimPlayer : MOBABaseFX
    {
        private MeshRenderer thisMR;
        private Material sharedMaterial;
        private Material material;
        private int animPropID;
        private int opacityPropID;

        public float inScale = 0.3f;
        public float outScale = 0.7f;
        public float fadeInScale = 0.0f;
        public float fadeOutScale = 1.0f;
        public float speed;

        private enum STATUS                     // 状态枚举
        {
            AWAIT,                          // 待命状态
            IN,                             // 淡入状态
            HOLD,                           // 稳定状态
            OUT                             // 淡出状态
        }
        private STATUS state = STATUS.AWAIT;    // 当前状态
        private float timer;

        public override void Init()
        {
            // 功能有效性检查
            if (bInitialized == true) return;
            thisMR = GetComponent<MeshRenderer>();
            if (thisMR == null) return;
            sharedMaterial = thisMR.sharedMaterial;
            if (sharedMaterial == null) return;
            if (sharedMaterial.IsKeywordEnabled("_GA_CYCLE")) return;
            if (!sharedMaterial.HasProperty("_AnimProgress")) return;
            if (!sharedMaterial.HasProperty("_Opacity")) return;
            // Base初始化
            ifCulling = true;
            base.Init();
            // 初始化
            animPropID = Shader.PropertyToID("_AnimProgress");
            opacityPropID = Shader.PropertyToID("_Opacity");
            thisMR.enabled = false;
            material = thisMR.material;
            state = STATUS.AWAIT;
        }

        public override void Play()
        {
            if (bInitialized == false)
            {
#if UNITY_EDITOR && !EDITOR_PROFILE
                Debug.LogError("GPUAnimPlayer " + this.name + " 未被初始化");
#endif
                return;
            }
            thisMR.enabled = true;
            timer = 0.0f;
            state = STATUS.IN;
            base.Play();
        }

        public override void UpdateMe()
        {
            switch (state)
            {
                case STATUS.IN:
                    timer += Time.deltaTime * speed;
                    material.SetFloat(animPropID, timer);
                    material.SetFloat(opacityPropID, Mathf.Min(timer / fadeInScale, 1.0f));
                    if (timer > inScale)
                    {
                        state = STATUS.HOLD;
                    }
                    break;
                case STATUS.HOLD:
                    timer += Time.deltaTime * speed;
                    material.SetFloat(animPropID, timer);
                    if (timer > outScale)
                    {
                        timer -= outScale - inScale;
                    }
                    break;
                case STATUS.OUT:
                    timer += Time.deltaTime * speed;
                    material.SetFloat(animPropID, timer);
                    material.SetFloat(opacityPropID, Mathf.Min((1.0f - timer) / (1.0f - fadeOutScale), 1.0f));
                    if (timer > 1.0f)
                    {
                        Stop(true);
                    }
                    break;
            }
        }

        public override void Stop(bool immidiate)
        {
            if (bInitialized == false)
            {
#if UNITY_EDITOR && !EDITOR_PROFILE
                Debug.LogError("GPUAnimPlayer " + this.name + " 未被初始化");
#endif
                return;
            }
            if (state == STATUS.AWAIT) return;
            if (immidiate == true)
            {
                thisMR.enabled = false;
                state = STATUS.AWAIT;
                base.Stop(true);
            }
            else
            {
                state = STATUS.OUT;
                base.Stop(false);
            }
        }

#if UNITY_EDITOR
        public override void PreSaveRenew()
        {
            base.PreSaveRenew();
            if (material != null)
            {
                thisMR.sharedMaterial = sharedMaterial;
                material = null;
            }
        }
#endif
    }
}