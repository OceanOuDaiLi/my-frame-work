using Model;
using System;
using FrameWork;
using DG.Tweening;
using UnityEngine;
using System.Threading;
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
    public class MoveCtr : ISerializationCallbackReceiver, IDisposable
    {
        private float _fSpeed;

        private Transform _insMap;
        private SceneMap _sceneMap;
        private Character _character;

        private List<Vector2> _vecfPath;
        private List<Vector2Int> _vecPath;

        private Timer _turnDirTimer;

        private int _nCurDir;

        public int CurDir { get => _nCurDir; }

        public float Speed { get => _fSpeed; set => _fSpeed = value; }

        private bool _bIsMoving;

        private GameObject map_root;

        private InputCatcher.IEventHandle inputEventHandle = null;

        public List<Vector2> CurPath
        {
            get { return _vecfPath; }
        }

        #region Unity Calls

        public void Initialized(Character _character)
        {
            this._character = _character;
            _sceneMap = GlobalData.instance.sceneModelMgr.SceneMap;


            _vecfPath = new List<Vector2>();

            _nCurDir = 0;

            _fSpeed = 350.0f;

            _bIsMoving = false;
        }


        public void OnAfterDeserialize()
        {
        }

        public void OnBeforeSerialize()
        {
        }

        #endregion


        public bool GotoAndDo(Vector2 vTagPos, int nDropStep = 0, Action funComplete = null)
        {
            _character.TransformSelf.DOKill();


            Vector2 vHeroPos = _character.TransformSelf.position;

            Vector2Int vSrcGrid = _sceneMap.Pixel2Grid(vHeroPos.x, vHeroPos.y);
            Vector2Int vDstGrid = _sceneMap.Pixel2Grid(vTagPos.x, vTagPos.y);
            _vecPath = _sceneMap.FindPath(vSrcGrid, vDstGrid, nDropStep, 5000);

            _vecfPath.Clear();
            foreach (var vData in _vecPath)
            {
                Vector2 vec = _sceneMap.Grid2Pixel(vData.x, vData.y);
                _vecfPath.Add(vec);
            }

            App.Instance.Trigger("HeroOnMove", _vecfPath);
            //0是当前位置 从第二步开始
            Move(1, funComplete);
            return true;
        }

        public void Move(int nIndex, Action funComplete)
        {
            _bIsMoving = true;

            if (nIndex >= _vecfPath.Count)
            {
                _vecfPath.Clear();
                _bIsMoving = false;
                Play(AnimCfg.STAND, _nCurDir);
                if (funComplete != null)
                {
                    funComplete();
                }
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
                Move(nIndex + 1, funComplete);
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
                _character.UpdateSortLayer((int)(_sceneMap.m_nHeight - vPos.y));
            }
        }

        public void Dispose()
        {
            // 引用类型 置空
            if (_vecPath != null) { _vecPath.Clear(); }
            if (_vecfPath != null) { _vecfPath.Clear(); }
            if (_sceneMap != null) { _sceneMap.Dispose(); }
            _vecPath = null;
            _sceneMap = null;
            _vecfPath = null;

            if (_insMap != null) { _insMap = null; }
            if (map_root != null) { map_root = null; }
            if (_character != null) { _character = null; }

            if (inputEventHandle != null)
            {
                InputCatcher.Ins.Event.RemoveListenerClick(inputEventHandle);
                inputEventHandle = null;
            }
        }
    }
}