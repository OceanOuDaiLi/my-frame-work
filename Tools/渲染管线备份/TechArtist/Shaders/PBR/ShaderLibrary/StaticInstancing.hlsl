#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
    uint _StaticInstanceIndexBase;

    UNITY_INSTANCING_CBUFFER_SCOPE_BEGIN(_StaticInstanceIndex)
        int4 _InstanceIndex[1024];
    UNITY_INSTANCING_CBUFFER_SCOPE_END

    UNITY_INSTANCING_CBUFFER_SCOPE_BEGIN(_StaticInstanceData)
        float4 _InstanceData[1024];
    UNITY_INSTANCING_CBUFFER_SCOPE_END

    void setup()
    {
        uint k = _StaticInstanceIndexBase + unity_InstanceID;
        int i = _InstanceIndex[k / 4][k % 4] * 4;
        
        unity_LightmapST = _InstanceData[i];
        unity_ObjectToWorld = float4x4(
            _InstanceData[i + 1],
            _InstanceData[i + 2],
            _InstanceData[i + 3],
            float4(0, 0, 0, 1));
    }
#endif