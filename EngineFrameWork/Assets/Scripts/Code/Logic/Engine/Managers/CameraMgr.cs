
using Cinemachine;
using Model;
using UI;
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
            userVirtualCamera.Follow = GlobalData.instance.characterModelMgr.GetPlayerCharacter().TransformSelf;
            var cinemachineConfiner = userVirtualCamera.GetComponent<CinemachineConfiner>();
            cinemachineConfiner.m_BoundingShape2D = GlobalData.instance.sceneModelMgr.MapInstance.GetComponentInChildren<PolygonCollider2D>();
        }

        public void OnGlitchEffectStart()
        {
            CDebug.Log("OnGlitchEffectStart");
            glitchEnd = false;
        }

        public void OnGlitchEffectEnd()
        {
            CDebug.Log("OnGlitchEffectEnd");
            glitchEnd = true;
            if (!bindedFight)
            {
                StartFigtBindCamera();
            }
        }

        bool glitchEnd = false;
        bool bindedFight = false;
        public void StartFigtBindCamera()
        {
            bindedFight = false;
            if (glitchEnd)
            {
                CDebug.Log("StartFigtBindCamera");
                GlobalData.instance.sceneModelMgr.MapMgrObj.SetActive(false);

                var mapIns = GlobalData.instance.fightModelMgr.GetCurFightMap().gameObject;
                userVirtualCamera.Follow = null;
                var cinemachineConfiner = userVirtualCamera.GetComponent<CinemachineConfiner>();
                cinemachineConfiner.m_BoundingShape2D = null;

                userVirtualCamera.Follow = mapIns.transform;
                cinemachineConfiner.m_BoundingShape2D = mapIns.GetComponent<PolygonCollider2D>();

                UIMgr.Ins.CloseToClear();
                bindedFight = true;
            }
        }

        public void EndFightBindCamera()
        {

        }
    }
}