//using System.IO;
//using UnityEngine;
//using UnityEditor;
//using System.Text.RegularExpressions;

//public class ProcessorSprite : AssetPostprocessor
//{

//    //纹理导入器, pre-process
//    void OnPreprocessTexture()
//    {
//        //获得importer实例
//        TextureImporter tImporter = assetImporter as TextureImporter;

//        //设置Read/Write Enabled开关,不勾选
//        tImporter.isReadable = false;

//        if (tImporter.assetPath.StartsWith("Assets/ArtAssets/ui/sprites"))
//        {
//            //设置UI纹理Generate Mipmaps开关,不勾选
//            tImporter.mipmapEnabled = false;
//            //设置UI纹理WrapMode开关,Clamp
//            tImporter.wrapMode = TextureWrapMode.Clamp;
//        }
//        else
//        {
//            return;
//        }

//        //设置压缩格式
//        TextureImporterPlatformSettings psAndroid = tImporter.GetPlatformTextureSettings("Android");

//#if UNITY_IOS
//        TextureImporterPlatformSettings psIPhone = tImporter.GetPlatformTextureSettings("iPhone");
//#endif
//        psAndroid.overridden = true;
//#if UNITY_IOS
//		psIPhone.overridden = true;
//#endif
//        if (tImporter.DoesSourceTextureHaveAlpha())
//        {
//            psAndroid.format = TextureImporterFormat.ASTC_6x6;
//#if UNITY_IOS
//			psIPhone.format = TextureImporterFormat.ASTC_6x6;
//#endif
//        }
//        else
//        {
//            psAndroid.format = TextureImporterFormat.ASTC_4x4;
//#if UNITY_IOS
//			psIPhone.format = TextureImporterFormat.ASTC_4x4;
//#endif
//        }
//        tImporter.SetPlatformTextureSettings(psAndroid);
//#if UNITY_IOS
//		tImporter.SetPlatformTextureSettings(psIPhone);
//#endif
//    }

//    [MenuItem("公共工具/资源检测/Sprite命名及尺寸检测")]
//    static public void AutoValidate()
//    {
//        //写入csv日志
//        string floderName = Application.dataPath.Replace("Assets", "资源检测");
//        if (!Directory.Exists(floderName))
//        {
//            Directory.CreateDirectory(floderName);
//        }
//        string fileName = string.Format("{0}/ValidateTexture.csv", floderName);

//        StreamWriter sw = new StreamWriter("资源检测/ValidateTexture.csv", false, System.Text.Encoding.UTF8);
//        sw.WriteLine("Validate -- Validate Textures");

//        string[] allAssets = AssetDatabase.GetAllAssetPaths();
//        foreach (string s in allAssets)
//        {
//            if (s.StartsWith("Assets/ArtAssets/ui/sprites"))
//            {
//                Texture tex = AssetDatabase.LoadAssetAtPath(s, typeof(Texture)) as Texture;

//                if (tex)
//                {
//                    //检测纹理资源命名是否合法
//                    if (!Regex.IsMatch(s, @"^[a-zA-Z][a-zA-Z0-9_/.]*$"))
//                    {
//                        sw.WriteLine(string.Format("illegal texture filename,{0}", s));
//                    }

//                    //判断纹理尺寸是否符合四的倍数
//                    if (((tex.width % 4) != 0) || ((tex.height % 4) != 0))
//                    {
//                        sw.WriteLine(string.Format("illegal texture W/H size,{0},{1},{2}", s, tex.width, tex.height));
//                    }
//                }
//            }
//        }

//        sw.Flush();
//        sw.Close();

//        Debug.LogFormat("检测完成，输出路径: {0}", fileName);
//    }
//}
