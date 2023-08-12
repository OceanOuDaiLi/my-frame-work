using Model;
using System;
using UnityEngine;
using Core.Interface.Event;
using System.Collections.Generic;
using static UnityEngine.UI.GridLayoutGroup;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2023/07/12
	Filename: 	Character.cs
	Author:		DaiLi.Ou
	Descriptions: 
*********************************************************************/
namespace GameEngine
{
    public class Character : MonoBehaviour
    {
        // Private Unity Components.
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        private Transform _transformSelf = null;
        private GameObject _gameObjectSelf = null;

        private GameObject _spriteObjectSelf = null;
        private Transform _spriteTransSelf = null;

        // Public Unity Components.
        public Transform SpriteTransSelf
        {
            get => _spriteTransSelf ?? (_spriteTransSelf = SpriteNode.transform);
        }
        public GameObject SpriteObjectSelf
        {
            get => _spriteObjectSelf ?? (_spriteObjectSelf = SpriteNode.gameObject);
        }
        public SpriteRenderer SpriteNode
        {
            get => _spriteRenderer ?? (_spriteRenderer = Animator.GetComponent<SpriteRenderer>());
        }
        public GameObject GameObjectSelf
        {
            get => _gameObjectSelf ?? (_gameObjectSelf = gameObject);
        }
        public Transform TransformSelf
        {
            get => _transformSelf ?? (_transformSelf = transform);
        }
        public Animator Animator
        {
            get => _animator ?? (_animator = GameObjectSelf.GetComponentInChildren<Animator>());
        }

        // Controls
        [HideInInspector]
        public MoveCtr MoveCtr;
        [HideInInspector]
        public AnimatorCtr AnimCtr;
        [HideInInspector]
        public CharacterProperty Property;
        public AnimationCurve moveCurve = new AnimationCurve();       // 角色战斗 -> 移动曲线  

        private bool _initialized = false;
        private IEventHandler eventHandlerInputSingleClick = null;
        private InputCatcher.IEventHandle clickHandle = null;

        private Transform characterSelectTrans = null;

        private Transform characterHeadFlagTrans = null;

        #region Unity Calls

        public void RefreshData(CharacterProperty property)
        {
            _initialized = false;

            if (Property != null) { Property.Dispose(); }
            Property = property;                                                    // if needed.opt for deep clone. command by daili.ou

            RefreshMoveCtr();

            RefreshAnimCtr();

            RefreshMono();

            _initialized = true;
        }

        private void RefreshAnimCtr()
        {
            if (AnimCtr != null) { AnimCtr.Dispose(); }
            AnimCtr = new AnimatorCtr();
            AnimCtr.Initialized(this);
            AnimCtr.PlayAnimationByName(AnimCfg.STAND, Property.MonoProperty.Dir);
        }

        private void RefreshMoveCtr()
        {
            if (Property.IsFightHero) { return; }

            if (MoveCtr != null) { MoveCtr.Dispose(); }
            MoveCtr = new MoveCtr();
            MoveCtr.Initialized(this);
        }

        private void RefreshMono()
        {
            int nSortLayer = GlobalData.instance.sceneModelMgr.SceneMap.m_nHeight - (int)TransformSelf.position.y;
            UpdateSortLayer(nSortLayer);

            //SpriteNode.sortingOrder = Property.MonoProperty.SortOrder;
            TransformSelf.position = Property.MonoProperty.Pos;

            SetHeadFlag(Property.MonoProperty.HeadFlag);

            SetSpeed(Property.MonoProperty.Speed);
        }


        public InputCatcher.IEventHandle AddClickEvent(Action<InputCatcher.SingleClickData> callback)
        {
            return InputCatcher.Ins.Event.AddListenerClick(SpriteObjectSelf, callback);
        }


        public virtual void Update()
        {
            if (!_initialized) { return; }

            if (MoveCtr != null)
                MoveCtr.OnUpdate();
        }

        //private void OnDestroy()
        //{
        //    if (characterHeadFlagTrans != null)
        //    {
        //        MapPool.Ins.DespawnMapCell(characterHeadFlagTrans);
        //        characterHeadFlagTrans = null;
        //    }

        //    if (characterSelectTrans != null)
        //    {
        //        MapPool.Ins.DespawnMapCell(characterSelectTrans);
        //        characterSelectTrans = null;
        //    }
        //}

#if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            //MoveCtr.OnDrawGizmos();
        }
#endif

        #endregion

        public void UpdateSortLayer(int order)
        {
            SpriteNode.sortingOrder = order;
            if (characterHeadFlagTrans != null)
            {
                characterHeadFlagTrans.GetComponent<SpriteRenderer>().sortingOrder = order;
            }
        }

        //当前寻路路径
        public List<Vector2> GetCurPath()
        {
            return MoveCtr.CurPath;
        }

        public Vector2 GetPosition()
        {
            return TransformSelf.position;
        }

        public void SetChooseFlag(bool bChoose)
        {
            if (bChoose)
            {
                characterSelectTrans = MapPool.Ins.SpawnMapCell(GameConfig.MAP_CHARACTER_SELECTED_OBJ);
                characterSelectTrans.SetParent(TransformSelf);
                characterSelectTrans.localPosition = Vector3.zero;
            }
            else
            {
                MapPool.Ins.DespawnMapCell(characterSelectTrans);
                characterSelectTrans = null;
            }
        }

        public void SetHeadFlag(HeadFlagCfg szFlag)
        {
            if (szFlag.Equals(HeadFlagCfg.DEFAULT_NONE))
                return;

            Transform headTransform = null;
            switch (szFlag)
            {
                case HeadFlagCfg.TASK_FLAG_COMPLETE:
                    headTransform = MapPool.Ins.SpawnMapCell(GameConfig.MAP_TASK_FLAG_COMPLETE);
                    break;
                case HeadFlagCfg.TASK_FLAG_AVALIABLE:
                    headTransform = MapPool.Ins.SpawnMapCell(GameConfig.MAP_TASK_FLAG_AVALIABLE);
                    break;
                default:
                    break;
            }

            if (headTransform != null)
            {
                if (characterHeadFlagTrans != null)
                {
                    MapPool.Ins.DespawnMapCell(characterHeadFlagTrans);
                    characterHeadFlagTrans = null;
                }

                headTransform.SetParent(TransformSelf);
                headTransform.localPosition = new Vector3(0, 330, 0);
                headTransform.localScale = new Vector3(1.5f, 1.5f, 1);
                characterHeadFlagTrans = headTransform;
            }
        }

        public void Play(string szActionName, int nDir)
        {
            AnimCtr.PlayAnimationByName(szActionName, nDir);
        }

        public void Play(string animatName, Vector2 dir)
        {
            AnimCtr.PlayAnimationByName(animatName, dir);
        }

        public Vector2 GetReverseDirection(Vector2 dir)
        {
            return AnimCtr.GetReverseDirection(dir);
        }

        public void SetBlendTreeParameter(Vector2 dir)
        {
            AnimCtr.SetBlendTreeParameter(dir);
        }

        public void SetMonoName(string szName)
        {
            GameObjectSelf.name = szName;
        }

        public void SetParent(Transform transform)
        {
            TransformSelf.SetParent(transform);
        }

        public void SetSpeed(float fSpeed)
        {
            MoveCtr.Speed = fSpeed;
        }



        #region Fight Methods

        Character enemy;
        Character demageSource;
        bool isDefendState = false;
        Vector2 _animDir = Vector2.zero;

        public Action AttackDown;



        public Character Enemy
        {
            get { return enemy; }
            set { enemy = value; }
        }

        public void ResetGround()
        {
            isDefendState = false;
            _animDir = Vector2.zero;
        }

        public void ChaseEnemy(Vector2 dir)
        {
            if (dir.Equals(_animDir)) { return; }
            _animDir = dir;

            SetBlendTreeParameter(dir);
        }

        public void ChaseBack()
        {
            SetAnimatorBool(AnimCfg.PARAM_BOOL_CHASE_BACK, true);
            SetAnimatorTrigger(AnimCfg.PARAM_TRIGGER_CHASEING);
        }

        public void DoAttack()
        {
            SetAnimatorTrigger(AnimCfg.PARAM_TRIGGER_ATTACK);
        }

        public void OnDefend()
        {
            isDefendState = true;
        }

        public void DefendEnd()
        {
            Stand();

            demageSource.ChaseBack();
        }

        public void BeAttacked(Vector2 dir, Character demageSource)
        {
            this.demageSource = demageSource;
            SetBlendTreeParameter(dir);
            if (isDefendState)
            {
                SetAnimatorTrigger(AnimCfg.PARAM_TRIGGER_DEFEND);
            }
            else
            {
                SetAnimatorTrigger(AnimCfg.PARAM_TRIGGER_BEATTACKED);
            }
        }

        public void BeAttackedEnd()
        {
            demageSource.ChaseBack();
        }

        public void Stand()
        {
            SetAnimatorTrigger(AnimCfg.PARAM_TRIGGER_STAND);
            Play(AnimCfg.STAND, Property.MonoProperty.Dir);

            AttackDown?.Invoke();
            // SkillDown?.Invoke();
        }


        public void AttackStart(int attackIdx)
        {
            Enemy.BeAttacked(GetReverseDirection(_animDir), this);
        }

        public void AttackEnd(int attackIdx)
        {
            Play(AnimCfg.STAND, 4);
        }


        #endregion

        #region Animation Param & Methods

        public void SetAnimatorTrigger(string key)
        {
            _animator.SetTrigger(key);
        }

        public void SetAnimatorBool(string key, bool value)
        {
            _animator.SetBool(key, value);
        }

        public bool GetAnimatorBool(string key)
        {
            return _animator.GetBool(key);
        }

        public bool IsInState(string stateName)
        {
            return _animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
        }
        #endregion
    }
}
