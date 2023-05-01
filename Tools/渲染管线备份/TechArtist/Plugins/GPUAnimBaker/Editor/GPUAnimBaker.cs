// 开发日志 Bound位置不对 需要处理

using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TheNextMoba.ArtTools.Anim
{
    public class GPUAnimBaker : EditorWindow
    {
        // 路径常量
        static readonly string rootPath = "Assets/ThirdParty/GPUAnimBaker/";
        static readonly string WireShaderPath = "GPUAnim/MeshWire";
        static readonly string PreviewShaderPath = "Shader Graphs/Next_E_11_GPUAnim_0_General";
        static readonly string GeneralShaderPath = "Shader Graphs/Next_E_11_GPUAnim_0_General";
        public static readonly string LogoPath = rootPath + "Editor/Image/GAB_Logo.tga";
        public static Object asset;
        static ModelImporter importer;
        static Animation animation;
        static List<AnimationState> animationStateList = new List<AnimationState>();
        static SkinnedMeshRenderer skinMeshRenderer;
        static Mesh skinedMesh;
        public static int subMeshCount = 1;
        static readonly string[] intLabels = new string[]
        {
            "X1",
            "X2",
            "X3",
            "X4",
            "X5",
            "X6",
            "X7",
            "X8",
            "X9",
            "X10"
        };
        static readonly int[] intValues = new int[]
        {
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9,
            10
        };


        public static bool copyNormals;
        public static bool copyTangents;
        public static bool copyColors;
        static bool copyUV1;
        public static bool copyUV2;
        public static bool copyUV3;
        static readonly bool copyUV4 = true;
        public static int stateIndex;

        public enum BakeAccuracy
        {
            Half,
            Float
        }

        static readonly string[] BakeAccuracyLables = new string[]
        {
            "16位精度",
            "32位精度"
        };

        public static BakeAccuracy bakeAccuracy;
        public enum SampleMode
        {
            Normal,
            Optimize
        }
        static readonly string[] SampleModeLables = new string[]
        {
            "普通采样",
            "优化采样"
        };

        public static SampleMode sampleMode;
        // GUI逻辑相关
        static PreviewRenderUtility previewGUI
        {
            get
            {
                if (m_PreviewGUI == null)
                {
                    m_PreviewGUI = new PreviewRenderUtility();
                    m_PreviewGUI.camera.farClipPlane = 500;
                    m_PreviewGUI.camera.clearFlags = CameraClearFlags.SolidColor;
                    m_PreviewGUI.camera.transform.position = new Vector3(0, 0, -PreviewDistance);
                }
                return m_PreviewGUI;
            }
        }
        static PreviewRenderUtility m_PreviewGUI;
        static Mesh meshBuffer
        {
            get
            {
                if (m_MeshBuffer == null) m_MeshBuffer = new Mesh();
                return m_MeshBuffer;
            }
        }
        static Mesh m_MeshBuffer;
        static Material wireMat
        {
            get
            {
                if (m_WireMat == null)
                    m_WireMat = new Material(Shader.Find(WireShaderPath));
                return m_WireMat;
            }
        }
        static Material m_WireMat;
        static Material previewMat
        {
            get
            {
                if (m_PreviewMat == null)
                    m_PreviewMat = new Material(Shader.Find(PreviewShaderPath));
                return m_PreviewMat;
            }
        }
        static Material m_PreviewMat;
        static bool assetIsValid;
        static bool ifPreviewAnim;
        static readonly float PreviewDistance = 10.0f;
        static Vector3 PreviewRotation = Vector3.zero;
        static float PreviewScale = 1.0f;

        static Texture logo
        {
            get
            {
                if (m_Logo == null)
                {
                    m_Logo = AssetDatabase.LoadAssetAtPath<Texture>(LogoPath);
                }
                return m_Logo;
            }
        }
        static Texture m_Logo;
        static readonly Color darkColor = new Color(0.0f, 0.0f, 0.0f, 0.3f);

        [MenuItem("公共工具/TATools/GPU动画烘培工具")]
        static void Init()
        {
            var window = (GPUAnimBaker)GetWindow(typeof(GPUAnimBaker));
            window.minSize = new Vector2(720, 300);
            window.maxSize = new Vector2(800, 480);
            window.titleContent = new GUIContent("GPUAnimBaker");
            window.Show();
        }

        void OnDestroy()
        {
            if (m_MeshBuffer != null) DestroyImmediate(m_MeshBuffer);
            if (m_WireMat != null) DestroyImmediate(m_WireMat);
            if (m_PreviewMat != null) DestroyImmediate(m_PreviewMat);
        }

        // GUI
        void OnGUI()
        {
            // Logo
            float aeraH = 30.0F;
            var rect = GUILayoutUtility.GetRect(100.0f, 100.0f);
            var previewRect = new Rect(rect.x + 450, rect.y + 10, 320, 300);

            // Asset输入框
            rect = GUILayoutUtility.GetRect(100.0f, aeraH);

            float lineY = rect.y + 2.0f;
            //-------------------- line 0--------------------
            lineY = rect.y + 2.0f;
            GUI.Label(new Rect(rect.x, lineY, 80.0f, 18.0f), " 模型动画FBX:");
            asset = EditorGUI.ObjectField(new Rect(rect.x + 80.0f, lineY, 170.0f, 16.0f), asset, typeof(Object), false);

            // 导入器状态反馈
            importer = ImporterExtractor(asset);
            GUI.Label(new Rect(rect.x + 250.0f, lineY, 80.0f, 18.0f), importer == null ? " 导入器不存在" : " 导入器已提取");
            // 导入器规范按钮
            assetIsValid = ImporterChecker(importer);
            EditorGUI.BeginDisabledGroup(importer == null);
            {
                if (assetIsValid == false)
                {
                    if (GUI.Button(new Rect(rect.x + 330.0f, lineY, 120.0F, 16.0f), "格式化FBX"))
                    {
                        ImporterNormalizer(importer);
                    }
                }
                else
                {
                    EditorGUI.BeginDisabledGroup(true);
                    {
                        GUI.Button(new Rect(rect.x + 330.0f, lineY, 120.0F, 16.0f), "符合Bake条件");
                    }
                    EditorGUI.EndDisabledGroup();
                }
            }
            EditorGUI.EndDisabledGroup();
            // 检查Asset资源
            assetIsValid &= AssetChecker(asset, ref animation, ref animationStateList, ref skinMeshRenderer, ref skinedMesh);
            // 检查模型信息
            var vertCount = 0;
            var hasNormals = false;
            var hasTangents = false;
            var hasColors = false;
            var hasUV1 = false;
            var hasUV2 = false;
            var hasUV3 = false;
            var hasUV4 = false;
            if (assetIsValid)
            {
                vertCount = skinedMesh.vertexCount;
                hasNormals = skinedMesh.normals.Length != 0;
                hasTangents = skinedMesh.tangents.Length != 0;
                hasColors = skinedMesh.colors.Length != 0;
                hasUV1 = skinedMesh.uv.Length != 0;
                hasUV2 = skinedMesh.uv2.Length != 0;
                hasUV3 = skinedMesh.uv3.Length != 0;
                hasUV4 = skinedMesh.uv4.Length != 0;

            }
            EditorGUI.BeginDisabledGroup(assetIsValid == false);
            {
                // Asset信息展示
                EditorGUI.BeginDisabledGroup(true);
                {
                    //---------------- line 1--------------------------------
                    // Animation信息
                    rect = GUILayoutUtility.GetRect(100.0f, aeraH);
                    GUI.Label(new Rect(rect.x, rect.y + 2.0f, 65.0f, 18.0f), " Animation");

                    EditorGUI.ObjectField(new Rect(rect.x + 65.0f, rect.y + 2.0f, 140.0f, 16.0f), animation, typeof(Animation), false);
                    // SkinMeshRenderer信息
                    GUI.Label(new Rect(rect.x + 205.0f, rect.y + 2.0f, 65.0f, 18.0f), "SMRender");

                    EditorGUI.ObjectField(new Rect(rect.x + 270.0f, rect.y + 2.0f, 140.0f, 16.0f), skinMeshRenderer, typeof(SkinnedMeshRenderer), false);

                    //------------------------- line 2----------------------------
                    // SkinedMesh信息
                    rect = GUILayoutUtility.GetRect(100.0f, aeraH);
                    GUI.Label(new Rect(rect.x, rect.y + 2.0f, 65.0f, 18.0f), " SkinMesh");
                    EditorGUI.ObjectField(new Rect(rect.x + 65.0f, rect.y + 2.0f, 105.0f, 16.0f), skinedMesh, typeof(Mesh), false);
                    // 顶点
                    GUI.Label(new Rect(rect.x + 170.0f, rect.y + 2.0f, 35.0f, 18.0f), "Verts");
                    EditorGUI.IntField(new Rect(rect.x + 205.0f, rect.y + 2.0f, 50.0f, 16.0f), vertCount);
                    // 法线
                    GUI.Label(new Rect(rect.x + 255.0f, rect.y + 2.0f, 15.0f, 18.0f), "N");
                    EditorGUI.Toggle(new Rect(rect.x + 265.0f, rect.y + 1.0f, 15.0f, 15.0f), hasNormals);
                    // 切线
                    GUI.Label(new Rect(rect.x + 280.0f, rect.y + 2.0f, 15.0f, 18.0f), "T");
                    EditorGUI.Toggle(new Rect(rect.x + 290.0f, rect.y + 1.0f, 15.0f, 15.0f), hasTangents);
                    // 顶点色
                    GUI.Label(new Rect(rect.x + 305.0f, rect.y + 2.0f, 15.0f, 18.0f), "C");
                    EditorGUI.Toggle(new Rect(rect.x + 315.0f, rect.y + 1.0f, 15.0f, 15.0f), hasColors);
                    // UV
                    GUI.Label(new Rect(rect.x + 330.0f, rect.y + 2.0f, 20.0f, 18.0f), "UV");
                    EditorGUI.Toggle(new Rect(rect.x + 350.0f, rect.y + 1.0f, 15.0f, 15.0f), hasUV1);
                    EditorGUI.Toggle(new Rect(rect.x + 365.0f, rect.y + 1.0f, 15.0f, 15.0f), hasUV2);
                    EditorGUI.Toggle(new Rect(rect.x + 380.0f, rect.y + 1.0f, 15.0f, 15.0f), hasUV3);
                    EditorGUI.Toggle(new Rect(rect.x + 395.0f, rect.y + 1.0f, 15.0f, 15.0f), hasUV4);
                }
                EditorGUI.EndDisabledGroup();

                //------------------------ line 3-----------------------------------
                // Mesh输出选项
                rect = GUILayoutUtility.GetRect(100.0f, aeraH);
                GUI.Label(new Rect(rect.x, rect.y + 2.0f, 120.0f, 18.0f), " 网格输出: 子网格数量");
                subMeshCount = EditorGUI.IntPopup(new Rect(rect.x + 120.0f, rect.y + 2.0f, 45.0f, 18.0f), subMeshCount, intLabels, intValues);
                // 顶点输出信息
                EditorGUI.BeginDisabledGroup(true);
                {
                    GUI.Label(new Rect(rect.x + 170.0f, rect.y + 2.0f, 35.0f, 18.0f), "Verts");
                    EditorGUI.IntField(new Rect(rect.x + 205.0f, rect.y + 2.0f, 50.0f, 16.0f), vertCount * subMeshCount);
                }
                EditorGUI.EndDisabledGroup();


                // 法线输出过滤
                GUI.Label(new Rect(rect.x + 255.0f, rect.y + 2.0f, 15.0f, 18.0f), "N");
                copyNormals = EditorGUI.Toggle(new Rect(rect.x + 265.0f, rect.y + 1.0f, 15.0f, 15.0f), copyNormals) && hasNormals;

                // 切线输出过滤
                GUI.Label(new Rect(rect.x + 280.0f, rect.y + 2.0f, 15.0f, 18.0f), "T");
                copyTangents = EditorGUI.Toggle(new Rect(rect.x + 290.0f, rect.y + 1.0f, 15.0f, 15.0f), copyTangents) && hasTangents;
                // 顶点色输出过滤
                GUI.Label(new Rect(rect.x + 305.0f, rect.y + 2.0f, 15.0f, 18.0f), "C");
                copyColors = EditorGUI.Toggle(new Rect(rect.x + 315.0f, rect.y + 1.0f, 15.0f, 15.0f), copyColors) && hasColors;
                // UV输出过滤
                GUI.Label(new Rect(rect.x + 330.0f, rect.y + 2.0f, 20.0f, 18.0f), "UV");



                EditorGUI.BeginDisabledGroup(true);
                {
                    copyUV1 = EditorGUI.Toggle(new Rect(rect.x + 350.0f, rect.y + 1.0f, 15.0f, 15.0f), hasUV1);
                }
                EditorGUI.EndDisabledGroup();
                copyUV2 = EditorGUI.Toggle(new Rect(rect.x + 365.0f, rect.y + 1.0f, 15.0f, 15.0f), copyUV2) && hasUV2;
                copyUV3 = EditorGUI.Toggle(new Rect(rect.x + 380.0f, rect.y + 1.0f, 15.0f, 15.0f), copyUV3) && hasUV3;
                EditorGUI.BeginDisabledGroup(true);
                {
                    EditorGUI.Toggle(new Rect(rect.x + 395.0f, rect.y + 1.0f, 15.0f, 15.0f), copyUV4);
                }
                EditorGUI.EndDisabledGroup();

                //-------------------------- line 4-----------------------------
                // 动画输出选项
                rect = GUILayoutUtility.GetRect(100.0f, aeraH);
                GUI.Label(new Rect(rect.x, rect.y + 2.0f, 130.0f, 18.0f), " 动画输出: 选择动画片段");
                var stateLabels = new List<string>() { "请选择" };
                var stateIndexes = new List<int> { 0 };

                var stateCount = animationStateList.Count;
                for (int i = 0; i < stateCount; i++)
                {
                    stateLabels.Add(animationStateList[i].clip.name);
                    stateIndexes.Add(i + 1);
                }
                stateIndex = Mathf.Min(stateCount, EditorGUI.IntPopup(new Rect(rect.x + 130.0f, rect.y + 2.0f, 95.0f, 18.0f), stateIndex, stateLabels.ToArray(), stateIndexes.ToArray()));
                // 采样精度设置
                GUI.Label(new Rect(rect.x + 230.0f, rect.y + 2.0f, 25.0f, 18.0f), "精度");
                bakeAccuracy = (BakeAccuracy)EditorGUI.Popup(new Rect(rect.x + 255.0f, rect.y + 2.0f, 65.0f, 18.0f), (int)bakeAccuracy, BakeAccuracyLables);
                EditorGUI.BeginDisabledGroup(true);
                {
                    GUI.Label(new Rect(rect.x + 320.0f, rect.y + 2.0f, 25.0f, 18.0f), "采样");
                    EditorGUI.Popup(new Rect(rect.x + 345.0f, rect.y + 2.0f, 65.0f, 18.0f), (int)sampleMode, SampleModeLables);
                    sampleMode = bakeAccuracy == BakeAccuracy.Half ? SampleMode.Optimize : SampleMode.Normal;
                }
                EditorGUI.EndDisabledGroup();

                //---------------------- line 5 ----------------------
                // 预览进度
                rect = GUILayoutUtility.GetRect(100.0f, aeraH);
                EditorGUI.BeginDisabledGroup(ifPreviewAnim == false);
                {
                    //GUI.Label(new Rect(rect.x, rect.y + 2.0f, 60.0f, 18.0f), " 动画预览");
                    //previewMat.SetFloat("_EvalutionValue", EditorGUI.Slider(new Rect(rect.x + 60.0f, rect.y + 2.0f, 295.0f, 16.0f), previewMat.GetFloat("_EvalutionValue"), 0.0f, 1.0f));
                }
                EditorGUI.EndDisabledGroup();
                // 输出按钮
                if (GUI.Button(new Rect(rect.x + 360.0f, rect.y + 1.0f, 50.0f, 16.0f), "Bake"))
                {
                    // 偏移 0.5个单位到正区间
                    var offset = Vector3.zero;
                    var scale = 0.0f;
                    GetSamplerParams(ref offset, ref scale);

                    var exportPath = GetExportFolder();
                    Mesh mesh = BakeMesh(exportPath, offset, scale);
                    GameObject go = asset as GameObject;


                    Texture2D mainTex = skinMeshRenderer.sharedMaterial.mainTexture as Texture2D;

                    for (int i = 0; i < stateCount; i++)
                    {
                        Texture2D animTex = BakeAnimation(animationStateList[i], exportPath, offset, scale);

                        Material material = BakeMat(animationStateList[i], exportPath, mainTex, animTex, offset, scale);

                        GameObject obj = BakePrefab(animationStateList[i], exportPath, mesh, material, go);
                    }
                }
            }
            EditorGUI.EndDisabledGroup();

            // 预览框
            ifPreviewAnim = PreviewGUI(previewRect);
            // 广告
            //rect = GUILayoutUtility.GetRect(100.0f, 20.0f);
            //EditorGUI.DrawRect(rect, darkColor);


            //GUI.Label(new Rect(rect.x, rect.y + 2.0f, 458.0f, 18.0f), " ---------------------------------------------------------------------------------------------------------");
            //if (GUI.Button(new Rect(rect.x + 463.0f, rect.y + 2.0f, 130.0f, 18.0f), "Present by LookingLu", EditorStyles.label))
            //{
            //    // 作者署名 移除联系LookingLu 不可偷换 注意节操
            //    Application.OpenURL("http://boyantata.com/");
            //}
            //GUI.Label(new Rect(rect.x + 595.0f, rect.y + 2.0f, 40.0f, 18.0f), "------");
            //GUI.Label(new Rect(rect.x + 595.0f, rect.y + 2.0f, 40.0f, 18.0f), "Out:");



        }

        // 模型导入器提取
        ModelImporter ImporterExtractor(Object asset)
        {
            // 输入资源为空时返回null
            if (asset == null) return null;
            // 不能提取ModelImporter时返回null
            var modelImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(asset)) as ModelImporter;
            if (modelImporter == null) return null;
            // 返回importer
            return modelImporter;
        }

        // 模型导入器检查
        bool ImporterChecker(ModelImporter importer)
        {
            // 输入为空时返回False
            if (importer == null) return false;
            // 无修改模型信息的相关设置 Legacy动画模式 导入动画并关闭压缩
            var checker = true;
            if (importer.meshCompression != ModelImporterMeshCompression.Off) checker = false;
            //if (importer.optimizeMesh != false) checker = false;
            if (importer.keepQuads != false) checker = false;
            if (importer.weldVertices != false) checker = false;
            if (importer.animationType != ModelImporterAnimationType.Legacy) checker = false;
            if (importer.generateAnimations != ModelImporterGenerateAnimations.GenerateAnimations) checker = false;
            if (importer.importAnimation != true) checker = false;
            if (importer.animationCompression != ModelImporterAnimationCompression.Off) checker = false;
            return checker;
        }

        // 模型导入器标准化
        void ImporterNormalizer(ModelImporter importer)
        {
            // 无修改模型信息的相关设置 Legacy动画模式 导入动画并关闭压缩
            importer.meshCompression = ModelImporterMeshCompression.Off;
            //importer.optimizeMesh = false;
            importer.keepQuads = false;
            importer.weldVertices = false;
            importer.animationType = ModelImporterAnimationType.Legacy;
            importer.generateAnimations = ModelImporterGenerateAnimations.GenerateAnimations;
            importer.importAnimation = true;
            importer.animationCompression = ModelImporterAnimationCompression.Off;
            // 保存导入器
            importer.SaveAndReimport();
        }

        // 模型资源检查
        bool AssetChecker(Object asset, ref Animation animation, ref List<AnimationState> animationStateList, ref SkinnedMeshRenderer skinMeshRenderer, ref Mesh skinedMesh)
        {
            // 置空缓存
            animation = null;
            animationStateList.Clear();
            skinMeshRenderer = null;
            skinedMesh = null;
            // 输入资源为空时返回False
            if (asset == null)
            {
                //Debug.LogError("asset == null");
                return false;
            }
            // 判断是否可转GameObject
            var assetGO = asset as GameObject;
            if (assetGO == null)
            {
                Debug.LogError("assetGO == null");
                return false;
            }


            // 判断是否含Animation
            var anim = assetGO.GetComponent<Animation>();
            if (anim == null)
            {
                Debug.LogError("anim == null");
                return false;
            }



            //for (int i = 0; i < cast.Count;i++)
            //{
            //    if (item == null || item.GetType() != typeof( AnimationState))
            //    {
            //        Debug.LogError("cast item== null");
            //        return false;
            //    } 
            //}
            // 判断是否含Legacy AnimationClip
            //List<AnimationState> allStates = null; 
            //try
            //{  
            //    allStates = new List<AnimationState>(cast);   
            //}
            //catch (System.Exception e)  
            //{
            //    //Debug.LogError("AssetChecker Error " + e.Message); 
            //    return false; 
            //} 

            List<AnimationState> allStates = null;

            try
            {
                allStates = new List<AnimationState>(anim.Cast<AnimationState>());
            }
            catch (System.InvalidCastException e)
            {
                Debug.LogError("InvalidCastException ");
                return false;
            }


            var animStates = new List<AnimationState>();

            foreach (var state in allStates)
            {
                if (state.clip.legacy == true) animStates.Add(state);
            }



            if (animStates.Count == 0)
            {
                Debug.LogError("animStates.Count == 0");
                return false;
            }


            // 判断是否含SkinMeshRenderer
            var smRenderer = assetGO.GetComponentInChildren<SkinnedMeshRenderer>();
            if (smRenderer == null)
            {
                Debug.LogError("smRenderer == null");
                return false;
            }

            // 判断Renderer的Mesh是否为空
            var skinMesh = smRenderer.sharedMesh;
            if (skinMesh == null)
            {
                Debug.LogError("skinMesh == null");
                return false;
            }


            // 合格资源填充缓存
            animation = anim;
            animationStateList = animStates;
            skinMeshRenderer = smRenderer;
            skinedMesh = skinMesh;

            return true;
        }

        // 计算采样范围
        void GetSamplerParams(ref Vector3 offset, ref float scale)
        {
            switch (sampleMode)
            {
                case SampleMode.Normal: // 普通采样
                    offset = Vector3.zero;
                    scale = 1.0f;
                    return;
                case SampleMode.Optimize: // 优化采样
                    // 计算模型边界
                    var xMin = 0.0f;
                    var yMin = 0.0f;
                    var zMin = 0.0f;
                    var xMax = 0.0f;
                    var yMax = 0.0f;
                    var zMax = 0.0f;
                    var vertexCount = skinedMesh.vertices.Length;
                    foreach (var state in animationStateList)
                    {
                        // 获得帧数量和帧间隔
                        var frameCount = Mathf.ClosestPowerOfTwo((int)(state.clip.frameRate * state.length));
                        var frameDuration = state.length / frameCount;
                        // 烘培动画纹理
                        animation.Play(state.name);
                        var sampleTimer = 0.0f;
                        for (int i = 0; i < frameCount; i++)
                        {
                            state.time = sampleTimer;
                            animation.Sample();
                            skinMeshRenderer.BakeMesh(meshBuffer);
                            for (int j = 0; j < vertexCount; j++)
                            {
                                var vertexPos = meshBuffer.vertices[j];
                                xMin = Mathf.Min(xMin, vertexPos.x);
                                yMin = Mathf.Min(yMin, vertexPos.y);
                                zMin = Mathf.Min(zMin, vertexPos.z);
                                xMax = Mathf.Max(xMax, vertexPos.x);
                                yMax = Mathf.Max(yMax, vertexPos.y);
                                zMax = Mathf.Max(zMax, vertexPos.z);
                            }
                            sampleTimer += frameDuration;
                        }
                    }
                    // 计算模型bound
                    var size = new Vector3(xMax - xMin, yMax - yMin, zMax - zMin);
                    var center = new Vector3(xMax + xMin, yMax + yMin, zMax + zMin) * 0.5f;
                    // 计算平移缩放值
                    offset = center;
                    scale = Mathf.Max(size.x, Mathf.Max(size.y, size.z));
                    offset -= Vector3.one * 0.5f * scale;       // 平移0.5单元至正值区间
                    return;
            }
        }

        // 获得输出路径
        string GetExportFolder(bool creatFolder = true)
        {
            var assetPath = AssetDatabase.GetAssetPath(asset);
            var assetName = Path.GetFileNameWithoutExtension(assetPath);
            var relativePath = Path.GetDirectoryName(assetPath) + "/" + assetName;
            if (creatFolder)
            {
                assetPath = Path.GetDirectoryName(Path.GetFullPath(assetPath)) + '\\' + assetName;
                var fullPath = "";
                foreach (var chr in assetPath)
                {
                    fullPath += chr == '\\' ? '/' : chr;
                }
                Directory.CreateDirectory(fullPath);
            }
            var exportPath = relativePath + '/' + assetName;
            return exportPath;
        }

        // 获得贴图名
        string GetAnimMapPath(string path, AnimationState state)
        {
            var suffix = "";
            switch (bakeAccuracy)
            {
                case BakeAccuracy.Half:
                    suffix = "X16";
                    break;
                case BakeAccuracy.Float:
                    suffix = "X32";
                    break;
            }
            return path + string.Format("_{0}_Anim{1}.asset", state.name, suffix);
        }

        // 获得法线贴图名
        string GetAnimNormalMapPath(string path, AnimationState state)
        {
            var suffix = "";
            switch (bakeAccuracy)
            {
                case BakeAccuracy.Half:
                    suffix = "X16";
                    break;
                case BakeAccuracy.Float:
                    suffix = "X32";
                    break;
            }
            return path + string.Format("_Normal_{0}_Anim{1}.asset", state.name, suffix);
        }

        // 获得材质名
        string GetMaterialPath(string path, AnimationState state)
        {
            var suffix = "";
            switch (bakeAccuracy)
            {
                case BakeAccuracy.Half:
                    suffix = "X16";
                    break;
                case BakeAccuracy.Float:
                    suffix = "X32";
                    break;
            }
            return path + string.Format("_{0}_Anim{1}.mat", state.name, suffix);
        }

        // 获得预设体名字
        string GetPrefabName(string path, AnimationState state)
        {
            var suffix = "";
            switch (bakeAccuracy)
            {
                case BakeAccuracy.Half:
                    suffix = "X16";
                    break;
                case BakeAccuracy.Float:
                    suffix = "X32";
                    break;
            }
            return string.Format("_{0}_Anim{1}.prefab", state.name, suffix);
        }

        // 获得预设体路径
        string GetPrefabPath(string path, AnimationState state)
        {
            var suffix = "";
            switch (bakeAccuracy)
            {
                case BakeAccuracy.Half:
                    suffix = "X16";
                    break;
                case BakeAccuracy.Float:
                    suffix = "X32";
                    break;
            }
            return path + string.Format("_{0}_Anim{1}.prefab", state.name, suffix);
        }


        // 获得网格名
        string GetMeshPath(string path)
        {

            return path + string.Format("_MeshX{0}.asset", subMeshCount);
        }

        // 烘培动画
        Texture2D BakeAnimation(AnimationState state, string path, Vector3 offset, float scale)
        {
            // 获得动画图宽度
            var vertexCount = skinedMesh.vertexCount;
            var texWidth = Mathf.NextPowerOfTwo(vertexCount);
            // 获得帧数量(动画图高度)和帧间隔
            var frameCount = (Mathf.ClosestPowerOfTwo((int)(state.clip.frameRate * state.length)));
            var frameDuration = state.length / frameCount;
            // 烘培动画纹理
            Texture2D animTex = null;

            //TODO:烘焙法线纹理
            //Texture2D normalTex = null;

            switch (bakeAccuracy)
            {
                case BakeAccuracy.Half:
                    animTex = new Texture2D(texWidth, frameCount, TextureFormat.RGBAHalf, false);
                    //normalTex = new Texture2D(texWidth, frameCount, TextureFormat.RGB565, false);
                    break;
                case BakeAccuracy.Float:
                    animTex = new Texture2D(texWidth, frameCount, TextureFormat.RGBAFloat, false);
                    //normalTex = new Texture2D(texWidth, frameCount, TextureFormat.RGBAFloat, false);
                    break;
            }
            animation.Play(state.name);
            var sampleTimer = 0.0f;

            for (int i = 0; i < frameCount; i++)
            {
                state.time = sampleTimer;
                // 采样
                animation.Sample();
                //缓存当前帧顶点数据
                skinMeshRenderer.BakeMesh(meshBuffer);
                for (int j = 0; j < vertexCount; j++)
                {
                    var vertexPos = meshBuffer.vertices[j];
                    // 顶点的信息 index 
                    var adjustedPos = (vertexPos - offset) / scale;
                    animTex.SetPixel(j, i, new Color(adjustedPos.x, adjustedPos.y, adjustedPos.z));

                    //if (copyNormals)
                    //{
                    //    var normalPos = bake_normals[j];
                    //    // 法线信息 index
                    //    normalTex.SetPixel(j, i, new Color(normalPos.x, normalPos.y, normalPos.z));
                    //}
                }
                sampleTimer += frameDuration;
            }

            animTex.Apply();
            // 保存贴图
            AssetDatabase.CreateAsset(animTex, GetAnimMapPath(path, state));

            //if (copyNormals)
            //{
            //    // 保存法线图
            //    normalTex.Apply();
            //    AssetDatabase.CreateAsset(normalTex, GetAnimNormalMapPath(path, state));
            //}
            //else
            //{
            //    normalTex = null;
            //}
            return animTex;
        }

        Material BakeMat(AnimationState state, string path, Texture2D mainTex, Texture2D animTex, Vector3 offset, float scale)
        {
            // 保存Material
            var shader = Shader.Find(GeneralShaderPath);
            var mat = new Material(shader);
            if (mainTex != null) mat.SetTexture("_MainTex", mainTex);

            mat.SetTexture("_AnimMap", animTex);
            mat.SetVector("_SamplerParams", new Vector4(offset.x, offset.y, offset.z, scale));

            AssetDatabase.CreateAsset(mat, GetMaterialPath(path, state));
            return mat;
        }

        GameObject BakePrefab(AnimationState state, string path, Mesh mesh, Material mat, GameObject baseObj)
        {
            string prefabName = baseObj.name;

            var go = GameObject.Find(prefabName);

            if (go == null)
            {
                go = new GameObject(prefabName);
            }

            if (go == null)
            {
                Debug.LogError("GO is Null");
                return null;
            }

            MeshRenderer renderer = go.GetComponent<MeshRenderer>();
            if (renderer == null) renderer = go.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = mat;

            MeshFilter meshFilter = go.GetComponent<MeshFilter>();
            if (meshFilter == null) meshFilter = go.AddComponent<MeshFilter>();

            meshFilter.sharedMesh = mesh;


            go.transform.rotation = baseObj.transform.rotation;
            go.transform.localScale = baseObj.transform.localScale;

            PrefabUtility.SaveAsPrefabAsset(go, GetPrefabPath(path, state));

            return go;
        }


        // 烘培网格
        Mesh BakeMesh(string path, Vector3 offset, float scale)
        {
            // 创建Mesh和数据容器
            var mesh = new Mesh();
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var normals = new List<Vector3>();
            var tangents = new List<Vector4>();
            var uvs = new List<Vector2>();
            var uv2 = new List<Vector2>();
            var uv3 = new List<Vector2>();
            var uv4 = new List<Vector2>();
            var colors = new List<Color>();

            // 填充数据
            var vertexCount = skinedMesh.vertexCount;

            for (int i = 0; i < subMeshCount; i++)
            {
                vertices.AddRange(skinedMesh.vertices);
                var tris = skinedMesh.triangles;
                foreach (var tri in tris)
                {
                    triangles.Add(tri + i * vertexCount);
                }
                normals.AddRange(skinedMesh.normals);
                tangents.AddRange(skinedMesh.tangents);
                colors.AddRange(skinedMesh.colors);
                uvs.AddRange(skinedMesh.uv);
                uv2.AddRange(skinedMesh.uv2);
                uv3.AddRange(skinedMesh.uv3);
                // 建立索引
                for (int j = 0; j < vertexCount; j++)
                {
                    uv4.Add(new Vector2(j, i));
                }
            }
            // 应用数据到Mesh
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            //设置subMesh  daili.ou
            int subMsNum = skinedMesh.subMeshCount;
            for (int i = 0; i < subMsNum; i++)
            {
                mesh.SetSubMesh(i, skinedMesh.GetSubMesh(i));
            }

            if (copyNormals)
            {
                mesh.SetNormals(normals);
                //bake_normals = normals;
            }
            if (copyTangents) mesh.SetTangents(tangents);
            if (copyColors) mesh.SetColors(colors);
            if (copyUV1) mesh.SetUVs(0, uvs);
            if (copyUV2) mesh.SetUVs(1, uv2);
            if (copyUV3) mesh.SetUVs(2, uv3);
            if (copyUV4) mesh.SetUVs(3, uv4);

            //Debug.LogError("GetMeshPath(path) " + GetMeshPath(path));
            // 保存Mesh
            AssetDatabase.CreateAsset(mesh, GetMeshPath(path));
            return mesh;
        }



        // 预览Box
        bool PreviewGUI(Rect rect)
        {
            // Asset未就绪直接返回
            if (assetIsValid == false)
            {

                GUI.Box(rect, "无预览内容");

                return false;
            }

            // 输出资源完备时开启预览 否则预览静态模型
            var previewAnim = false;
            Mesh mesh = null;
            Material mat = null;
            if (stateIndex == 0)
            {
                mesh = skinedMesh;
                mat = wireMat;
            }
            else
            {
                var path = GetExportFolder(false);
                var state = animationStateList[stateIndex - 1];
                var tempMesh = AssetDatabase.LoadAssetAtPath<Mesh>(GetMeshPath(path));
                var tempMat = AssetDatabase.LoadAssetAtPath<Material>(GetMaterialPath(path, state));
                var tempMap = AssetDatabase.LoadAssetAtPath<Texture>(GetAnimMapPath(path, state));

                if (tempMesh == null || tempMat == null || tempMap == null)
                {
                    mesh = skinedMesh;
                    mat = wireMat;

                    //UnityEngine.Debug.LogErrorFormat("tempMesh == null {0} || tempMat == null {1} || tempMap == null {2}", 
                    //    tempMesh == null, tempMat == null, tempMap == null);
                }
                else
                {
                    mesh = tempMesh;
                    mat = previewMat;


                    Texture2D mainTex = skinMeshRenderer.sharedMaterial.mainTexture as Texture2D;
                    if (mainTex != null)
                    {
                        previewMat.SetTexture("_MainTex", mainTex);
                    }

                    var aniMap = previewMat.GetTexture("_AniMap");
                    if (aniMap == null)
                    {
                        previewMat.SetTexture("_AniMap", tempMap);
                    }
                    previewAnim = true;
                }
            }


            // 渲染模型Preview
            previewGUI.BeginPreview(rect, GUIStyle.none);

            previewGUI.DrawMesh(mesh, Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(PreviewRotation.x, PreviewRotation.y, PreviewRotation.z), Vector3.one * PreviewScale), mat, 0);

            previewGUI.camera.Render();
            var texture = previewGUI.EndPreview();
            GUI.Box(rect, texture);
            // 说明文字
            GUI.Label(new Rect(rect.x + 85.0f, rect.y + rect.height - 20.0f, 50.0f, 20.0f), previewAnim ? "GPU动画" : "静态模型");

            // 缩放旋转按钮
            if (GUI.Button(new Rect(rect.x + 5.0f, rect.y + 5.0f, 20.0f, 20.0f), " ↓", EditorStyles.largeLabel))
            {
                PreviewScale -= 0.05f;

            }

            if (GUI.Button(new Rect(rect.x + rect.width - 25.0f, rect.y + 5.0f, 20.0f, 20.0f), " ↑", EditorStyles.largeLabel))
            {
                PreviewScale += 0.05f;
            }



            if (GUI.Button(new Rect(rect.x + 5.0f, rect.y + rect.height - 25.0f, 20.0f, 20.0f), "X+5", EditorStyles.largeLabel))
            {
                PreviewRotation.x += 5.0f;
            }

            if (GUI.Button(new Rect(rect.x + rect.width - 25.0f, rect.y + rect.height - 25.0f, 20.0f, 20.0f), "X-5", EditorStyles.largeLabel))
            {
                PreviewRotation.x -= 5.0f;
            }

            if (GUI.Button(new Rect(rect.x + 25.0f, rect.y + rect.height - 25.0f, 20.0f, 20.0f), "Y+5", EditorStyles.largeLabel))
            {
                PreviewRotation.y += 5.0f;
            }
            if (GUI.Button(new Rect(rect.x + rect.width - 50.0f, rect.y + rect.height - 25.0f, 20.0f, 20.0f), "Y-5", EditorStyles.largeLabel))
            {
                PreviewRotation.y -= 5.0f;
            }


            if (GUI.Button(new Rect(rect.x + 50.0f, rect.y + rect.height - 25.0f, 20.0f, 20.0f), "Z+5", EditorStyles.largeLabel))
            {
                PreviewRotation.z += 5.0f;
            }

            if (GUI.Button(new Rect(rect.x + rect.width - 75.0f, rect.y + rect.height - 25.0f, 20.0f, 20.0f), "Z-5", EditorStyles.largeLabel))
            {
                PreviewRotation.z -= 5.0f;
            }



            //if (GUI.Button(new Rect(rect.x + 5.0f, rect.y + rect.height - 25.0f, 20.0f, 20.0f), " ↻", EditorStyles.largeLabel))
            //{
            //    PreviewRotation += 5.0f;
            //}
            //if (GUI.Button(new Rect(rect.x + rect.width - 25.0f, rect.y + rect.height - 25.0f, 20.0f, 20.0f), " ↺", EditorStyles.largeLabel))
            //{
            //    PreviewRotation -= 5.0f;
            //}



            // 返回是否预览动画中
            return previewAnim;
        }

        bool SelectedAndBake(string path)
        {

            return true;
        }
    }
}