
using Model;
using FrameWork;
using Cinemachine;
using UnityEngine;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2023/07/12
	Filename: 	CameraMgr.cs
	Author:		DaiLi.Ou
	Descriptions: 
*********************************************************************/
namespace GameEngine
{
    public class CameraMgr : MonoBehaviour
    {
        public static CameraMgr Instance = null;

        [SerializeField] Camera baseCamera;
        [SerializeField] Animator postEffectAni = null;
        [SerializeField] CinemachineVirtualCamera userVirtualCamera = null;

        // Public Variables
        public Camera BaseCamera
        {
            get => baseCamera;
        }

        private void Awake()
        {
            Instance = this;
        }

        public void OnDestroy()
        {
            Instance = null;

            baseCamera = null;
        }

        public void ShowGlitchEffect()
        {
            postEffectAni.enabled = true;
            postEffectAni.Play("GlitchAnim", 0, 0);

            GlobalData.instance.fightModelMgr.SetDemoFightData();
            StartCoroutine(SceneLoadMgr.Ins.StartLoadFightMapAssets());
        }

        public void BindVirtualCamera()
        {
            userVirtualCamera.Follow = GlobalData.instance.characterModelMgr.GetMapPlayerCharacter().Character.TransformSelf;
            var cinemachineConfiner = userVirtualCamera.GetComponent<CinemachineConfiner>();
            cinemachineConfiner.m_BoundingShape2D = GlobalData.instance.sceneModelMgr.MapInstance.GetComponentInChildren<PolygonCollider2D>();
        }

        public void OnGlitchEffectStart()
        {
            FightControl.Ins.State = FightState.Initialize;
            glitchEnd = false;
        }

        public void OnGlitchEffectEnd()
        {
            glitchEnd = true;
            if (!bindedFight)
            {
                StartFightBindCamera();
            }
            else
            {
                postEffectAni.Play("GlitchAnim", 0, 0);
            }
        }

        bool glitchEnd = false;
        bool bindedFight = false;
        public void StartFightBindCamera()
        {
            bindedFight = false;
            if (glitchEnd)
            {
                var globalData = GlobalData.instance;
                var fightModelMgr = globalData.fightModelMgr;

                globalData.sceneModelMgr.MapMgrObj.SetActive(false);

                var mapIns = fightModelMgr.GetCurFightMap;
                userVirtualCamera.Follow = null;
                var cinemachineConfiner = userVirtualCamera.GetComponent<CinemachineConfiner>();
                cinemachineConfiner.m_BoundingShape2D = null;

                userVirtualCamera.Follow = mapIns;
                cinemachineConfiner.m_BoundingShape2D = mapIns.GetComponent<PolygonCollider2D>();

                bindedFight = true;
                CDebug.LogProgress("## [5] FightBindCamera End ##");

                FightControl.Ins.State = FightState.Start;
            }
        }

        public void EndFightBindCamera()
        {

        }
    }
}