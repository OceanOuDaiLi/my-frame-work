    public static void TestUnZipAssetBundle()
    {
        UnInit();
        string srcFullDir = AssetBundlesMaker.StandardPath(_pacakageReleaseChachedDir.Path + Path.AltDirectorySeparatorChar);
        var dstFile = srcFullDir + "Test";
        byte[] bytes = File.ReadAllBytes(dstFile);
        UnityEngine.Debug.Log($"[GameRoot::LoadData] begin decompressfast....{dstFile}");
        using var ms = new MemoryStream(bytes, 0, bytes.Length, false, true);
        using var output = new MemoryStream(bytes.Length);
        if (!ZipHelper.Decompress(ms, output))
        {
            UnityEngine.Debug.Log($"[GameRoot::LoadData] DecompressFast {dstFile} error!");
            return;
        }

        UnityEngine.Debug.Log($"[GameRoot::LoadData] end decompressfast....{dstFile}");

        output.Seek(0, SeekOrigin.Begin);
        var dictBytes = new Dictionary<string, byte[]>();
        {
            using var br = new BinaryReader(output);
            var count = br.ReadInt32();
            for (var i = 0; i < count; ++i)
            {
                var fn = br.ReadString();
                var cnt = br.ReadInt32();
                var buf = br.ReadBytes(cnt);

                dictBytes.Add(fn, buf);
            }
        }

        foreach (var item in dictBytes)
        {
            UnityEngine.Debug.LogFormat($"fileName: {item.Key} Len: {item.Value.Length} ");
        }
    }