using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class ShaderVaraintsCollecter
{
    public static ShaderVaraintsCollecter Instance
    {
        get
        {
            if (_instance == null)
                _instance = new ShaderVaraintsCollecter();
            return _instance;
        }
    }

    private static ShaderVaraintsCollecter _instance;

    private ShaderVariantCollection.ShaderVariant _sv = new ShaderVariantCollection.ShaderVariant();
    private int _matNum = 0;
    public bool OnlyCollectCG = false;

    private ShaderVaraintsCollecter() { }

    // PerformanceCalculator calculator = new PerformanceCalculator();

    private const string COMPILE_NONE = "THE_NAME_YOU_WILL_NEVER_USE";
    private string[] _stripConditionProperties =
    {
        //"_LightAttenuation"
    };

    private string[][] _stripTargetKeywords =
    {
        new string[]
        {
            "_MAIN_LIGHT_SHADOWS_CASCADE",
            "_SHADOWS_SOFT"
        },
    };

    public void Collect()
    {
        Prepare();
        CollectShaderVaraints();

        // calculator.LogAllKeypair();
        // calculator.Reset();
    }

    private void Prepare()
    {
        _matNum = 0;

        //AssetDatabase.DeleteAsset("Assets/Shaderlib/Materials");
        //Directory.CreateDirectory("Assets/Shaderlib/Materials");

    }

    private void CollectShaderVaraints()
    {
        ShaderVariantCollection svc = new ShaderVariantCollection();

        string[] allGuids = AssetDatabase.FindAssets("t:Material", new string[] { "Assets/Art/Effect", "Assets/Art/Shaders", "Assets/TResources", "Assets/TechArtist" });
        string assetPath;
        Material material;
        Shader shader;
        string shaderCode;
        string[] materialKeywords;
        StreamReader sr;
        string shaderPath;
        float percent = 1.0f / allGuids.Length;

        GameObject svcPrefab = new GameObject("ShaderVariants");
        ShaderWarmup warmupScript = svcPrefab.AddComponent<ShaderWarmup>();
        warmupScript.ShaderVariants = new List<SerializeShaderVariant>();

        for (int i = 0; i < allGuids.Length; ++i)
        {
            //获取所有材质球
            assetPath = AssetDatabase.GUIDToAssetPath(allGuids[i]);
            material = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Material)) as Material;
            if (material == null)
                continue;

            shader = material.shader;
            EditorUtility.DisplayProgressBar("正在分析Shader : " + shader.name, "正在生成第" + _matNum + "个变体", percent * i);

            //过滤掉部分shader
            if (shader == null)
                continue;

            shaderPath = AssetDatabase.GetAssetPath(shader);
            if (!ShaderFilter(shader, shaderPath))
                continue;

            //读取shader代码进行分析
            try
            {
                sr = new StreamReader(shaderPath, Encoding.Default);
            }
            catch { continue; }
            shaderCode = sr.ReadToEnd();
            materialKeywords = material.shaderKeywords;

            //分析一个pass
            ParseCGShaderCode(shaderCode, materialKeywords, shader, ref svc, warmupScript);
            ParseHLSLShaderCode(shaderCode, materialKeywords, shader, ref svc, material, warmupScript);
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();

        string prefabPath = "Assets/TechArtist/Shaders/ShaderVariantsPrefab.prefab";
        string path = "Assets/TechArtist/Shaders/ShaderVariants.shadervariants";
        if (path != "")
        {
            path = path.Substring(path.IndexOf("/Assets/") + 1);
            Debug.Log("shader变体总数：" + svc.variantCount);
            AssetDatabase.CreateAsset(svc, path);
            PrefabUtility.SaveAsPrefabAsset(svcPrefab, prefabPath);
        }

        GameObject.DestroyImmediate(svcPrefab);
    }

    private bool ShaderFilter(Shader shader, string shaderPath)
    {
        if (shader.name.ToLower().Contains("nba") && shaderPath.Contains("TechArtist/Shaders"))
            return true;
        return false;
    }

    public void ParseCGShaderCode(string shaderCode, string[] materialKeywords, Shader shader, ref ShaderVariantCollection svc, ShaderWarmup warmupScript)
    {
        int passBeginIndex = -1, passEndIndex = -1;
        PassType passType;
        List<string[]> multiCompileKeywordList = new List<string[]>();
        List<string> featureKeywordList = new List<string>(8);

        //分析一个pass
        while ((passBeginIndex = shaderCode.IndexOf("CGPROGRAM")) != -1)
        {
            multiCompileKeywordList.Clear();
            featureKeywordList.Clear();
            string unityMultiCompile = null;
            passEndIndex = shaderCode.IndexOf("ENDCG");
            if (passEndIndex < passBeginIndex)
            {
                shaderCode = shaderCode.Substring(passEndIndex + 5, shaderCode.Length - passEndIndex - 6);
                continue;
            }

            string subStr = shaderCode.Substring(passBeginIndex, passEndIndex - passBeginIndex);

            //multi_compile keyword
            if (!ParsePassCGMultiKeyword(subStr, out passType, out unityMultiCompile, multiCompileKeywordList))
            {
                //next pass
                shaderCode = shaderCode.Substring(passEndIndex + 5, shaderCode.Length - passEndIndex - 6);
                continue;
            }

            //feature keyword
            ParseCGPassFeatureKeyword(subStr, materialKeywords, featureKeywordList);

            //计算multi_compile keyword的组合
            List<string[]> keywordsList = new List<string[]>();

            if (unityMultiCompile != null)
                keywordsList.Add(new string[] { unityMultiCompile.ToUpper() });
            else
                keywordsList.Add(new string[] { COMPILE_NONE }); //如果unityMultiComile为空，将这里将complie_none加入数组作占位让逻辑能继续进行

            int keywordListCount = keywordsList.Count;
            List<string[]> arrayTemp = new List<string[]>();
            List<string> temp = new List<string>();

            //calculator.BeginCalculate("计算组合");
            for (int j = 0; j < multiCompileKeywordList.Count; ++j)
            {
                arrayTemp.Clear();
                for (int k = 0; k < keywordListCount; ++k)
                {
                    for (int l = 0; l < multiCompileKeywordList[j].Length; ++l)
                    {
                        temp.Clear();

                        temp.AddRange(keywordsList[k]);

                        if (multiCompileKeywordList[j][l] != COMPILE_NONE)
                            temp.Add(multiCompileKeywordList[j][l]);
                        arrayTemp.Add(temp.ToArray());
                    }
                }

                keywordsList.Clear();
                for (int k = 0; k < arrayTemp.Count; ++k)
                    if (!keywordsList.Contains(arrayTemp[k]))
                        keywordsList.Add(arrayTemp[k]);

                keywordListCount = keywordsList.Count;
            }

            //calculator.EndCalculate("计算组合");
            //calculator.BeginCalculate("feature组合");
            //加上feature keyword
            if (featureKeywordList.Count > 0)
            {
                for (int j = 0; j < keywordsList.Count; ++j)
                {
                    temp.Clear();
                    temp.AddRange(keywordsList[j]);
                    temp.AddRange(featureKeywordList.ToArray());

                    ////清除unityMultiCompile为空时加入的占位符
                    //for (int k = 0; k < temp.Count; ++k)
                    //    if (temp[k] == COMPILE_NONE)
                    //    {
                    //        temp.RemoveAt(k);
                    //        --k;
                    //        break;
                    //    }
                    temp.Sort();
                    keywordsList[j] = temp.ToArray();
                }
            }

            for (int j = 0; j < keywordsList.Count; ++j)
            {
                temp.Clear();
                temp.AddRange(keywordsList[j]);

                //清除unityMultiCompile为空时加入的占位符
                for (int k = 0; k < temp.Count; ++k)
                    if (temp[k] == COMPILE_NONE)
                    {
                        temp.RemoveAt(k);
                        --k;
                        break;
                    }

                temp.Sort();
                keywordsList[j] = temp.ToArray();
            }

            //calculator.EndCalculate("feature组合");
            //calculator.BeginCalculate("写入shaderVariants");
            //添加shader varaints
            if (keywordsList.Count > 0)
            {
                for (int j = 0; j < keywordsList.Count; ++j)
                {
                    try
                    {
                        _sv.shader = shader;
                        _sv.passType = passType;
                        _sv.keywords = keywordsList[j];
                        if (svc.Contains(_sv))
                            continue;

                        SerializeShaderVariant sv = new SerializeShaderVariant(_sv);
                        warmupScript.ShaderVariants.Add(sv);

                        // ShaderVariantCollection.ShaderVariant sv = new ShaderVariantCollection.ShaderVariant(shader, passtype, keywordsList[j]);
                        if (svc.Add(_sv))
                        {
                            _matNum++;

                            //Material newMat = new Material(shader);
                            //newMat.shaderKeywords = keywordsList[j];
                            //AssetDatabase.CreateAsset(newMat, "Assets/Shaderlib/Materials/Mat" + _matNum++.ToString() + ".mat");
                        }
                    }
                    catch { continue; }
                }
            }
            else
            {
                try
                {
                    _sv.shader = shader;
                    _sv.passType = passType;
                    if (svc.Contains(_sv))
                        continue;

                    SerializeShaderVariant sv = new SerializeShaderVariant(_sv);
                    warmupScript.ShaderVariants.Add(sv);

                    // ShaderVariantCollection.ShaderVariant sv = new ShaderVariantCollection.ShaderVariant(shader, passtype, keywordsList[j]);
                    if (svc.Add(_sv))
                    {
                        _matNum++;

                        //Material newMat = new Material(shader);
                        //AssetDatabase.CreateAsset(newMat, "Assets/Shaderlib/Materials/Mat" + _matNum++.ToString() + ".mat");
                    }
                }
                catch { continue; }
            }

            //calculator.EndCalculate("写入shaderVariants");
            //--------------大功告成---------------------------

            //next pass
            shaderCode = shaderCode.Substring(passEndIndex + 5, shaderCode.Length - passEndIndex - 6);
        }
    }

    public void ParseHLSLShaderCode(string shaderCode, string[] materialKeywords, Shader shader, ref ShaderVariantCollection svc, Material material, ShaderWarmup warmupScript)
    {
        if (OnlyCollectCG)
            return;

        int passBeginIndex = -1, passEndIndex = -1;
        PassType passType;
        List<string[]> multiCompileKeywordList = new List<string[]>();
        List<string> featureKeywordList = new List<string>(8);

        //分析一个pass
        while ((passBeginIndex = shaderCode.IndexOf("HLSLPROGRAM")) != -1)
        {
            multiCompileKeywordList.Clear();
            featureKeywordList.Clear();
            string unityMultiCompile = null;
            passEndIndex = shaderCode.IndexOf("ENDHLSL");
            if (passEndIndex < passBeginIndex)
            {
                shaderCode = shaderCode.Substring(passEndIndex + 5, shaderCode.Length - passEndIndex - 6);
                continue;
            }

            string subStr = shaderCode.Substring(passBeginIndex, passEndIndex - passBeginIndex);

            //multi_compile keyword
            if (!ParsePassHLSLMultiKeyword(subStr, out passType, out unityMultiCompile, multiCompileKeywordList))
            {
                //next pass
                shaderCode = shaderCode.Substring(passEndIndex + 5, shaderCode.Length - passEndIndex - 6);
                continue;
            }

            //if (material.enableInstancing)
            //{
            //    if (unityMultiCompile == "instancing_on")
            //    {
            //        unityMultiCompile = null;
            //        featureKeywordList.Add("INSTANCING_ON");
            //    }
            //    else if (unityMultiCompile == "procedural_instancing_on")
            //    {
            //        unityMultiCompile = null;
            //        featureKeywordList.Add("PROCEDURAL_INSTANCING_ON");
            //    }
            //}

            //feature keyword
            ParseHLSLPassFeatureKeyword(subStr, materialKeywords, featureKeywordList);

            //计算multi_compile keyword的组合
            List<string[]> keywordsList = new List<string[]>();

            if (unityMultiCompile != null)
                keywordsList.Add(new string[] { unityMultiCompile.ToUpper() });

            int keywordListCount = keywordsList.Count;
            List<string[]> arrayTemp = new List<string[]>();
            List<string> temp = new List<string>();

            //calculator.BeginCalculate("计算组合");
            for (int j = 0; j < multiCompileKeywordList.Count; ++j)
            {
                arrayTemp.Clear();

                for (int l = 0; l < multiCompileKeywordList[j].Length; ++l)
                {
                    if (keywordListCount > 0)
                    {
                        for (int k = 0; k < keywordListCount; ++k)
                        {
                            temp.Clear();
                            temp.AddRange(keywordsList[k]);

                            if (!temp.Contains(multiCompileKeywordList[j][l]))
                                temp.Add(multiCompileKeywordList[j][l]);

                            temp.Sort();
                            arrayTemp.Add(temp.ToArray());
                        }
                    }
                    else
                    {
                        temp.Clear();

                        if (!temp.Contains(multiCompileKeywordList[j][l]))
                            temp.Add(multiCompileKeywordList[j][l]);

                        temp.Sort();
                        arrayTemp.Add(temp.ToArray());
                    }
                }

                keywordsList.Clear();
                for (int k = 0; k < arrayTemp.Count; ++k)
                    if (!keywordsList.Contains(arrayTemp[k]))
                        keywordsList.Add(arrayTemp[k]);

                keywordListCount = keywordsList.Count;
            }

            //calculator.EndCalculate("计算组合");
            //calculator.BeginCalculate("feature组合");
            //加上feature keyword
            if (featureKeywordList.Count > 0)
            {
                for (int j = 0; j < keywordsList.Count; ++j)
                {
                    temp.Clear();
                    temp.AddRange(keywordsList[j]);
                    temp.AddRange(featureKeywordList.ToArray());

                    temp.Sort();
                    keywordsList[j] = temp.ToArray();
                }
            }

            // NBARenderPipelineAsset pipeline = (NBARenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
            // bool softShadow = pipeline ? pipeline.supportsSoftShadows : false;
            bool softShadow = false;

            for (int j = 0; j < keywordsList.Count; ++j)
            {
                temp.Clear();
                temp.AddRange(keywordsList[j]);

                bool removeFlag = false;

                for (int k = 0; k < _stripConditionProperties.Length; ++k)
                {
                    if (material.GetFloat(_stripConditionProperties[k]) < 1)
                    {
                        for (int l = 0; l < _stripTargetKeywords[k].Length; ++l)
                        {
                            if (temp.Contains(_stripTargetKeywords[k][l].ToUpper()))
                            {
                                removeFlag = true;
                                break;
                            }
                        }
                    }

                    if (removeFlag)
                        break;
                }

                if (!softShadow && temp.Contains("_SHADOWS_SOFT"))
                    removeFlag = true;

                if (removeFlag)
                {
                    keywordsList.RemoveAt(j);
                    --j;
                    continue;
                }

                //清除unityMultiCompile为空时加入的占位符
                for (int k = 0; k < temp.Count; ++k)
                    if (temp[k] == COMPILE_NONE)
                    {
                        temp.RemoveAt(k);
                        --k;
                    }

                temp.Sort();
                if (temp.Count > 0)
                    keywordsList[j] = temp.ToArray();
                else
                {
                    keywordsList.RemoveAt(j);
                    --j;
                }
            }

            //calculator.EndCalculate("feature组合");
            //calculator.BeginCalculate("写入shaderVariants");
            //添加shader varaints
            if (keywordsList.Count > 0)
            {
                for (int j = 0; j < keywordsList.Count; ++j)
                {
                    try
                    {
                        _sv.shader = shader;
                        _sv.passType = passType;
                        _sv.keywords = keywordsList[j];
                        if (svc.Contains(_sv))
                            continue;

                        SerializeShaderVariant sv = new SerializeShaderVariant(_sv);
                        warmupScript.ShaderVariants.Add(sv);

                        if (svc.Add(_sv))
                        {
                            _matNum++;

                            //Material newMat = new Material(shader);
                            //newMat.shaderKeywords = keywordsList[j];
                            //AssetDatabase.CreateAsset(newMat, "Assets/Shaderlib/Materials/Mat" + _matNum++.ToString() + ".mat");
                        }
                    }
                    catch { continue; }
                }
            }
            else
            {
                try
                {
                    _sv.shader = shader;
                    _sv.passType = passType;
                    _sv.keywords = new string[0];
                    if (!svc.Contains(_sv))
                    {
                        SerializeShaderVariant sv = new SerializeShaderVariant(_sv);
                        warmupScript.ShaderVariants.Add(sv);

                        // ShaderVariantCollection.ShaderVariant sv = new ShaderVariantCollection.ShaderVariant(shader, passtype, keywordsList[j]);
                        if (svc.Add(_sv))
                        {
                            _matNum++;

                            //Material newMat = new Material(shader);
                            //AssetDatabase.CreateAsset(newMat, "Assets/Shaderlib/Materials/Mat" + _matNum++.ToString() + ".mat");
                        }
                    }
                }
                catch { }
            }

            //calculator.EndCalculate("写入shaderVariants");
            //--------------大功告成---------------------------

            //next pass
            shaderCode = shaderCode.Substring(passEndIndex + 5, shaderCode.Length - passEndIndex - 6);
        }
    }

    private bool ParsePassCGMultiKeyword(in string shaderPassCode, out PassType passType, out string unityMultiCompile, List<string[]> multiCompileKeywordList)
    {
        string passCodeTemp = shaderPassCode;
        int keyWordIndex = -1, keyWordEndIndex = -1;
        List<string> keywords = new List<string>();
        passType = PassType.Normal;
        unityMultiCompile = null;
        while ((keyWordIndex = passCodeTemp.IndexOf("multi_compile")) != -1)
        {
            keyWordEndIndex = passCodeTemp.IndexOf('\n', keyWordIndex);
            string multiStr = passCodeTemp.Substring(keyWordIndex + 13, keyWordEndIndex - keyWordIndex - 13);
            passCodeTemp = passCodeTemp.Substring(keyWordEndIndex, passCodeTemp.Length - keyWordEndIndex - 1);

            if (multiStr.Contains("LIGHTMAP_ON"))
                continue;

            if (multiStr[0] == '_')
            {
                multiStr = multiStr.ToLower();
                if (multiStr.Contains("_fwdbase"))
                {
                    passType = UnityEngine.Rendering.PassType.ForwardBase;
                    unityMultiCompile = "DIRECTIONAL";

                    //multiCompileKeywordList.Add(new string[] { "SHADOWS_SCREEN", COMPILE_NONE });
                }
                else if (multiStr.Contains("_fwdadd"))
                {
                    passType = UnityEngine.Rendering.PassType.ForwardAdd;
                    unityMultiCompile = "point";
                    return false;
                }
                else if (multiStr.Contains("_particles"))
                {
                    passType = UnityEngine.Rendering.PassType.Normal;
                    unityMultiCompile = "softparticles_on";
                }
                else if (multiStr.Contains("_instancing"))
                {
                    //passtype = UnityEngine.Rendering.PassType.Normal;
                    //unityMultiCompile = null

                    //multiCompileKeywordList.Add(new string[] { "INSTANCING_ON", COMPILE_NONE });
                }
                else if (multiStr.Contains("_shadowcaster"))
                {
                    passType = UnityEngine.Rendering.PassType.ShadowCaster;
                    unityMultiCompile = "shadows_depth";
                }
                else if (multiStr.Contains("_fog"))
                {
                    unityMultiCompile = null;
                    //multiCompileKeywordList.Add(new string[] { "FOG_LINEAR", COMPILE_NONE });
                }
                else
                {
                    unityMultiCompile = null;
                    passType = UnityEngine.Rendering.PassType.Normal;
                    return false;
                }
            }
            else
            {
                //calculator.BeginCalculate("收集keyword");
                keywords.Clear();

                while (multiStr.Length != 0)
                {
                    while (multiStr[0] == ' ')
                    {
                        multiStr = multiStr.Substring(1, multiStr.Length - 1);
                        if (multiStr.Length == 0)
                            break;
                    }

                    keyWordIndex = multiStr.IndexOf(' ');
                    if (keyWordIndex < 0)
                        keyWordIndex = multiStr.IndexOf('\n');
                    if (keyWordIndex < 0)
                    {
                        if (multiStr.Length > 2)
                        {
                            multiStr = multiStr.Replace(" ", "");
                            multiStr = multiStr.Replace("\r", "");
                            keywords.Add(multiStr);
                        }
                        break;
                    }
                    string keyword = multiStr.Substring(0, keyWordIndex + 1);
                    multiStr = multiStr.Substring(keyWordIndex + 1, multiStr.Length - keyWordIndex - 1);

                    keyword = keyword.Replace(" ", "");

                    if (keyword == "")
                        continue;

                    if (keyword == "__" || keyword == "_")
                        keywords.Add(COMPILE_NONE);
                    else
                        keywords.Add(keyword);
                }
                if (keywords.Count == 1)
                    keywords.Add(COMPILE_NONE);
                multiCompileKeywordList.Add(keywords.ToArray());

                //calculator.EndCalculate("收集keyword");
            }
        }
        return true;
    }

    private bool ParsePassHLSLMultiKeyword(in string shaderPassCode, out PassType passType, out string unityMultiCompile, List<string[]> multiCompileKeywordList)
    {
        string passCodeTemp = shaderPassCode;
        int keyWordIndex = -1, keyWordEndIndex = -1;
        List<string> keywords = new List<string>();
        passType = PassType.ScriptableRenderPipeline;
        unityMultiCompile = null;
        while ((keyWordIndex = passCodeTemp.IndexOf("multi_compile")) != -1)
        {
            keyWordEndIndex = passCodeTemp.IndexOf('\n', keyWordIndex);
            string multiStr = passCodeTemp.Substring(keyWordIndex + 13, keyWordEndIndex - keyWordIndex - 13);
            passCodeTemp = passCodeTemp.Substring(keyWordEndIndex, passCodeTemp.Length - keyWordEndIndex - 1);

            if (multiStr.Contains("LIGHTMAP_ON"))
                continue;

            if (multiStr[0] == '_')
            {
                multiStr = multiStr.ToLower();
                if (multiStr.Contains("_fwdbase"))
                {
                    unityMultiCompile = "DIRECTIONAL";

                    //multiCompileKeywordList.Add(new string[] { "SHADOWS_SCREEN", COMPILE_NONE });
                }
                else if (multiStr.Contains("_fwdadd"))
                {
                    unityMultiCompile = "point";
                }
                else if (multiStr.Contains("_particles"))
                {
                    unityMultiCompile = "softparticles_on";
                }
                else if (multiStr.Contains("_instancing"))
                {
                    //if (shaderPassCode.Contains("instancing_options procedural"))
                    //    unityMultiCompile = "procedural_instancing_on";
                    //else
                    //    unityMultiCompile = "instancing_on";
                    unityMultiCompile = null;
                }
                else if (multiStr.Contains("_shadowcaster"))
                {
                    passType = PassType.ShadowCaster;
                }
                else if (multiStr.Contains("_fog"))
                {
                    unityMultiCompile = null;
                    //multiCompileKeywordList.Add(new string[] { "FOG_LINEAR", COMPILE_NONE });
                }
                else
                {
                    unityMultiCompile = null;
                }
            }
            else
            {
                //calculator.BeginCalculate("收集keyword");
                keywords.Clear();

                while (multiStr.Length != 0)
                {
                    while (multiStr[0] == ' ')
                    {
                        multiStr = multiStr.Substring(1, multiStr.Length - 1);
                        if (multiStr.Length == 0)
                            break;
                    }

                    keyWordIndex = multiStr.IndexOf(' ');
                    if (keyWordIndex < 0)
                        keyWordIndex = multiStr.IndexOf('\n');
                    if (keyWordIndex < 0)
                    {
                        if (multiStr.Length > 2)
                        {
                            multiStr = multiStr.Replace(" ", "");
                            multiStr = multiStr.Replace("\r", "");
                            keywords.Add(multiStr);
                        }
                        break;
                    }
                    string keyword = multiStr.Substring(0, keyWordIndex + 1);
                    multiStr = multiStr.Substring(keyWordIndex + 1, multiStr.Length - keyWordIndex - 1);

                    keyword = keyword.Replace(" ", "");

                    if (keyword == "")
                        continue;

                    if (keyword == "__" || keyword == "_")
                        keywords.Add(COMPILE_NONE);
                    else
                        keywords.Add(keyword);
                }
                if (keywords.Count == 1)
                    keywords.Add(COMPILE_NONE);
                multiCompileKeywordList.Add(keywords.ToArray());

                //calculator.EndCalculate("收集keyword");
            }
        }
        return true;
    }

    private void ParseCGPassFeatureKeyword(in string shaderPassCode, in string[] materialKeywords, List<string> featureKeywordList)
    {
        string passCodeTemp = shaderPassCode;
        int featureBeginIndex = -1, featureEndIndex = -1;
        string[] featureKeywords;
        while ((featureBeginIndex = passCodeTemp.IndexOf("shader_feature")) != -1)
        {
            //确保字符串是以shader_feature开头的
            passCodeTemp = passCodeTemp.Substring(featureBeginIndex, passCodeTemp.Length - featureBeginIndex - 1);
            featureBeginIndex = passCodeTemp.IndexOf("shader_feature");

            //calculator.BeginCalculate("收集feature");
            featureEndIndex = passCodeTemp.IndexOf('\n', featureBeginIndex) - 1;
            if (featureEndIndex < 0)
                featureEndIndex = passCodeTemp.Length - 1;
            string featureTemp = passCodeTemp.Substring(featureBeginIndex + 15, featureEndIndex - featureBeginIndex - 15);
            featureKeywords = featureTemp.Split(' ');

            for (int j = 0; j < materialKeywords.Length; ++j)
            {
                for (int k = 0; k < featureKeywords.Length; ++k)
                {
                    featureTemp = featureKeywords[k].Replace(" ", "");
                    if (featureTemp == "_" || featureTemp == "__" || featureTemp == "")
                        continue;

                    if (materialKeywords[j].ToLower() == featureTemp.ToLower())
                        featureKeywordList.Add(featureTemp);
                    else if (materialKeywords[j].ToLower() == "fog_linear" && !featureKeywordList.Contains("FOG_LINEAR"))
                        featureKeywordList.Add("FOG_LINEAR");
                    else if (materialKeywords[j].ToLower() == "lightmap_on" && !featureKeywordList.Contains("LIGHTMAP_ON"))
                        featureKeywordList.Add("LIGHTMAP_ON");
                }
            }

            passCodeTemp = passCodeTemp.Substring(featureEndIndex, passCodeTemp.Length - featureEndIndex - 1);

            //calculator.EndCalculate("收集feature");
        }
    }

    private void ParseHLSLPassFeatureKeyword(in string shaderPassCode, in string[] materialKeywords, List<string> featureKeywordList)
    {
        string passCodeTemp = shaderPassCode;
        int featureBeginIndex = -1, featureEndIndex = -1;
        string[] featureKeywords;

        while ((featureBeginIndex = passCodeTemp.IndexOf("shader_feature_local")) != -1)
        {
            //确保字符串是以shader_feature开头的
            passCodeTemp = passCodeTemp.Substring(featureBeginIndex, passCodeTemp.Length - featureBeginIndex - 1);
            featureBeginIndex = passCodeTemp.IndexOf("shader_feature_local");

            //calculator.BeginCalculate("收集feature");
            featureEndIndex = passCodeTemp.IndexOf('\n', featureBeginIndex) - 1;
            if (featureEndIndex < 0)
                featureEndIndex = passCodeTemp.Length - 1;
            string featureTemp = passCodeTemp.Substring(featureBeginIndex + 15, featureEndIndex - featureBeginIndex - 15);
            featureKeywords = featureTemp.Split(' ');

            for (int j = 0; j < materialKeywords.Length; ++j)
            {
                for (int k = 0; k < featureKeywords.Length; ++k)
                {
                    featureTemp = featureKeywords[k].Replace(" ", "");
                    if (featureTemp == "_" || featureTemp == "__" || featureTemp == "")
                        continue;

                    if (materialKeywords[j].ToLower() == featureTemp.ToLower())
                        featureKeywordList.Add(featureTemp);
                    else if (materialKeywords[j].ToLower() == "fog_linear" && !featureKeywordList.Contains("FOG_LINEAR"))
                        featureKeywordList.Add("FOG_LINEAR");
                    else if (materialKeywords[j].ToLower() == "lightmap_on" && !featureKeywordList.Contains("LIGHTMAP_ON"))
                        featureKeywordList.Add("LIGHTMAP_ON");
                }
            }

            passCodeTemp = passCodeTemp.Substring(featureEndIndex, passCodeTemp.Length - featureEndIndex - 1);

            //calculator.EndCalculate("收集feature");
        }
    }
}
