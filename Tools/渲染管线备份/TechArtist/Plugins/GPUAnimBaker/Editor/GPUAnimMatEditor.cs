using UnityEngine;
using UnityEditor;

public class GPUAnimMatEditor : ShaderGUI
{
    // 播放模式相关 Evaluate-评估值模式 Cycle-循环模式 Status-状态机模式
    enum PlayMode
    {
        Evaluate,
        Cycle,
    };
    static readonly string[] PlayModeKeys = new string[]
    {
        "_GA_EVALUATE",
        "_GA_CYCLE",
    };

    static readonly string[] PlayModeLabels = new string[]
    {
        "进度",
        "循环",
    };

    static PlayMode GetPlayMode(Material mat)
    {
        for (int i = 0; i < PlayModeKeys.Length; i++)
        {
            if (mat.IsKeywordEnabled(PlayModeKeys[i]))
            {
                var mode = (PlayMode)i;
                SetPlayMode(mat, mode);
                return mode;
            }
        }
        SetPlayMode(mat, PlayMode.Evaluate);
        return PlayMode.Evaluate;
    }
    static void SetPlayMode(Material mat, PlayMode playMode)
    {
        switch (playMode)
        {
            case PlayMode.Evaluate:
                mat.DisableKeyword(PlayModeKeys[1]);
                break;
            case PlayMode.Cycle:
                mat.EnableKeyword(PlayModeKeys[1]);
                break;
        }
    }
    // 表现模式相关 Normal-一般模式 Ghost-幽灵模式 AfterImage-残影模式
    enum ShowMode
    {
        Normal,
        Ghost,
        AfterImage
    }
    static readonly string[] ShowModeKeys = new string[]
    {
        "_GA_NORMAL",
        "_GA_GHOST",
        "_GA_AFTERIMAGE"
    };
    static readonly string[] ShowModeLabels = new string[]
    {
        "普通",
        "幽灵",
        "残影"
    };
    static ShowMode GetShowMode(Material mat)
    {
        for (int i = 0; i < ShowModeKeys.Length; i++)
        {
            if (mat.IsKeywordEnabled(ShowModeKeys[i]))
            {
                var mode = (ShowMode)i;
                SetShowMode(mat, mode);
                return mode;
            }
        }
        SetShowMode(mat, ShowMode.Normal);
        return ShowMode.Normal;
    }

    static void SetShowMode(Material mat, ShowMode showMode)
    {
        switch (showMode)
        {
            case ShowMode.Normal:
                mat.DisableKeyword(ShowModeKeys[1]);
                mat.DisableKeyword(ShowModeKeys[2]);
                break;

            case ShowMode.Ghost:
                mat.EnableKeyword(ShowModeKeys[1]);
                mat.DisableKeyword(ShowModeKeys[2]);
                break;

            case ShowMode.AfterImage:
                mat.DisableKeyword(ShowModeKeys[1]);
                mat.EnableKeyword(ShowModeKeys[2]);
                break;
        }
    }
    // 混合模式相关 Opaque-不透明 Cutout-透明裁剪 AlphaBlend-透明混合 Add-叠加模式
    enum BlendMode
    {
        Opaque,
        Cutout,
        AlphaBlend,
        Add
    }
    static readonly string[] BlendModeKeys = new string[]
    {
        "_BLEND_OFF",
        "_BLEND_AT",
        "_BLEND_AB",
        "_BLEND_AD"
    };
    static readonly string[] BlendModeLabels = new string[]
    {
        "不透明",
        "透明裁剪",
        "透明混合",
        "叠加混合"
    };
    static BlendMode GetBlendMode(Material mat)
    {
        for (int i = 1; i < BlendModeKeys.Length; i++)
        {
            if (mat.IsKeywordEnabled(BlendModeKeys[i]))
            {
                var mode = (BlendMode)i;
                SetBlendMode(mat, mode);
                return mode;
            }
        }
        SetBlendMode(mat, BlendMode.Opaque);
        return BlendMode.Opaque;
    }
    static void SetBlendMode(Material mat, BlendMode blendMode, bool resetQueue = false)
    {
        switch (blendMode)
        {
            case BlendMode.Opaque:
                mat.SetOverrideTag("RenderType", "Opaque");
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                mat.SetInt("_ZWrite", 1);
                mat.DisableKeyword(BlendModeKeys[1]);
                mat.DisableKeyword(BlendModeKeys[2]);
                mat.DisableKeyword(BlendModeKeys[3]);
                if (resetQueue) mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
                break;
            case BlendMode.Cutout:
                mat.SetOverrideTag("RenderType", "TransparentCutout");
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                mat.SetInt("_ZWrite", 1);
                mat.EnableKeyword(BlendModeKeys[1]);
                mat.DisableKeyword(BlendModeKeys[2]);
                mat.DisableKeyword(BlendModeKeys[3]);
                if (resetQueue) mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                break;
            case BlendMode.AlphaBlend:
                mat.SetOverrideTag("RenderType", "Transparent");
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 1);
                mat.DisableKeyword(BlendModeKeys[1]);
                mat.EnableKeyword(BlendModeKeys[2]);
                mat.DisableKeyword(BlendModeKeys[3]);
                if (resetQueue) mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                break;
            case BlendMode.Add:
                mat.SetOverrideTag("RenderType", "Transparent");
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword(BlendModeKeys[1]);
                mat.DisableKeyword(BlendModeKeys[2]);
                mat.EnableKeyword(BlendModeKeys[3]);
                if (resetQueue) mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                break;
        }
    }
    // 通道遮罩相关
    enum ColorMask
    {
        RGB,
        RGBA
    }
    static readonly string[] ColorMaskKeys = new string[]
{
        "_COLORMASK_RGB",
        "_COLORMASK_RGBA"
    };
    static readonly string[] ColorMaskLabels = new string[]
    {
        "RGB",
        "RGBA"
    };
    static ColorMask GetColorMask(Material mat)
    {
        for (int i = 1; i < ColorMaskKeys.Length; i++)
        {
            if (mat.IsKeywordEnabled(ColorMaskKeys[i]))
            {
                var mode = (ColorMask)i;
                SetColorMask(mat, mode);
                return mode;
            }
        }
        SetColorMask(mat, ColorMask.RGB);
        return ColorMask.RGB;
    }
    static void SetColorMask(Material mat, ColorMask maskMode)
    {
        switch (maskMode)
        {
            case ColorMask.RGB:
                mat.SetInt("_ColorMask", (int)(UnityEngine.Rendering.ColorWriteMask.Red | UnityEngine.Rendering.ColorWriteMask.Green | UnityEngine.Rendering.ColorWriteMask.Blue));
                mat.DisableKeyword(ColorMaskKeys[1]);
                break;
            case ColorMask.RGBA:
                mat.SetInt("_ColorMask", (int)UnityEngine.Rendering.ColorWriteMask.All);
                mat.EnableKeyword(ColorMaskKeys[1]);
                break;
        }
    }
    // RenderQueue相关
    static int GetQueueAddition(Material mat, BlendMode mode)
    {
        var queueValue = mat.renderQueue;
        switch (mode)
        {
            case BlendMode.Opaque:
                return queueValue - (int)UnityEngine.Rendering.RenderQueue.Geometry;
            case BlendMode.Cutout:
                return queueValue - (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
            case BlendMode.AlphaBlend:
                return queueValue - (int)UnityEngine.Rendering.RenderQueue.Transparent;
            case BlendMode.Add:
                return queueValue - (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }
        return queueValue - (int)UnityEngine.Rendering.RenderQueue.Geometry;
    }
    static void SetQueueAddition(Material mat, BlendMode mode, int addition)
    {
        switch (mode)
        {
            case BlendMode.Opaque:
                mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry + addition;
                return;
            case BlendMode.Cutout:
                mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest + addition;
                return;
            case BlendMode.AlphaBlend:
                mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent + addition;
                return;
            case BlendMode.Add:
                mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent + addition;
                return;
        }
    }


    // GUI相关
    static bool defaultGUI;
    static readonly Color barColor = new Color(0.0f, 0.0f, 0.0f, 0.3f);
    static readonly Color darkColor = new Color(0.0f, 0.0f, 0.0f, 0.1f);
    static readonly Color lightColor = new Color(1.0f, 1.0f, 1.0f, 0.1f);

    // DrawGUI
    override public void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        // 材质面板
        if (defaultGUI) base.OnGUI(materialEditor, properties);
        else CustomGUI(materialEditor, properties);
        // 功能按钮
        var rect = GUILayoutUtility.GetRect(100.0f, 22.0f);
        EditorGUI.DrawRect(rect, barColor);
        if (GUI.Button(new Rect(rect.x + 10.0f, rect.y + 3.0f, rect.width * 0.5f - 85.0f, 16.0f), "查看Keywords"))
        {
            var report = "";
            var targetMat = materialEditor.target as Material;
            var keys = targetMat.shaderKeywords;
            foreach(var key in keys)
            {
                report += key + " ";
            }
            Debug.Log(report == "" ? "无激活Key..." : "Key: " + report);
        }
        if (GUI.Button(new Rect(rect.x + rect.width * 0.5f - 65.0f, rect.y + 3.0f, rect.width * 0.5f - 85.0f, 16.0f), defaultGUI ? "切到自定义界面" : "切到默认界面"))
        {
            defaultGUI = !defaultGUI;
        }
        // 广告
        if (GUI.Button(new Rect(rect.x + rect.width - 140.0f, rect.y + 3.0f, 130.0f, 18.0f), " Present by LookingLu", EditorStyles.label))
        {
            // 作者署名 移除联系LookingLu 不可偷换 注意节操
            Application.OpenURL("http://boyantata.com/");
        }
    }

    // 自定义GUI方法
    void CustomGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        var targetMat = materialEditor.target as Material;
        // 设置FieldWidth为64.0f 以适配材质面板图片预览窗口
        EditorGUIUtility.fieldWidth = 64.0f;
        // Logo
        var rect = GUILayoutUtility.GetRect(100.0f, 22.0f);
        EditorGUI.DrawRect(rect, barColor);
        GUI.Label(new Rect(rect.x, rect.y + 1.0f, 120.0f, 22.0f), "GPU动画专用材质", EditorStyles.whiteLargeLabel);
        GUI.Label(new Rect(rect.x + rect.width - 110.0f, rect.y + 3.0f, 110.0f, 18.0f), "MoreFun TATeam");
        // 播放模式
        var tempInt = 0;
        rect = GUILayoutUtility.GetRect(100.0f, 20.0f);
        EditorGUI.DrawRect(rect, lightColor);
        var rect1 = new Rect(rect.x, rect.y, rect.width * 0.25f, rect.height);
        GUI.Label(new Rect(rect1.x, rect1.y + 2.0f, 50.0f, 18.0f), "播放模式");
        EditorGUI.BeginChangeCheck();
        {
            tempInt = EditorGUI.Popup(new Rect(rect1.x + 50.0f, rect1.y + 2.0f, rect1.width - 55.0f, 16.0f), (int)GetPlayMode(targetMat), PlayModeLabels);
        }
        if (EditorGUI.EndChangeCheck())
        {
            SetPlayMode(targetMat, (PlayMode)tempInt);
        }
        var playMode = (PlayMode)tempInt;
        // 表现模式
        var rect2 = new Rect(rect.x + rect.width * 0.25f, rect.y, rect.width * 0.25f, rect.height);
        GUI.Label(new Rect(rect2.x, rect2.y + 2.0f, 50.0f, 18.0f), "表现模式");
        EditorGUI.BeginChangeCheck();
        {
            tempInt = EditorGUI.Popup(new Rect(rect2.x + 50.0f, rect2.y + 2.0f, rect2.width - 55.0f, 16.0f), (int)GetShowMode(targetMat), ShowModeLabels);
        }
        if (EditorGUI.EndChangeCheck())
        {
            SetShowMode(targetMat, (ShowMode)tempInt);
        }
        var showMode = (ShowMode)tempInt;
        // 混合模式
        var rect3 = new Rect(rect.x + rect.width * 0.5f, rect.y, rect.width * 0.25f, rect.height);
        GUI.Label(new Rect(rect3.x, rect3.y + 2.0f, 50.0f, 18.0f), "混合模式");
        EditorGUI.BeginChangeCheck();
        {
            tempInt = EditorGUI.Popup(new Rect(rect3.x + 50.0f, rect3.y + 2.0f, rect3.width - 50.0f, 16.0f), (int)GetBlendMode(targetMat), BlendModeLabels);
        }
        if (EditorGUI.EndChangeCheck())
        {
            SetBlendMode(targetMat, (BlendMode)tempInt, true);
        }
        var blendMode = (BlendMode)tempInt;
        // 通道遮罩
        var rect4 = new Rect(rect.x + rect.width * 0.75f, rect.y, rect.width * 0.25f, rect.height);
        GUI.Label(new Rect(rect4.x + 5.0f, rect4.y + 2.0f, 50.0f, 18.0f), "通道遮罩");
        EditorGUI.BeginChangeCheck();
        {
            tempInt = EditorGUI.Popup(new Rect(rect4.x + 55.0f, rect4.y + 2.0f, rect4.width - 55.0f, 16.0f), (int)GetColorMask(targetMat), ColorMaskLabels);
        }
        if (EditorGUI.EndChangeCheck())
        {
            SetColorMask(targetMat, (ColorMask)tempInt);
        }
        var colorMask = (ColorMask)tempInt;
        // RenderQueue
        rect = GUILayoutUtility.GetRect(100.0f, 20.0f);
        EditorGUI.DrawRect(rect, lightColor);
        GUI.Label(new Rect(rect.x, rect.y + 2.0f, 50.0f, 18.0f), "渲染队列");
        EditorGUI.BeginDisabledGroup(true);
        {
            EditorGUI.IntField(new Rect(rect.x + 50.0f, rect.y + 2.0f, 50.0f, 16.0f), targetMat.renderQueue);
        }
        EditorGUI.EndDisabledGroup();
        EditorGUI.BeginChangeCheck();
        {
            tempInt = EditorGUI.IntSlider(new Rect(rect.x + 110.0f, rect.y + 2.0f, rect.width - 110.0f, 16.0f), GetQueueAddition(targetMat, blendMode), -10, 10);
        }
        if (EditorGUI.EndChangeCheck())
        {
            SetQueueAddition(targetMat, blendMode, tempInt);
        }
        // 空行
        rect = GUILayoutUtility.GetRect(100.0f, 5.0f);
        EditorGUI.DrawRect(rect, lightColor);
        // 颜色贴图相关设置
        rect = GUILayoutUtility.GetRect(100.0f, 74.0f);
        EditorGUI.DrawRect(rect, darkColor);
        var mainTex = FindProperty("_MainTex", properties);
        materialEditor.TextureProperty(new Rect(rect.x, rect.y + 2.0f, rect.width, 72.0f), mainTex, mainTex.displayName, mainTex.name, true);
        var opacity = FindProperty("_Opacity", properties);
        if (blendMode != BlendMode.Opaque)
        {
            GUI.Label(new Rect(rect.x + 15.0f, rect.y + 19.0f, 60.0f, 15.0f), opacity.displayName);
            materialEditor.RangeProperty(new Rect(rect.x + 80.0f, rect.y + 19.0f, rect.width - 146.0f, 15.0f), opacity, "");
        }
        // 动画图相关设置
        rect = GUILayoutUtility.GetRect(100.0f, 0.0f);
        EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 74.0f), lightColor);
        var animMap = FindProperty("_AnimMap", properties);
        materialEditor.TextureProperty(animMap, animMap.displayName + ":" + animMap.name, false);
        rect = GUILayoutUtility.GetRect(100.0f, 0.0f);
        rect = new Rect(rect.x + 15.0f, rect.y - 40.0f, rect.width - 81.0f, 30.0f);
        switch (playMode)
        {
            case PlayMode.Evaluate:

                var animProgress = FindProperty("_AnimProgress", properties);
                GUI.Label(new Rect(rect.x, rect.y, 60.0f, 15.0f), new GUIContent(animProgress.displayName, animProgress.name));
                materialEditor.RangeProperty(new Rect(rect.x + 65.0f, rect.y, rect.width - 65.0f, 15.0f), animProgress, "");
                break;
            case PlayMode.Cycle:
                var animSpeed = FindProperty("_AnimSpeed", properties);
                GUI.Label(new Rect(rect.x, rect.y, 60.0f, 15.0f), new GUIContent(animSpeed.displayName, animSpeed.name));
                materialEditor.RangeProperty(new Rect(rect.x + 65.0f, rect.y, rect.width - 65.0f, 15.0f), animSpeed, "");
                break;
        }
        var samplerParams = FindProperty("_SamplerParams", properties);
        GUI.Label(new Rect(rect.x, rect.y + 16.0f, 60.0f, 15.0f), samplerParams.displayName);
        EditorGUI.BeginDisabledGroup(true);
        {
            materialEditor.VectorProperty(new Rect(rect.x + 65.0f, rect.y + 16.0f, rect.width - 65.0f, 15.0f), samplerParams, "");
        }
        EditorGUI.EndDisabledGroup();
        // SubMesh相关设置
        if (showMode != ShowMode.Normal)
        {
            rect = GUILayoutUtility.GetRect(100.0f, 0.0f);
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 64.0f), darkColor);
            var animStep = FindProperty("_AnimStep", properties);
            materialEditor.RangeProperty(animStep, animStep.displayName);
            var alphaStep = FindProperty("_AlphaStep", properties);
            materialEditor.RangeProperty(alphaStep, alphaStep.displayName);
            var offsetStep = FindProperty("_OffsetStep", properties);
            materialEditor.VectorProperty(offsetStep, offsetStep.displayName);
            EditorGUILayout.Space();
        }
        // 设置FieldWidth回默认值
        EditorGUIUtility.fieldWidth = 0.0f;
    }
}