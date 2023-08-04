//using System.IO;
//using UnityEngine;
//using UnityEditor;
//using System.Text.RegularExpressions;

//public class ProcessorSprite : AssetPostprocessor
//{

//    //��������, pre-process
//    void OnPreprocessTexture()
//    {
//        //���importerʵ��
//        TextureImporter tImporter = assetImporter as TextureImporter;

//        //����Read/Write Enabled����,����ѡ
//        tImporter.isReadable = false;

//        if (tImporter.assetPath.StartsWith("Assets/ArtAssets/ui/sprites"))
//        {
//            //����UI����Generate Mipmaps����,����ѡ
//            tImporter.mipmapEnabled = false;
//            //����UI����WrapMode����,Clamp
//            tImporter.wrapMode = TextureWrapMode.Clamp;
//        }
//        else
//        {
//            return;
//        }

//        //����ѹ����ʽ
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

//    [MenuItem("��������/��Դ���/Sprite�������ߴ���")]
//    static public void AutoValidate()
//    {
//        //д��csv��־
//        string floderName = Application.dataPath.Replace("Assets", "��Դ���");
//        if (!Directory.Exists(floderName))
//        {
//            Directory.CreateDirectory(floderName);
//        }
//        string fileName = string.Format("{0}/ValidateTexture.csv", floderName);

//        StreamWriter sw = new StreamWriter("��Դ���/ValidateTexture.csv", false, System.Text.Encoding.UTF8);
//        sw.WriteLine("Validate -- Validate Textures");

//        string[] allAssets = AssetDatabase.GetAllAssetPaths();
//        foreach (string s in allAssets)
//        {
//            if (s.StartsWith("Assets/ArtAssets/ui/sprites"))
//            {
//                Texture tex = AssetDatabase.LoadAssetAtPath(s, typeof(Texture)) as Texture;

//                if (tex)
//                {
//                    //���������Դ�����Ƿ�Ϸ�
//                    if (!Regex.IsMatch(s, @"^[a-zA-Z][a-zA-Z0-9_/.]*$"))
//                    {
//                        sw.WriteLine(string.Format("illegal texture filename,{0}", s));
//                    }

//                    //�ж�����ߴ��Ƿ�����ĵı���
//                    if (((tex.width % 4) != 0) || ((tex.height % 4) != 0))
//                    {
//                        sw.WriteLine(string.Format("illegal texture W/H size,{0},{1},{2}", s, tex.width, tex.height));
//                    }
//                }
//            }
//        }

//        sw.Flush();
//        sw.Close();

//        Debug.LogFormat("�����ɣ����·��: {0}", fileName);
//    }
//}
