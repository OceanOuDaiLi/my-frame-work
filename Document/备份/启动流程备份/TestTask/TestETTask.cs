using ET;
using System.IO;
using UnityEngine;
using Core.Interface.IO;
using System.Collections.Generic;
using FrameWork.Launch;


public class TestETTask : MonoBehaviour
{
    void Start()
    {
        TestUnZipAssetBundle().Coroutine();
    }

    public async ETTask<Dictionary<string, byte[]>> TestUnZipAssetBundle()
    {
        ETTask tcs = ETTask.Create(true);
        Dictionary<string, byte[]> dictBytes = new Dictionary<string, byte[]>();

        IDisk streamingDisk = IOHelper.StreamingDisk;
        IDirectory androidDir = streamingDisk.Directory("IOS"/*IOHelper.PlatformToName(RuntimePlatform.Android)*/);

        IFile zipFile = androidDir.File("AssetZip");

        zipFile.ReadAsync((bytes) =>
        {
            using var ms = new MemoryStream(bytes, 0, bytes.Length, false, true);
            using var output = new MemoryStream(bytes.Length);
            //��Դ��ѹ�����...
            Debug.Log("��Դ��ѹ�����...");
            if (!ZipHelper.Decompress(ms, output))
            {
                Debug.Log($"��Դ�쳣�����¿�ʼ��");
                dictBytes = null;
            }

            output.Seek(0, SeekOrigin.Begin);
            // ��Դ��ѹ��ѹ��... 0 - 50/100
            dictBytes = new Dictionary<string, byte[]>();
            {
                using var br = new BinaryReader(output);
                var count = br.ReadInt32();
                for (var i = 0; i < count; ++i)
                {
                    var fn = br.ReadString();
                    var cnt = br.ReadInt32();
                    var buf = br.ReadBytes(cnt);
                    // progress..
                    dictBytes.Add(fn, buf);
                    float result = (float)((float)i / (float)count) * 0.5f;
                    double _re = Mathf.Round(result * 100f) / 100f;
                    Debug.LogFormat($"��Դ��ѹ��ѹ��... {_re * 100}/100");
                }
            }

            // ��Դд���� ... 50 - 100/100

            tcs.SetResult();
            tcs = null;
        });

        await tcs;
        //foreach (var item in dictBytes)
        //{
        //    UnityEngine.Debug.LogFormat($"fileName: {item.Key} Len: {item.Value.Length} ");
        //}
        return dictBytes;
    }

}
