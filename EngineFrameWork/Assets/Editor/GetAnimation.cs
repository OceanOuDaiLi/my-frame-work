using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class GetAnimation
{
    [MenuItem("AnimationTool/GetAnimation", true)]
    static bool NotSelection()
    {
        return Selection.activeObject;     //�ж��Ƿ�ѡ������� ûѡ��Ļ��޷�ִ�й���
    }

    [MenuItem("AnimationTool/GetAnimation")]
    static void Get()
    {
        string targetPath = Application.dataPath + "/AnimationClip";          //Ŀ¼AnimationClip
        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);     //���Ŀ¼�����ھʹ���һ��
        }
        UnityEngine.Object[] objects = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Unfiltered);     //��ȡ����ѡ�е�����
        foreach (UnityEngine.Object o in objects)     //����ѡ�������
        {
            AnimationClip clip = new AnimationClip();      //newһ��AnimationClip������ɵ�AnimationClip
            string fbxPath = AssetDatabase.GetAssetPath(o);       //FBX�ĵ�ַ
            string name = o.name;     //FBX������
            AnimationClip fbxClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(fbxPath);     //��ȡFBX�ϵ�animationClip
            if (fbxClip == null)
            {
                Debug.Log("��ǰѡ����ļ����Ǵ���AnimationClip��FBX�ļ�");
            }
            else
            {
                EditorUtility.CopySerialized(fbxClip, clip);    //����
                AssetDatabase.CreateAsset(clip, "Assets/AnimationClip/" + name + ".anim");    //�����ļ�
            }
        }
    }
}