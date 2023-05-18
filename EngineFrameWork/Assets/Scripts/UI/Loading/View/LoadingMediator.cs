using System;
using FrameWork;
using UnityEngine;
using Core.Interface;
using Core.Resources;
using UnityEngine.SceneManagement;
using strange.extensions.mediation.impl;
using strange.extensions.dispatcher.eventdispatcher.impl;

namespace UI
{
    public class LoadingMediator : EventMediator
    {
        [Inject]
        public LoadingView view { get; set; }

        private string lastSceneBundle = string.Empty;
        private string lastScene = "" + "/" + Config.Scene_Launch;

        public override void OnRegister()
        {
            dispatcher.AddListener(LoadEvent.SET_LOADING_PROGRESS, OnUpdateProg);
            dispatcher.AddListener(LoadEvent.SET_LOADING_TIP, SetLoadingTips);
            dispatcher.AddListener(LoadEvent.LOAD_SCENE, OnStartLoadScene);
            dispatcher.AddListener(LoadEvent.SHOW_ACTIVITY_INDICATOR, OnStartDataLoad);
            dispatcher.AddListener(LoadEvent.HIDE_ACTIVITY_INDICATOR, OnHidenDataLoad);
            dispatcher.AddListener(LoadEvent.SHOW_ACTIVITY_INDICATOR_IMMEDIATELY, OnShowLoad);
        }

        public override void OnRemove()
        {
            dispatcher.RemoveListener(LoadEvent.SET_LOADING_PROGRESS, OnUpdateProg);
            dispatcher.RemoveListener(LoadEvent.LOAD_SCENE, OnStartLoadScene);
            dispatcher.RemoveListener(LoadEvent.SHOW_ACTIVITY_INDICATOR, OnStartDataLoad);
            dispatcher.RemoveListener(LoadEvent.HIDE_ACTIVITY_INDICATOR, OnHidenDataLoad);
            dispatcher.RemoveListener(LoadEvent.SET_LOADING_TIP, SetLoadingTips);
            dispatcher.RemoveListener(LoadEvent.SHOW_ACTIVITY_INDICATOR_IMMEDIATELY, OnShowLoad);
        }


        void OnUpdateProg(object v)
        {
            view.SetLoadingProg((float)(v as TmEvent).data);
        }

        void OnStartLoadScene(object e)
        {
            LoadingStructure loadscene = (e as TmEvent).data as LoadingStructure;
            StartLoadScene(loadscene);
        }

        void StartLoadScene(LoadingStructure structure)
        {
            string scene = string.Format("{0}/{1}", structure.bundleName, structure.sceneName);

            lastScene = scene;

            float curProg = view.GetLoadingProg();
            if (curProg <= 0 || curProg >= 1) view.SetLoadingProg(0);
            AsynctLoadScene(structure).Coroutine();
        }

        async ETTask AsynctLoadScene(LoadingStructure loadscene)
        {

            App.Instance.Trigger(GameEvent.APP_EVENT_SCENE_CHANGE_START);

            string bundlePath = string.Empty;
#if UNITY_EDITOR
            if (App.Env.DebugLevel == DebugLevels.Product || App.Env.DebugLevel == DebugLevels.Staging)
            {
                if (!string.IsNullOrEmpty(loadscene.bundleName))
                {
                    bundlePath = string.Format("scenes/{0}", loadscene.bundleName);
                    await App.AssetBundleLoader.LoadAssetBundleAsync(bundlePath, (assetBundle) => { });
                }
            }
#else
            if (!string.IsNullOrEmpty(loadscene.bundleName))
            {
                bundlePath = string.Format("scenes/{0}", loadscene.bundleName);
                await App.AssetBundleLoader.LoadAssetBundleAsync(bundlePath, (assetBundle) => { });
            }
#endif

            App.Instance.Trigger(GameEvent.APP_EVENT_SCENE_CHANGE);

            float progOffset = view.GetLoadingProg();

            if (loadscene.isAsync)
            {
                AsyncOperation async = SceneManager.LoadSceneAsync(loadscene.sceneName);
                async.allowSceneActivation = loadscene.sceneActivation;

                while (!async.isDone)
                {
                    view.SetLoadingProg(progOffset + async.progress * loadscene.progressRate);
                    await TimeAwaitHelper.AwaitTime(ResourcesHosted.PER_FRAME_TIME);
                }

                view.SetLoadingProg(progOffset + loadscene.progressRate);
            }
            else
            {
                SceneManager.LoadScene(loadscene.sceneName);
                view.SetLoadingProg(progOffset + loadscene.progressRate);
            }

            if (!string.IsNullOrEmpty(lastSceneBundle) && !bundlePath.Equals(lastSceneBundle))
            {
                App.AssetBundleLoader.UnloadAssetBundle(lastSceneBundle);
            }

            lastSceneBundle = bundlePath;
            App.Instance.Trigger(GameEvent.APP_EVENT_SCENE_CHANGE_END);
        }

        void OnStartDataLoad()
        {
            view.ShowDataLoadingView();
        }

        void OnStartDataLoad(object sender, EventArgs e)
        {
            view.ShowDataLoadingView();
        }

        void OnHidenDataLoad(object sender, EventArgs e)
        {
            view.HidenDataLoadingView();
        }

        void OnHidenDataLoad()
        {
            view.HidenDataLoadingView();
        }

        void OnShowLoad()
        {
            view.ShowLoading();
        }

        void SetLoadingTips(object evt)
        {
            string tips = (evt as TmEvent).data.ToString();
            view.SetLoadingTips(tips);
        }
    }
}