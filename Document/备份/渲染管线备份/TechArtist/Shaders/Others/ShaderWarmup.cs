using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class ShaderWarmup : MonoBehaviour
{
    [SerializeField]
    public List<SerializeShaderVariant> ShaderVariants;

    private int _warmupVariantsCount = 0;
    private ShaderVariantCollection _svc;
    private ShaderVariantCollection.ShaderVariant _sv;
    private const int WarmupNumPerframe = 1;
    private float _totalTime = 0;

    private void Start()
    {
        _svc = new ShaderVariantCollection();
        _sv = new ShaderVariantCollection.ShaderVariant();
    }

    private void Update()
    {
        UnityEngine.Profiling.Profiler.BeginSample("Shader Warmup");
        float time = Time.realtimeSinceStartup;
        _svc.Clear();
        int length = Mathf.Min(ShaderVariants.Count, _warmupVariantsCount + WarmupNumPerframe);
        for (int i = _warmupVariantsCount; i < length; ++i)
        {
            var variant = ShaderVariants[i];
            _sv.shader = variant.Shader;
            _sv.passType = variant.PassType;
            _sv.keywords = variant.keywords;
            _svc.Add(_sv);
        }

        _warmupVariantsCount += WarmupNumPerframe;
        _svc.WarmUp();
        _totalTime += Time.realtimeSinceStartup - time;
        UnityEngine.Profiling.Profiler.EndSample();

        if (_warmupVariantsCount >= ShaderVariants.Count - 1)
        {
            Debug.LogError("warm up : " + _totalTime);
            GameObject.Destroy(this);
        }
    }
}

[System.Serializable]
public struct SerializeShaderVariant
{
    public Shader Shader;
    public PassType PassType;
    public string[] keywords;

    public SerializeShaderVariant(ShaderVariantCollection.ShaderVariant sv)
    {
        Shader = sv.shader;
        PassType = sv.passType;
        keywords = sv.keywords;
    }
}
