
using System;
using FrameWork;
using UnityEngine;
using GameEngine;
using System.Collections;
using Core.Interface.Event;
using UnityEngine.SceneManagement;
using strange.extensions.mediation.impl;
using strange.extensions.dispatcher.eventdispatcher.impl;

namespace UI
{
    public class LoadingMediator : EventMediator
    {
        [Inject]
        public LoadingView view { get; set; }

        IEventHandler eHandlerShowToast = null;
        IEventHandler eHandlerLoadGameScene = null;

        public override void OnRegister()
        {

            eHandlerShowToast = App.Instance.On(GameEvent.SHOW_TOAST, OnShowToast);
            eHandlerLoadGameScene = App.Instance.On(LoadEvent.LOAD_GAME_SCENE, OnLoadGameScene);
        }

        public override void OnRemove()
        {

            if (eHandlerLoadGameScene != null) { eHandlerLoadGameScene.Cancel(); }
            eHandlerLoadGameScene = null;

            if (eHandlerShowToast != null) { eHandlerShowToast.Cancel(); }
            eHandlerShowToast = null;
        }

        private void OnShowToast(object sender, EventArgs e)
        {
            string data = sender as string;
            view.ShowToast(data);
        }


        #region Game 场景 加载

        void OnLoadGameScene(object sender, EventArgs e)
        {
            view.ShowLoading(true);
            view.ShowInit(true);
            StartCoroutine(AsyncLoadGameScne());
        }

        IEnumerator AsyncLoadGameScne()
        {
            view.ShowInit(true);

            AsyncOperation sc = SceneManager.LoadSceneAsync("Scenes/Game", new LoadSceneParameters(LoadSceneMode.Single));
            while (!sc.isDone)
            {
                // xxx.text = sc.progress.ToString();
                // todo: show progress on ui.
                view.SetLoadingProg(sc.progress * 0.5f);
                yield return Yielders.EndOfFrame;
            }

            yield return SceneLoadMgr.Ins.StartLoadGameMapAssets(() =>
            {
                //MapMgr.Instance.BindCameraAndCharacter();
                CameraMgr.Instance.BindVirtualCamera();

                UIConfig mainView = new UIConfig();
                mainView.floaderName = "main";
                mainView.prefabName = "main";
                mainView.hideAllBefore = false;
                mainView.fullScreen = false;
                UIMgr.Ins.OpenUI(mainView, (s) =>
                {
                    view.SetLoadingProg(1);
                    view.ShowInit(false);
                    view.ShowLoading(false);

                    //   App.AssetBundleLoader.UnloadAssetBundle("LogIn");
                });
            }, (f) =>
            {
                view.SetLoadingProg(0.5f + f * .5f);

            });
        }

        #endregion

        void SetLoadingTips(object evt)
        {
            string tips = (evt as TmEvent).data.ToString();
            view.SetLoadingTips(tips);
        }
    }
}