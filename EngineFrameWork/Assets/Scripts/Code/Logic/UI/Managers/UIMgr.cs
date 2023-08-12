using Model;
using System;
using FrameWork;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace UI
{
    public class UIMgr : MonoSingleton<UIMgr>
    {
        #region MVC Component

        UICrossRoot uICrossRoot;

        #endregion

        #region UI Canvas
        GameObject self;
        Canvas uiCanvas;
        CanvasScaler scaler;

        private int SortOrder = 3;
        private const string CanvasName = "UICanvas";
        private Vector2 ReferenceResolution = new Vector2(720, 1280);
        private RenderMode CanvasRenderMode = RenderMode.ScreenSpaceOverlay;
        private CanvasScaler.ScaleMode UIScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;


        private GlobalData _globalData;
        private GlobalData globalData
        {
            get => _globalData ?? (_globalData = GlobalData.instance);
        }

        #endregion

        #region Open & Close UI

        Dictionary<string, GameObject> cachesUI = new Dictionary<string, GameObject>();
        Stack<GameObject> openStack = new Stack<GameObject>();
        Stack<UIConfig> openConfig = new Stack<UIConfig>();
        Stack<UIConfig> tmpConfig = new Stack<UIConfig>();
        Stack<GameObject> tmpStack = new Stack<GameObject>();

        bool opening = false;
        UIConfig curConfig;
        Transform uiContainer;

        public event Action<GameObject> onEventShowUI = null;
        public event Func<GameObject, bool> onEventCloseUI = null;
        public event Action onEventRoot = null;
        public event Action onEventClear = null;


        #endregion

        #region Unity Calls

        private void Start()
        {
            uiContainer = transform;
            uICrossRoot = self.AddComponent<UICrossRoot>();
        }

        public bool Inited()
        {
            if (uICrossRoot == null) { return false; }

            return uICrossRoot.Inited();
        }

        private void OnDestroy()
        {
            foreach (var item in cachesUI)
            {
                Destroy(item.Value);
            }

            foreach (var item in openStack)
            {
                if (item != null)
                    Destroy(item);
            }
            cachesUI.Clear();
            openStack.Clear();
            openConfig.Clear();

            onEventShowUI = null;
            onEventCloseUI = null;
            onEventRoot = null;
            onEventClear = null;
        }

        protected override void Init()
        {
            self = gameObject;

            //Canvas setting.
            uiCanvas = self.AddComponent<Canvas>();
            uiCanvas.sortingOrder = SortOrder;
            uiCanvas.renderMode = CanvasRenderMode;
            //Canvas Scaler.
            scaler = self.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = UIScaleMode;
            scaler.referenceResolution = ReferenceResolution;
            float screenRatio = 1f * Screen.width / Screen.height;
            if (screenRatio >= 9f / 16f)
            {
                scaler.matchWidthOrHeight = 1f;
            }
            else
            {
                scaler.matchWidthOrHeight = 0f;
            }

            //Screen setting.
            Screen.orientation = ScreenOrientation.Portrait;
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;

            self.AddComponent<GraphicRaycaster>();
            self.name = CanvasName;
            base.Init();
        }

        #endregion

        public void OpenUI(UIConfig config, Action<GameObject> callback = null)
        {
            if (opening) return;

            GameObject ui = null;
            if (cachesUI.TryGetValue(config.prefabName, out ui))
            {
                if (ui != null)
                {
                    ShowUI(ui, config);
                    if (callback != null) callback(ui);
                    return;
                }
            }

            if (config.transform == null)
            {
                opening = true;
                string path = string.Format("ui/prefabs/{0}/{1}", config.floaderName, config.prefabName);

                //CrossDisptacher.Dispatch(LoadEvent.SHOW_ACTIVITY_INDICATOR);
                App.Res.LoadAsync<GameObject>(path, (obj) =>
                {
                    if (cachesUI.ContainsKey(config.prefabName)) return;


                    GameObject prefabObj = obj.Get<GameObject>(this);
                    if (prefabObj == null)
                    {
                        return;
                    }

                    GameObject uiO = Instantiate(prefabObj, uiContainer);
                    uiO.name = config.prefabName;
                    cachesUI.Add(config.prefabName, uiO);

                    ShowUI(uiO, config);
                    if (callback != null) callback(uiO);
                    opening = false;
                });
            }
            else
            {
                config.transform.SetParent(uiContainer);

                GameObject uiO = config.transform.gameObject;
                cachesUI.Add(config.prefabName, uiO);
                ShowUI(uiO, config);
                if (callback != null) callback(uiO);
            }
        }

        /// <summary>
        /// 关闭界面，并显示缓存的第一个打开的界面
        /// </summary>
        public void CloseToRoot()
        {
            if (onEventCloseUI != null && !onEventCloseUI(openStack.Peek())) return;

            while (openStack.Count > 1)
            {
                openConfig.Pop();
                GameObject ui = openStack.Pop();
                ui.SetActive(false);
            }
            UpdateBaseUiElements();
            GameObject peek = openStack.Peek();
            if (peek != null && !peek.activeSelf) peek.SetActive(true);
            if (onEventRoot != null) onEventRoot();
        }

        /// <summary>
        /// 关闭所有界面
        /// </summary>
        public void CloseToClear()
        {
            if (onEventCloseUI != null && !onEventCloseUI(openStack.Peek())) return;

            while (openStack.Count > 0)
            {
                openConfig.Pop();
                GameObject ui = openStack.Pop();
                ui.SetActive(false);
            }
            UpdateBaseUiElements();
            if (onEventClear != null) onEventClear();

        }

        /// <summary>
        /// 关闭当前界面，显示缓存的上一个打开的界面
        /// </summary>
        public void CloseUI()
        {
            if (onEventCloseUI != null && !onEventCloseUI(openStack.Peek())) return;

            openConfig.Pop();
            GameObject ui = openStack.Pop();
            ui.SetActive(false);
            curConfig = openConfig.Count > 0 ? openConfig.Peek() : null;

            UpdateBaseUiElements();
        }

        /// <summary>
        /// 关闭指定ui
        /// </summary>
        /// <param name="target">需要关闭的界面 config</param>
        public void CloseUI(UIConfig target)
        {
            tmpConfig.Clear();
            tmpStack.Clear();
            foreach (var item in openConfig)
            {
                if (!item.prefabName.Equals(target.prefabName))
                {
                    tmpConfig.Push(item);
                }
            }

            foreach (var item in openStack)
            {
                if (!item.name.Equals(target.prefabName))
                {
                    tmpStack.Push(item);
                }
                else
                {
                    item.SetActive(false);
                }
            }

            if (!cachesUI.ContainsKey(target.prefabName))
            {
                CDebug.LogError(string.Format("未打开过ui：{0}", target.prefabName));
                return;
            }
            else
            {
                cachesUI[target.prefabName].SetActive(false);
            }

            openConfig.Clear();
            openConfig = tmpConfig;
            openStack.Clear();
            openStack = tmpStack;
        }

        /// <summary>
        /// 判断某个UI是否打开，若打开过返回实例，未打开过返回空
        /// </summary>
        /// <param name="cfg">需要判断的界面</param>
        /// <param name="trans">界面实例</param>
        /// <returns></returns>
        public bool IsOpenUI(UIConfig cfg, ref Transform trans)
        {
            if (curConfig == null)
            {
                trans = null;       // no cached ui from stack.
                return false;
            }

            bool opened = curConfig.prefabName.Equals(cfg.prefabName);
            trans = curConfig.transform;

            return opened;
        }

        void ShowUI(GameObject ui, UIConfig config)
        {
            if (onEventShowUI != null) onEventShowUI(ui);

            Transform trans = ui.transform;
            if (openConfig.Count > 0 && (openConfig.Peek().prefabName.Equals(config.prefabName) || openConfig.Peek().transform == trans)) return;

            ui.SetActive(true);
            openStack.Push(ui);
            trans.SetAsLastSibling();
            config.transform = trans;
            curConfig = config;
            openConfig.Push(config);

            UpdateBaseUiElements();
        }

        void UpdateBaseUiElements()
        {
            Transform curTrans = null;
            GameObject curGO = null;
            UIConfig lastCfg = null;
            bool hideAllBefore = false;

            foreach (UIConfig config in openConfig)
            {
                curTrans = config.transform;
                curGO = curTrans.gameObject;
                if (lastCfg == null)
                {
                    lastCfg = config;
                    curGO.SetActive(true);
                }
                else if (!lastCfg.fullScreen)
                {
                    if (!curGO.activeSelf) curGO.SetActive(true);
                    lastCfg = config;
                    continue;
                }
                else
                {
                    lastCfg = config;
                }

                if (hideAllBefore)
                {
                    if (curGO.activeSelf) curGO.SetActive(false);
                    continue;
                }

                if (config.hideAllBefore)
                {
                    hideAllBefore = true;
                    if (!curGO.activeSelf) curGO.SetActive(true);
                }
            }
        }

        void OnDispose()
        {

        }
    }
}
