// 开发日志 Bound位置不对 需要处理

using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;

namespace GPUAnim
{

    ///<summary>
    /// 
    /// 烘焙
    /// 
    /// </summary>
    public class GPUAnimUtil : EditorWindow
    {
        // 路径常量
        static readonly string rootPath = "Assets/ArtResource/ArtScripts/Animation/GPUAnimBaker/";
        static readonly string WireShaderPath = "GPUAnim/MeshWire";
        static readonly string PreviewShaderPath = "GPUAnim/Preview";
        static readonly string GeneralShaderPath = "Next_E/11_GPUAnim/0_General";


        public enum BakeAccuracy
        {
            Fixed,
            Half,
            Float
        }

        static readonly string[] BakeAccuracyLables = new string[]
        {
            "8位精度",
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




        public static string Bake(string assetPath,ref  bool Result)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();

            Object asset = Resources.Load(assetPath);
            Result = true;
            if (asset == null)
            {
                asset = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(assetPath);

            }

            if (asset == null)
            {
                builder.Append(string.Format("加载{0}失败！\n", assetPath));
                Result = false;

                return builder.ToString();

            }

            int subMeshCount = 1;

            ModelImporter importer = ImporterExtractor(assetPath);

            bool assetIsValid = ImporterChecker(importer);

            if (assetIsValid == false)
            {
                builder.Append("进行格式化\n");

                ImporterNormalizer(importer);
            }

            assetIsValid = ImporterChecker(importer);

            if (assetIsValid == false)
            {

                Debug.LogError("格式化失败");
                builder.Append("格式化失败\n");
                Result = false;
                return builder.ToString();
            }

            builder.Append("进行资源检查\n");

            Animation animation = null;
            List<AnimationState> animationStateList = new List<AnimationState>();
            SkinnedMeshRenderer skinMeshRenderer = null;
            Mesh skinedMesh = null;
            SampleMode sampleMode = SampleMode.Normal;
            Mesh meshBuffer = new Mesh();
            // 检查模型信息

            // 检查Asset资源
            assetIsValid &= AssetChecker(asset, ref animation, ref animationStateList, ref skinMeshRenderer, ref skinedMesh);


            if (assetIsValid == false)
            {
                Debug.LogError("资源检查不通过");
                builder.Append("资源检查不通过\n");
                Result = false;
                return builder.ToString();
            }

            if (animation == null || animationStateList == null || animationStateList.Count == 0)
            {
                Debug.LogError("动画不存在\n");

                builder.Append("动画不存在\n");
                Result = false;
                return builder.ToString();
            }

            if (skinMeshRenderer == null)
            {
                Debug.LogError("skinMeshRenderer 不存在\n");
                builder.Append("skinMeshRenderer 不存在\n");
                Result = false;
                return builder.ToString();
            }

            if (skinedMesh == null)
            {
                Debug.LogError("skinedMesh 不存在\n");
                builder.Append("skinedMesh 不存在\n");
                Result = false;
                return builder.ToString();
            }


            int vertCount = skinedMesh.vertexCount;
            bool hasNormals = skinedMesh.normals.Length != 0;
            bool hasTangents = skinedMesh.tangents.Length != 0;
            bool hasColors = skinedMesh.colors.Length != 0;
            bool hasUV1 = skinedMesh.uv.Length != 0;
            bool hasUV2 = skinedMesh.uv2.Length != 0;
            bool hasUV3 = skinedMesh.uv3.Length != 0;
            bool hasUV4 = skinedMesh.uv4.Length != 0;

            Vector3 offset = Vector3.zero;
            float scale = 0.0f;


            GetSamplerParams(skinedMesh, animationStateList, animation, skinMeshRenderer, meshBuffer,
              sampleMode, ref offset, ref scale);

            var exportPath = GetExportFolder(assetPath);

            Mesh mesh = BakeMesh(
                    skinedMesh,
                    subMeshCount,
                    hasNormals,
                    hasTangents,
                    hasColors,
                    hasUV1,
                    hasUV2,
                    hasUV3,
                    hasUV4,
                    exportPath,
                    offset,
                    scale);

            GameObject go = asset as GameObject;

            Texture2D mainTex = skinMeshRenderer.sharedMaterial.mainTexture as Texture2D;

            int stateCount = animationStateList.Count;


            for (int i = 0; i < stateCount; i++)
            {
                AnimationState state = animationStateList[i];

                Texture2D animTex = BakeAnimation(skinedMesh, animation, skinMeshRenderer, meshBuffer,
                    state, exportPath, offset, scale);

                builder.Append(string.Format("生成Texture {0} \n", AssetDatabase.GetAssetPath(animTex)));


                Material material = BakeMat(state, exportPath, mainTex, animTex, offset, scale);

                builder.Append(string.Format("生成Material {0} \n", AssetDatabase.GetAssetPath(material)));

                GameObject obj = BakePrefab(state, exportPath, mesh, material, go);

                builder.Append(string.Format("生成Prefab {0} \n", AssetDatabase.GetAssetPath(obj)));

            }


            return builder.ToString();

        }

        public   static string  BakeASync(Object asset,string assetPath)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();

                int subMeshCount = 1;

                ModelImporter importer = ImporterExtractor(assetPath);

                bool assetIsValid = ImporterChecker(importer);

                if (assetIsValid == false)
                {
                    builder.Append("进行格式化\n");

                    ImporterNormalizer(importer);
                }
                assetIsValid = ImporterChecker(importer);

                if (assetIsValid == false)
                {

                    Debug.LogError("格式化失败");
                    builder.Append("格式化失败\n");

                   return builder.ToString();
                     
                }

                builder.Append("进行资源检查\n");

                Animation animation = null;

                List<AnimationState> animationStateList = new List<AnimationState>();

                SkinnedMeshRenderer skinMeshRenderer = null;

                Mesh skinedMesh = null;

                SampleMode sampleMode = SampleMode.Normal;

                Mesh meshBuffer = new Mesh();
                // 检查模型信息

                // 检查Asset资源
                assetIsValid &= AssetChecker(asset, ref animation, ref animationStateList, ref skinMeshRenderer, ref skinedMesh);


                if (assetIsValid == false)
                {
                    Debug.LogError("资源检查不通过");
                    builder.Append("资源检查不通过\n");

                return builder.ToString();
                   
                }

                if (animation == null || animationStateList == null || animationStateList.Count == 0)
                {
                    builder.Append("动画不存在\n");
                return builder.ToString();
                   
                }

                if (skinMeshRenderer == null)
                {
                    builder.Append("skinMeshRenderer 不存在\n");
                return builder.ToString();
                    
                }

                if (skinedMesh == null)
                {
                    builder.Append("skinedMesh 不存在\n");
                return builder.ToString();
                    
                }


                int vertCount = skinedMesh.vertexCount;
                bool hasNormals = skinedMesh.normals.Length != 0;
                bool hasTangents = skinedMesh.tangents.Length != 0;
                bool hasColors = skinedMesh.colors.Length != 0;
                bool hasUV1 = skinedMesh.uv.Length != 0;
                bool hasUV2 = skinedMesh.uv2.Length != 0;
                bool hasUV3 = skinedMesh.uv3.Length != 0;
                bool hasUV4 = skinedMesh.uv4.Length != 0;

                Vector3 offset = Vector3.zero;
                float scale = 0.0f;


                GetSamplerParams(skinedMesh, animationStateList, animation, skinMeshRenderer, meshBuffer,
                  sampleMode, ref offset, ref scale);

                var exportPath = GetExportFolder(assetPath);

                Mesh mesh = BakeMesh(
                        skinedMesh,
                        subMeshCount,
                        hasNormals,
                        hasTangents,
                        hasColors,
                        hasUV1,
                        hasUV2,
                        hasUV3,
                        hasUV4,
                        exportPath,
                        offset,
                        scale);

                GameObject go = asset as GameObject;

                Texture2D mainTex = skinMeshRenderer.sharedMaterial.mainTexture as Texture2D;

                int stateCount = animationStateList.Count;


                for (int i = 0; i < stateCount; i++)
                {
                    AnimationState state = animationStateList[i];

                    Texture2D animTex = BakeAnimation(skinedMesh, animation, skinMeshRenderer, meshBuffer,
                        state, exportPath, offset, scale);

                    builder.Append(string.Format("生成纹理 {0} \n", AssetDatabase.GetAssetPath(animTex)));


                    Material material = BakeMat(state, exportPath, mainTex, animTex, offset, scale);

                    builder.Append(string.Format("生成材质 {0} \n", AssetDatabase.GetAssetPath(animTex)));

                    GameObject obj = BakePrefab(state, exportPath, mesh, material, go);

                    builder.Append(string.Format("生成预设体 {0} \n", AssetDatabase.GetAssetPath(obj)));

                }


               return builder.ToString();
                 


        }


        // 模型导入器提取 
        static ModelImporter ImporterExtractor(string filePath)
        {
            // 输入资源为空时返回null
            if (string.IsNullOrEmpty(filePath))
            {
                return null;
            }
            // 不能提取ModelImporter时返回null
            ModelImporter modelImporter = AssetImporter.GetAtPath(filePath) as ModelImporter;

            if (modelImporter == null) return null;
            // 返回importer
            return modelImporter;
        }

        // 模型导入器检查
        static bool ImporterChecker(ModelImporter importer)
        {
            // 输入为空时返回False
            if (importer == null)
            {
                return false;
            }

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
        static void ImporterNormalizer(ModelImporter importer)
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
        static bool AssetChecker(Object asset, ref Animation animation, ref List<AnimationState> animationStateList, ref SkinnedMeshRenderer skinMeshRenderer, ref Mesh skinedMesh)
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
        static void GetSamplerParams(Mesh skinedMesh, List<AnimationState> animationStateList, Animation animation,
            SkinnedMeshRenderer skinMeshRenderer, Mesh meshBuffer, SampleMode sampleMode, ref Vector3 offset, ref float scale)
        {
            switch (sampleMode)
            {
                case SampleMode.Normal:
                    offset = Vector3.zero;
                    scale = 1.0f;
                    return;
                case SampleMode.Optimize:
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
        static string GetExportFolder(string assetPath)
        {
            bool creatFolder = true;
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
        static string GetAnimMapPath(string path, AnimationState state)
        {
            var suffix = "";
            switch (bakeAccuracy)
            {
                case BakeAccuracy.Fixed:
                    suffix = "X8";
                    break;
                case BakeAccuracy.Half:
                    suffix = "X16";
                    break;
                case BakeAccuracy.Float:
                    suffix = "X32";
                    break;
            }
            return path + string.Format("_{0}_Anim{1}.asset", state.name, suffix);
        }

        // 获得材质名
        static string GetMaterialPath(string path, AnimationState state)
        {
            var suffix = "";
            switch (bakeAccuracy)
            {
                case BakeAccuracy.Fixed:
                    suffix = "X8";
                    break;
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
        static string GetPrefabName(string path, AnimationState state)
        {
            var suffix = "";
            switch (bakeAccuracy)
            {
                case BakeAccuracy.Fixed:
                    suffix = "X8";
                    break;
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
        static string GetPrefabPath(string path, AnimationState state)
        {
            var suffix = "";
            switch (bakeAccuracy)
            {
                case BakeAccuracy.Fixed:
                    suffix = "X8";
                    break;
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
        static string GetMeshPath(int subMeshCount, string path)
        {

            return path + string.Format("_MeshX{0}.asset", subMeshCount);
        }



        // 烘培动画
        static Texture2D BakeAnimation(Mesh skinedMesh, Animation animation,
         SkinnedMeshRenderer skinMeshRenderer, Mesh meshBuffer,
            AnimationState state, string path, Vector3 offset, float scale)
        {
            // 获得动画图宽度
            var vertexCount = skinedMesh.vertexCount;
            var texWidth = Mathf.NextPowerOfTwo(vertexCount);
            // 获得帧数量(动画图高度)和帧间隔
            var frameCount = (Mathf.ClosestPowerOfTwo((int)(state.clip.frameRate * state.length)));
            var frameDuration = state.length / frameCount;
            // 烘培动画纹理
            Texture2D animTex = null;
            switch (bakeAccuracy)
            {
                case BakeAccuracy.Fixed:
                    animTex = new Texture2D(texWidth, frameCount, TextureFormat.RGB24, false);
                    break;
                case BakeAccuracy.Half:
                    animTex = new Texture2D(texWidth, frameCount, TextureFormat.RGB565, false);
                    break;
                case BakeAccuracy.Float:
                    animTex = new Texture2D(texWidth, frameCount, TextureFormat.RGBAFloat, false);
                    break;
            }
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
                    var adjustedPos = (vertexPos - offset) / scale;
                    animTex.SetPixel(j, i, new Color(adjustedPos.x, adjustedPos.y, adjustedPos.z));
                }
                sampleTimer += frameDuration;
            }
            animTex.Apply();
            // 保存贴图
            AssetDatabase.CreateAsset(animTex, GetAnimMapPath(path, state));

            return animTex;
        }


        static Material BakeMat(AnimationState state, string path, Texture2D mainTex, Texture2D animTex, Vector3 offset, float scale)
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

        static GameObject BakePrefab(AnimationState state, string path, Mesh mesh, Material mat, GameObject baseObj)
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
        static Mesh BakeMesh(Mesh skinedMesh, int subMeshCount, bool copyNormals, bool copyTangents, bool copyColors,
            bool copyUV1, bool copyUV2, bool copyUV3, bool copyUV4, string path, Vector3 offset, float scale)
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
                for (int j = 0; j < vertexCount; j++)
                {
                    uv4.Add(new Vector2(j, i));
                }
            }
            // 应用数据到Mesh
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            if (copyNormals) mesh.SetNormals(normals);
            if (copyTangents) mesh.SetTangents(tangents);
            if (copyColors) mesh.SetColors(colors);
            if (copyUV1) mesh.SetUVs(0, uvs);
            if (copyUV2) mesh.SetUVs(1, uv2);
            if (copyUV3) mesh.SetUVs(2, uv3);
            if (copyUV4) mesh.SetUVs(3, uv4);

            //Debug.LogError("GetMeshPath(path) " + GetMeshPath(path));
            // 保存Mesh
            AssetDatabase.CreateAsset(mesh, GetMeshPath(subMeshCount, path));

            return mesh;
        }


    }
}