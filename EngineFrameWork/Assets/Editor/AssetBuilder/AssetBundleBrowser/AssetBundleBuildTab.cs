using System.IO;
using UnityEditor;
using UnityEngine;
using FrameWork;
using System.Text;
using FrameWork.Launch;
using Core.AssetBuilder;
using Core.Interface.IO;
using System.Collections.Generic;

namespace AssetBundleBrowser
{
    [System.Serializable]
    internal class AssetBundleBuildTab
    {
        GUIStyle tipStyle;
        GUIStyle _badgeStyle;

        long totalSize;
        string updateTips;
        UpdateFile updateFile;

        bool close = false;
        bool _isBuildFlodout;
        bool _isBuildSubPkgFlodout;
        bool _isBuildSettingFlodout;
        bool _isBuildSplitPkgFlodout;
        bool _isEnCryptSettingFlodout;

        private GUIContent _addTextureCnt;
        private GUIContent _deleteTextureCnt;
        private GUIContent _warningTextureCnt;

        AssetBundleSplitSetting currentSplitSetting = null;
        AssetBundleBuildSetting currentBuildSetting = null;

        Vector2 m_ScrollPosition;

        List<ToggleData> toggleDatas;
        internal enum BuildOption
        {
            BUILD_ATLAS = 0,
            BUILD_HCLR_AOT = 1,
            BUILD_FIRST_PKG = 2,
            BUILD_HOT_FIXED = 3,
            BUILD_ALL = 4,
            BUILD_NULL = 5,
        }
        public class ToggleData
        {
            public bool clicked = false;
            public string title = string.Empty;
            public string toolTips = string.Empty;

            public GUIContent content;
            public BuildOption option = BuildOption.BUILD_ATLAS;
            public ToggleData(string title, string toolTips, BuildOption option)
            {
                this.title = title;
                this.toolTips = toolTips;

                this.option = option;
                content = new GUIContent(title, toolTips);
            }
        }


        internal void OnEnable(EditorWindow parent)
        {
            currentBuildSetting = AssetBundlesMaker.LoadAssetBundleSetting();
            currentSplitSetting = AssetBundlesMaker.LoadAssetBundleSplitSetting();
        }

        internal void OnDisable()
        {
            close = true;
        }

        /* ---------------------------*/
        /* -----------GUI-------------*/
        /* ---------------------------*/
        internal void OnGUI()
        {
            if (tipStyle == null)
            {
                tipStyle = new GUIStyle(EditorStyles.toolbarButton);
                tipStyle.fontSize = 13;
                tipStyle.alignment = TextAnchor.MiddleLeft;
                tipStyle.normal.textColor = Color.white;
                tipStyle.hover.textColor = Color.green;
            }
            if (_badgeStyle == null)
            {
                _badgeStyle = new GUIStyle(EditorStyles.toolbarButton);
                _badgeStyle.normal.textColor = Color.white;
                _badgeStyle.active.textColor = Color.white;
                _badgeStyle.focused.textColor = Color.white;
                _badgeStyle.hover.textColor = Color.green;
                _badgeStyle.alignment = TextAnchor.MiddleCenter;
            }

            if (_addTextureCnt == null || _deleteTextureCnt == null)
            {
                _addTextureCnt = EditorGUIUtility.IconContent("d_Toolbar Plus");
                _deleteTextureCnt = EditorGUIUtility.IconContent("d_winbtn_win_close");
                _warningTextureCnt = EditorGUIUtility.IconContent("console.warnicon.sml");
            }

            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);

            EditorGUILayout.Space();
            GUILayout.BeginVertical();

            // Options GUIStyle
            GUIStyle style = new GUIStyle(EditorStyles.toolbarButton);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = 14;

            // Build Options
            EditorGUILayout.Space(5);
            string tips = "Build  Options";
            string caption = (_isBuildFlodout) ? string.Format("▼ {0}", tips) : string.Format("► {0}", tips);
            _isBuildFlodout = GUILayout.Toggle(_isBuildFlodout, caption, style, GUILayout.Width(250));
            EditorGUILayout.Space(5);
            DrawBuildOptions();

            // Build Settings
            EditorGUILayout.Space(5);
            tips = "Build Settings";
            caption = (_isBuildSettingFlodout) ? string.Format("▼ {0}", tips) : string.Format("► {0}", tips);
            _isBuildSettingFlodout = GUILayout.Toggle(_isBuildSettingFlodout, caption, style, GUILayout.Width(250));
            DrawBuildSettings();

            // EnCrypt Settings
            EditorGUILayout.Space(5);
            tips = "EnCrypt Settings";
            caption = (_isEnCryptSettingFlodout) ? string.Format("▼ {0}", tips) : string.Format("► {0}", tips);
            _isEnCryptSettingFlodout = GUILayout.Toggle(_isEnCryptSettingFlodout, caption, style, GUILayout.Width(250));
            DrawAssetEnCryptSettings();

            // Sub Bundle Options
            EditorGUILayout.Space(5);
            tips = "Zip Bundle Options";
            caption = (_isBuildSubPkgFlodout) ? string.Format("▼ {0}", tips) : string.Format("► {0}", tips);
            _isBuildSubPkgFlodout = GUILayout.Toggle(_isBuildSubPkgFlodout, caption, style, GUILayout.Width(250));
            DrawBuildSubPkgSettings();

            // SplitBundle Options
            EditorGUILayout.Space(5);
            tips = "Split First Package Options";
            caption = (_isBuildSplitPkgFlodout) ? string.Format("▼ {0}", tips) : string.Format("► {0}", tips);
            _isBuildSplitPkgFlodout = GUILayout.Toggle(_isBuildSplitPkgFlodout, caption, style, GUILayout.Width(250));
            DrawBuildSplitPkgSettings();

            GUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        private void DrawLabelTitle(GUIStyle style, string tips, GUILayoutOption opts = null)
        {
            style.alignment = TextAnchor.MiddleLeft;
            style.fontSize = 14;
            style.normal.textColor = Color.yellow;

            if (opts != null)
            {
                GUILayout.Label(tips, style, opts);
            }
            else
            {
                GUILayout.Label(tips, style);
            }
            GUILayout.Space(8);
        }

        private void DrawBuildOptions()
        {
            if (_isBuildFlodout)
            {

                toggleDatas = new List<ToggleData>();
                toggleDatas.Add(new ToggleData
                    ("调试 - 构建UI图集",
                    "根据规范目录，将UI Sprites 打包到AssetBundle下图集目录",
                    BuildOption.BUILD_ATLAS)
                    );
                toggleDatas.Add(new ToggleData
                    ("[ -------pkg------- ]",
                    "换行",
                    BuildOption.BUILD_NULL)
                    );

                toggleDatas.Add(new ToggleData
                    ("调试 - 构建AOT",
                    "增删改VS工程，或首次打包HCLR热更新，或修改AOT代码时，构建一次AOT",
                    BuildOption.BUILD_HCLR_AOT)
                    );

                toggleDatas.Add(new ToggleData
                    ("调试 - 构建应用",
                    "构建首包",
                    BuildOption.BUILD_FIRST_PKG)
                    );
                toggleDatas.Add(new ToggleData
                    ("调试 - 构建热更",
                    "构建热更新",
                    BuildOption.BUILD_HOT_FIXED)
                    );
                toggleDatas.Add(new ToggleData
                    ("[ -------pkg------- ]",
                    "换行",
                    BuildOption.BUILD_NULL)
                    );
                toggleDatas.Add(new ToggleData
                    ("[ -------提示------- ]",
                    "此面板仅用于测试分析用，出包使用Jenkins出对应渠道包",
                    BuildOption.BUILD_NULL)
                    );

                foreach (var item in toggleDatas)
                {
                    bool clicked = GUILayout.Toggle(item.clicked, item.content, _badgeStyle, GUILayout.Width(250));
                    if (clicked)
                    {
                        switch (item.option)
                        {
                            case BuildOption.BUILD_ATLAS:
                                TipsAlertWindow.ShowAlertWithBtn(item.title, item.toolTips + "\n" + "是否构建图集？", BuildAltas);
                                break;
                            case BuildOption.BUILD_HCLR_AOT:
                                TipsAlertWindow.ShowAlertWithBtn(item.title, item.toolTips + "\n" + "是否构建HCLR AOT？", BuildHCLRAot);
                                break;
                            case BuildOption.BUILD_FIRST_PKG:
                                TipsAlertWindow.ShowAlertWithBtn(item.title, item.toolTips + "\n" + "是否构建首包？", BuildFirstPkg);
                                break;
                            case BuildOption.BUILD_HOT_FIXED:
                                TipsAlertWindow.ShowAlertWithBtn(item.title, item.toolTips + "\n" + "是否构建热更新？", BuildHotFixed);
                                break;
                            default:
                                break;
                        }
                    }

                    EditorGUILayout.Space(5);
                }
            }
        }

        private void DrawBuildSettings()
        {
            if (_isBuildSettingFlodout)
            {
                currentBuildSetting = (AssetBundleBuildSetting)EditorGUILayout.ObjectField(currentBuildSetting, typeof(AssetBundleBuildSetting), false, GUILayout.Width(250));

                GUILayout.BeginVertical();

                GUIStyle style = new GUIStyle(EditorStyles.toolbarButton);
                // Application Build Information
                DrawLabelTitle(style, "### 默认构建信息 ###", GUILayout.Width(250));

                DrawTitleContentInfo("CompanyName", currentBuildSetting.applicationBuildInfo.CompanyName, style);
                DrawTitleContentInfo("ProductName", currentBuildSetting.applicationBuildInfo.ProductName, style);
                DrawTitleContentInfo("Version", currentBuildSetting.applicationBuildInfo.Version, style);
                DrawTitleContentInfo("DeviceLevel", currentBuildSetting.applicationBuildInfo.DeviceLv.ToString(), style, true);
#if UNITY_ANDROID
                DrawTitleContentInfo("BundleVCode", currentBuildSetting.applicationBuildInfo.BundleVersionCode, style);
                DrawTitleContentInfo("PackageName", currentBuildSetting.applicationBuildInfo.PackageName, style);
#endif

#if UNITY_IOS
                DrawTitleContentInfo("Build", currentBuildSetting.applicationBuildInfo.Build, style);
                DrawTitleContentInfo("BundleIdentifier", currentBuildSetting.applicationBuildInfo.BundleIdentifier, style);
#endif

                // AssetBundke Build Information
                GUILayout.Space(8);
                DrawLabelTitle(style, "### 默认AssetBundle构建信息 ###", GUILayout.Width(250));

                string caption;
                for (int i = 0; i < currentBuildSetting.AssetBundleBuildInfos.Length; i++)
                {
                    var info = currentBuildSetting.AssetBundleBuildInfos[i];

                    caption = (info.IsFloadout) ? string.Format("●  {0}", info.TipsName) : string.Format("○ {0}", info.TipsName);
                    info.IsFloadout = GUILayout.Toggle(info.IsFloadout, caption, tipStyle, GUILayout.Width(260));

                    if (info.IsFloadout)
                    {
                        foreach (var item in currentBuildSetting.AssetBundleBuildInfos) { if (!item.JenkinsBuildId.Equals(info.JenkinsBuildId)) { item.IsFloadout = false; } }
                        DrawAssetBundleBuildInfo("JenkinsBuildId", info.JenkinsBuildId.ToString(), style);
                        DrawAssetBundleBuildInfo("SummaryInfo", info.TipsName, style);
                        DrawAssetBundleBuildInfo("ChanelName", info.ChanelName, style);
                        DrawAssetBundleBuildInfo("Language", info.buildLanguage.ToString(), style);
                        DrawAssetBundleBuildInfo("AssetZipWay", info.assetZipTool.ToString(), style);
                        DrawAssetBundleBuildInfo("BuildTarget", info.BuildTarget, style);
                    }
                    GUILayout.Space(2);
                }

                GUILayout.EndVertical();
            }
        }

        private void DrawTitleContentInfo(string title, string content, GUIStyle style, bool showDevice = false)
        {
            GUILayout.BeginHorizontal();
            style.fontSize = 13;
            style.alignment = TextAnchor.MiddleLeft;
            style.normal.textColor = Color.green;

            GUILayout.Label(title, style, GUILayout.Width(105));
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;
            if (!showDevice)
            {
                GUILayout.Label(content, style);
            }
            else
            {
                var c = GUI.color;
                GUI.color = Color.cyan;
                currentBuildSetting.applicationBuildInfo.DeviceLv = (DeviceLevel)EditorGUILayout.EnumPopup(currentBuildSetting.applicationBuildInfo.DeviceLv, style);
                GUI.color = c;
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(5);
        }

        private void DrawAssetBundleBuildInfo(string title, string content, GUIStyle style)
        {
            GUILayout.BeginHorizontal();

            style.fontSize = 13;
            style.alignment = TextAnchor.MiddleLeft;
            style.normal.textColor = Color.green;
            GUILayout.Label(title, style, GUILayout.Width(105));

            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label(content, style);

            GUILayout.EndHorizontal();
            GUILayout.Space(5);
        }

        private void DrawBuildSubPkgSettings()
        {
            if (_isBuildSubPkgFlodout)
            {
                GUILayout.BeginVertical();
                //currentSplitSetting = (AssetBundleSplitSetting)EditorGUILayout.ObjectField(currentSplitSetting, typeof(AssetBundleSplitSetting), false, GUILayout.Width(250));
                //todo 支持自定义分包，并在打包管线中。自动打包。
                GUIStyle style = new GUIStyle(EditorStyles.toolbarButton);
                DrawLabelTitle(style, "### Asset Bundle 压缩设置 ###", GUILayout.Width(250));

                style = new GUIStyle(EditorStyles.helpBox);
                style.fontSize = 14;
                style.fontStyle = FontStyle.Normal;
                style.alignment = TextAnchor.UpperLeft;
                style.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;

                //### 固定每个Zip大小 ###
                string tipsContent = string.Empty;
                tipsContent += "## 固定每个Zip大小 ##" + "\n";
                tipsContent += "例如需要热更资源总大小为500 Mb" + "\n";
                tipsContent += "固定每50Mb压缩成一个Zip包" + "\n";
                tipsContent += "最终将输出5个zip文件，每个Zip文件压缩50Mb的资源" + "\n";
                GUILayout.Label(new GUIContent(tipsContent, EditorGUIUtility.FindTexture("d_console.infoicon.sml")), style, GUILayout.Height(78));

                style = new GUIStyle(EditorStyles.toolbarButton);
                updateTips = GetTotalHotUpdateResSize();
                DrawTitleContentInfo("热更资源总大小", updateTips, style);

                var allInfo = AssetBundlesMaker.LoadAssetBundleSetting();
                var zipMb = allInfo.applicationBuildInfo.subZipSize;

                DrawTitleContentInfo("单个Zip大小", $"{zipMb}Mb", style);

                long totalMb = totalSize / 1048576L;

                long tmpCount = totalMb % zipMb == 0 ? (totalMb / zipMb) : (totalMb / zipMb) + 1;
                long zipCount = totalMb < zipMb ? 1 : tmpCount;
                DrawTitleContentInfo("Summary", $"共{updateTips}资源 => 自动划分为 {zipCount} 个zip", style);

                GUILayout.EndVertical();
            }
        }

        private string GetTotalHotUpdateResSize()
        {
            if (!close && updateFile != null) { return updateTips; }

            var _streamingDisk = App.IO.Disk(App.Env.DataPath + App.Env.ReleasePath);
            var _souceAssetDir = _streamingDisk.Directory(App.Env.PlatformToName(App.Env.SwitchPlatform));
            IFile tarFile = _souceAssetDir.File(UpdateFileStore.FILE_NAME);
            if (!tarFile.Exists)
            {
                return "构建AssetBundle后预览详情";
            }

            updateFile = new UpdateFile(Encoding.Default.GetString(tarFile.Read()));
            UpdateFileField[] updateFields = updateFile.Fields;

            totalSize = 0;
            foreach (var item in updateFields)
            {
                totalSize += item.Size;
            }

            return GetBytesString(totalSize);
        }

        string GetBytesString(long bytes)
        {
            if (bytes >= 1073741824L)
            {
                return (bytes / 1073741824L) + "GB";
            }
            else if (bytes >= 1048576L)
            {
                return (bytes / 1048576L) + "MB";
            }
            else if (bytes >= 1024L)
            {
                return (bytes / 1024L) + "KB";
            }
            else
            {
                return bytes + "B";
            }
        }

        private void DrawBuildSplitPkgSettings()
        {
            if (_isBuildSplitPkgFlodout)
            {
                GUILayout.BeginVertical();
                currentSplitSetting = (AssetBundleSplitSetting)EditorGUILayout.ObjectField(currentSplitSetting, typeof(AssetBundleSplitSetting), false, GUILayout.Width(250));
                GUIStyle style = new GUIStyle(EditorStyles.toolbarButton);
                DrawLabelTitle(style, "### 首包 拆分 设置 ###", GUILayout.Width(250));

                // 基于平台的分包信息
                string caption;
                for (int i = 0; i < currentSplitSetting.SplitInfos.Length; i++)
                {
                    var info = currentSplitSetting.SplitInfos[i];

                    caption = (info.IsFloadout) ? string.Format("●  {0}", info.BuildTarget) : string.Format("○ {0}", info.BuildTarget);
                    info.IsFloadout = GUILayout.Toggle(info.IsFloadout, caption, tipStyle, GUILayout.Width(260));

                    bool needRepaint = false;
                    if (info.IsFloadout)
                    {
                        foreach (var item in currentSplitSetting.SplitInfos) { if (!item.BuildTarget.Equals(info.BuildTarget)) { item.IsFloadout = false; } }

                        // Draw Split File Infor.
                        DrawSplitBundleToggleTips("### 跟包文件 ###", info, Color.green, true);
                        if (info.IsFloadoutFiles)
                        {
                            for (int j = 0; j < info.AccompanyFiles.Length; j++)
                            {
                                needRepaint = DrawSplitFileOrDirectoryRect(j == info.AccompanyFiles.Length - 1, true, i, j);
                                if (needRepaint)
                                {
                                    break;
                                }
                            }
                        }

                        if (needRepaint)
                        {
                            EditorUtility.SetDirty(currentSplitSetting);
                            GUI.changed = true;
                            break;
                        }

                        // Draw Split Dorectory Infor.
                        DrawSplitBundleToggleTips("### 跟包文件夹 ###", info, Color.cyan, false);
                        if (info.IsFloadoutDir)
                        {
                            for (int k = 0; k < info.AccompanyDirectorys.Length; k++)
                            {
                                needRepaint = DrawSplitFileOrDirectoryRect(k == info.AccompanyDirectorys.Length - 1, false, i, k);
                                if (needRepaint)
                                {
                                    break;
                                }
                            }
                        }

                        if (needRepaint)
                        {
                            EditorUtility.SetDirty(currentSplitSetting);
                            GUI.changed = true;
                            break;
                        }

                    }

                    GUILayout.Space(5);
                }

                GUILayout.Space(8);

                GUIStyle btnStyle = new GUIStyle(EditorStyles.toolbarButton);
                btnStyle.fontSize = 14;
                btnStyle.hover.textColor = Color.cyan;
                btnStyle.normal.textColor = Color.white;
                btnStyle.alignment = TextAnchor.MiddleCenter;

                GUILayout.EndVertical();
            }
        }

        private void DrawAssetEncryptToggle(int idx = 0)
        {
            GUILayout.BeginHorizontal();
            string caption;
            var buildInfo = currentBuildSetting.applicationBuildInfo;
            var c = GUI.color;
            switch (idx)
            {
                case 0:
                    caption = (buildInfo.EncryptCode) ? string.Format("●  {0}", "开启代码资源加密") : string.Format("○ {0}", "关闭代码资源加密");
                    GUI.color = buildInfo.EncryptCode ? Color.green : c;
                    buildInfo.EncryptCode = GUILayout.Toggle(buildInfo.EncryptCode, caption, tipStyle, GUILayout.Width(260));
                    if (buildInfo.EncryptCode)
                    {
                        GUILayout.Space(3);
                        buildInfo.EncryptCodeAlg = (EnCryptAlgorithm)EditorGUILayout.EnumPopup(buildInfo.EncryptCodeAlg);
                    }
                    break;
                case 1:
                    caption = (buildInfo.EncryptAsset) ? string.Format("●  {0}", "开启游戏资产加密") : string.Format("○ {0}", "关闭游戏资产加密");
                    GUI.color = buildInfo.EncryptAsset ? Color.green : c;
                    buildInfo.EncryptAsset = GUILayout.Toggle(buildInfo.EncryptAsset, caption, tipStyle, GUILayout.Width(260));
                    if (buildInfo.EncryptAsset)
                    {
                        GUILayout.Space(3);
                        buildInfo.EncryptAssetAlg = (EnCryptAlgorithm)EditorGUILayout.EnumPopup(buildInfo.EncryptAssetAlg);
                    }
                    break;
                case 2:
                    caption = (buildInfo.EncryptCfgAsset) ? string.Format("●  {0}", "开启配置资源加密") : string.Format("○ {0}", "关闭配置资源加密");
                    GUI.color = buildInfo.EncryptCfgAsset ? Color.green : c;
                    buildInfo.EncryptCfgAsset = GUILayout.Toggle(buildInfo.EncryptCfgAsset, caption, tipStyle, GUILayout.Width(260));
                    if (buildInfo.EncryptCfgAsset)
                    {
                        GUILayout.Space(3);
                        buildInfo.EncryptCfgAlg = (EnCryptAlgorithm)EditorGUILayout.EnumPopup(buildInfo.EncryptCfgAlg);
                    }
                    break;
                default:
                    break;
            }
            GUI.color = c;
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            //string caption;
            //caption = (info.IsFloadout) ? string.Format("●  {0}", info.BuildTarget) : string.Format("○ {0}", info.BuildTarget);
            //info.IsFloadout = GUILayout.Toggle(info.IsFloadout, caption, tipStyle, GUILayout.Width(260));
        }

        private void DrawAssetEnCryptSettings()
        {
            if (_isEnCryptSettingFlodout)
            {
                GUILayout.BeginVertical();

                GUIStyle style = new GUIStyle(EditorStyles.toolbarButton);
                DrawLabelTitle(style, "### 当前加密信息 ###", GUILayout.Width(250));
                DrawAssetEncryptToggle(0);
                DrawAssetEncryptToggle(1);
                DrawAssetEncryptToggle(2);

                GUILayout.EndVertical();
            }
        }

        private void DrawSplitBundleToggleTips(string title, SplitBundleInfo info, Color c, bool drawFile)
        {

            GUIStyle style = new GUIStyle(EditorStyles.toolbarButton);
            style.fontSize = 13;
            style.alignment = TextAnchor.MiddleLeft;
            style.normal.textColor = c;
            style.hover.textColor = c;
            var pC = GUI.color;
            string caption = string.Empty;

            GUI.color = c;
            if (drawFile)
            {

                caption = info.IsFloadoutFiles ? string.Format("▼ {0}", title) : string.Format("► {0}", title);
                info.IsFloadoutFiles = GUILayout.Toggle(info.IsFloadoutFiles, caption, style, GUILayout.Width(260));
            }
            else
            {
                caption = info.IsFloadoutDir ? string.Format("▼ {0}", title) : string.Format("► {0}", title);
                info.IsFloadoutDir = GUILayout.Toggle(info.IsFloadoutDir, caption, style, GUILayout.Width(260));
            }
            GUI.color = pC;

            GUILayout.Space(5);
        }

        private bool DrawSplitFileOrDirectoryRect(bool isEnd, bool drawFile, int splitIdx, int tarIdx)
        {
            GUILayout.BeginHorizontal();


            GUIStyle txtStyle = new GUIStyle(EditorStyles.textField);
            txtStyle.fontSize = 12;
            txtStyle.fontStyle = FontStyle.Bold;
            txtStyle.normal.textColor = Color.white;
            txtStyle.alignment = TextAnchor.MiddleCenter;

            Rect splitBundleRect = EditorGUILayout.GetControlRect(GUILayout.Height(25));
            if (drawFile)
            {
                EditorGUI.LabelField(splitBundleRect, currentSplitSetting.SplitInfos[splitIdx].AccompanyFiles[tarIdx], txtStyle);
            }
            else
            {
                EditorGUI.LabelField(splitBundleRect, currentSplitSetting.SplitInfos[splitIdx].AccompanyDirectorys[tarIdx], txtStyle);
            }


            if (Event.current.type == EventType.DragUpdated && splitBundleRect.Contains(Event.current.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            }
            if (Event.current.type == EventType.DragExited && splitBundleRect.Contains(Event.current.mousePosition))
            {
                if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
                {
                    string resultPath = DragAndDrop.paths[0];

                    if (drawFile && File.Exists(resultPath))
                    {
                        if (!currentSplitSetting.Contains(currentSplitSetting.SplitInfos[splitIdx].AccompanyFiles, resultPath))
                        {
                            currentSplitSetting.SplitInfos[splitIdx].AccompanyFiles[tarIdx] = resultPath;
                        }
                    }

                    if (!drawFile && Directory.Exists(resultPath))
                    {
                        if (!currentSplitSetting.Contains(currentSplitSetting.SplitInfos[splitIdx].AccompanyDirectorys, resultPath))
                        {
                            currentSplitSetting.SplitInfos[splitIdx].AccompanyDirectorys[tarIdx] = resultPath;
                        }
                    }
                }
            }

            GUILayout.Space(5);

            //draw warning file.
            if (drawFile)
            {
                string containedDir = string.Empty;
                bool showWarning = currentSplitSetting.JudgeAccompanyFileContainsToDirectory(splitIdx, tarIdx, out containedDir);

                if (showWarning)
                {
                    if (GUILayout.Button(_warningTextureCnt, GUILayout.Width(25), GUILayout.Height(25)))
                    {
                        TipsAlertWindow.ShowAlertWithBtn("警告: 请删除此跟包文件",
                             containedDir
                            + "\n" + "点击确定，自动删除",
                            () =>
                            {
                                currentSplitSetting.RemoveFileInfo(splitIdx, tarIdx);
                            });
                    }
                }
            }

            if (isEnd)
            {
                // draw add
                if (GUILayout.Button(_addTextureCnt, GUILayout.Width(25), GUILayout.Height(25)))
                {
                    if (drawFile)
                    {
                        currentSplitSetting.AddFileInfo(splitIdx);
                    }
                    else
                    {
                        currentSplitSetting.AddDirectoryInfo(splitIdx);
                    }

                    return true;
                }
            }
            else
            {
                // draw delete
                if (GUILayout.Button(_deleteTextureCnt, GUILayout.Width(25), GUILayout.Height(25)))
                {
                    if (drawFile)
                    {
                        currentSplitSetting.RemoveFileInfo(splitIdx, tarIdx);
                    }
                    else
                    {
                        currentSplitSetting.RemoveDirectoryInfo(splitIdx, tarIdx);
                    }

                    return true;
                }
            }

            GUILayout.EndHorizontal();

            return false;
        }


        /* ---------------------------*/
        /* -----------BUILD-----------*/
        /* ---------------------------*/
        private void BuildAltas()
        {
            Debug.LogFormat($"<color=#FFFF00> 开始构建图集 ... </color>");
            AssetBundlesMaker.BuildUIAltas();
        }

        private void BuildHCLRAot()
        {
            Debug.LogFormat($"<color=#FFFF00> 开始构建HCLR AOT ... </color>");
            AssetBundlesMaker.BuildAot();
        }

        private void BuildFirstPkg()
        {
            Debug.LogFormat($"<color=#FFFF00> 开始构建首包 ... </color>");
            AssetBundlesMaker.BuildFirstPackage();
        }

        private void BuildHotFixed()
        {
            Debug.LogFormat($"<color=#FFFF00> 开始构建热更新 </color>");
            AssetBundlesMaker.BuildHotFixPackapge();
        }
    }

}