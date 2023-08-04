using Model;
using System;
using DG.Tweening;
using UnityEngine;
using System.Threading;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2023/07/12
	Filename: 	MoveCtr.cs
	Author:		ShenMa
	Descriptions: 非战斗内的寻路模块
*********************************************************************/
namespace GameEngine
{
    [System.Serializable]
    public class MoveCtr : ISerializationCallbackReceiver
    {
        private float _fSpeed;

        private Transform _insMap;
        private SceneMap _sceneMap;
        private Character _character;

        private List<Vector2> _vecfPath;
        private List<MaskInfo> _vecMaskInfo;
        private List<Vector2Int> _vecPath;

        private Timer _turnDirTimer;

        private int _nCurDir;

        private bool _bIsMoving;

        private GameObject map_root;

        #region Unity Calls

        public void Initialized(Character _character)
        {
            this._character = _character;
            _sceneMap = GlobalData.instance.sceneModelMgr.SceneMap;

            _insMap = GlobalData.instance.sceneModelMgr.MapInstance;

            if (_character.Property.UserCharacter.isNpc == false)
            {
                map_root = _insMap.Find("map_root").gameObject;
                UIEventTrigger.Get(map_root).onClick += OnPanelClick;
            }

            _character.TransformSelf.position = new Vector2(1500, 1500);


            _vecfPath = new List<Vector2>();

            _vecMaskInfo = _sceneMap.m_vecMaskInfo;

            _nCurDir = 0;

            _fSpeed = 300.0f;

            _bIsMoving = false;
        }


        //public void OnDestroy()
        //{
        //    if ((_character.Property.UserCharacter.isNpc == false) && map_root != null)
        //        UIEventTrigger.Get(map_root).onClick -= OnPanelClick;
        //}

        public void OnAfterDeserialize()
        {
        }

        public void OnBeforeSerialize()
        {
        }

        #endregion

        public Vector2 GetMapPos(float x, float y, Vector2 size)
        {
            Vector2 vPos = new Vector2();
            vPos.x = _sceneMap.m_nWidth * (x + size.x / 2) / size.x;
            vPos.y = _sceneMap.m_nHeight * (y + size.y / 2) / size.y;
            return vPos;
        }

        private void OnPanelClick(PointerEventData eventData)
        {
            _character.TransformSelf.DOKill();

            Vector3 mousePos = eventData.pointerCurrentRaycast.worldPosition;

            Vector3 vTagPos = _insMap.InverseTransformPoint(mousePos);

            Vector2 vHeroPos = _character.TransformSelf.position;

            Vector2Int vSrcGrid = _sceneMap.Pixel2Grid(vHeroPos.x, vHeroPos.y);
            Vector2Int vDstGrid = _sceneMap.Pixel2Grid(vTagPos.x, vTagPos.y);
            _vecPath = _sceneMap.FindPath(vSrcGrid, vDstGrid, 0, 5000);

            _vecfPath.Clear();
            foreach (var vData in _vecPath)
            {
                Vector2 vec = _sceneMap.Grid2Pixel(vData.x, vData.y);
                _vecfPath.Add(vec);
            }

            //0是当前位置 从第二步开始
            Move(1);
        }

        public void GenPath(Vector3 targetPos)
        {
            var insMap = GlobalData.instance.sceneModelMgr.MapInstance;
            var curPos = _character.TransformSelf.position;

            targetPos = insMap.InverseTransformPoint(targetPos);
            curPos = insMap.InverseTransformPoint(curPos);

            Vector2Int viSrc = _sceneMap.Pixel2Grid(curPos.x, curPos.y);
            Vector2Int viDst = _sceneMap.Pixel2Grid(targetPos.x, targetPos.y);

            _vecPath.Clear();
            _vecPath = _sceneMap.FindPath(viSrc, viDst, 0, 5000);
        }

        public void Move(int nIndex)
        {
            _bIsMoving = true;

            if (nIndex >= _vecfPath.Count)
            {
                _bIsMoving = false;
                Play(AnimCfg.STAND, _nCurDir);
                return;
            }

            Vector3 tarPos = _vecfPath[nIndex];
            Vector3 curPos = _character.TransformSelf.position;
            tarPos.z = curPos.z;

            int nToDir = _sceneMap.DegreeToDir(tarPos.x - curPos.x, tarPos.y - curPos.y);

            float fDis = Vector3.Distance(curPos, tarPos);

            Play(AnimCfg.RUN, nToDir);
            _character.TransformSelf.DOMove(tarPos, fDis / _fSpeed).SetEase(Ease.Linear).OnComplete(() =>
            {
                Move(nIndex + 1);
            });
        }

        public void Play(string szName, int nDir)
        {
            _nCurDir = nDir;
            _character.AnimCtr.PlayAnimationByName(szName, nDir);
        }

        public void Turn(int nToDir, Action funComplete)
        {
            if (_turnDirTimer != null)
            {
                _turnDirTimer.Dispose();
                _turnDirTimer = null;
            }

            _turnDirTimer = new Timer((object state) =>
        {
            int nNextDir = _sceneMap.NextTurnDir(_nCurDir, nToDir);
            if (nNextDir != -1)
            {
                _nCurDir = nNextDir;
                //CDebug.Log($"~~~~~~~~~{AnimCfg.RUN}  --> {nNextDir}");
                _character.AnimCtr.PlayAnimationByName(AnimCfg.RUN, nNextDir);
            }

            if (nNextDir == nToDir)
            {
                if (funComplete != null)
                {
                    funComplete.Invoke();
                    _turnDirTimer.Dispose();
                    _turnDirTimer = null;
                }
            }

        }, null, 200, -1);

        }

        public void OnUpdate()
        {
            if (_bIsMoving)
            {
                Vector2 vPos = _character.TransformSelf.position;
                GlobalData.instance.heroMgr.SetClientPos(vPos);

                Vector2Int viSrc = _sceneMap.Pixel2Grid(vPos.x, vPos.y);
                GlobalData.instance.heroMgr.SetServerPos(viSrc);

                //遮挡关系目前按照y轴计算
                _character.GetComponentInChildren<SpriteRenderer>().sortingOrder = (int)(_sceneMap.m_nHeight - vPos.y);
            }
        }



    }
}