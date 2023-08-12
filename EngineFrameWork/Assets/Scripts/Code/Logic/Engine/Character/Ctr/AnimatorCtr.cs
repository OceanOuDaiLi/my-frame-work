using System;
using UnityEngine;
using System.Collections.Generic;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2023/07/27
	Filename: 	Animator.cs
	Author:		DaiLi.Ou
	Descriptions: 角色动画模块
*********************************************************************/
namespace GameEngine
{
    [Serializable]
    public class AnimatorCtr : ISerializationCallbackReceiver, IDisposable
    {
        private Animator _animator;
        private Character _character;

        /// <summary>
        /// 描述BlendTree Input x & y
        /// 0 - 7: 
        /// 0 为 down 动画
        /// 顺时针方向描述动画
        /// </summary>
        private Dictionary<int, Vector2> blendTreeDir = new Dictionary<int, Vector2>()
        {
            { 0, new Vector2(0, -1)},           // down
            { 1, new Vector2(-1, -1)},          // left down
            { 2, new Vector2(-1, 0)},           // left
            { 3, new Vector2(-1, 1)},           // left up
            { 4, new Vector2(0, 1)},            // up
            { 5, new Vector2(1, 1)},            // right up
            { 6, new Vector2(1, 0)},            // right
            { 7, new Vector2(1, -1)},           // right down
        };

        public void Initialized(Character _character)
        {
            this._character = _character;
            _animator = this._character.Animator;
        }

        public void PlayAnimationByName(string name, Vector2 direction)
        {
            SetAnimatorParameter(name, direction);
        }

        public void PlayAnimationByName(string name, int direction)
        {
            SetAnimatorParameter(name, direction);
        }

        public void SetAnimatorParameter(string animationName, int direction)
        {
            Vector2 dir = blendTreeDir[direction];
            switch (animationName)
            {
                case AnimCfg.STAND:
                    _animator.SetBool(AnimCfg.PARAM__BOOL_MOVEING, false);
                    break;
                case AnimCfg.RUN:
                    _animator.SetBool(AnimCfg.PARAM__BOOL_MOVEING, true);
                    break;
                default:
                    break;
            }

            SetBlendTreeParameter(dir);
        }

        public void SetAnimatorParameter(string animationName, Vector2 dir)
        {
            switch (animationName)
            {
                case AnimCfg.STAND:
                    _animator.SetBool(AnimCfg.PARAM__BOOL_MOVEING, false);
                    break;
                case AnimCfg.RUN:
                    _animator.SetBool(AnimCfg.PARAM__BOOL_MOVEING, true);
                    break;
                default:
                    break;
            }

            SetBlendTreeParameter(dir);
        }

        public void SetBlendTreeParameter(Vector2 dir)
        {
            _animator.SetFloat(AnimCfg.PARAM_INPUT_X, dir.x);
            _animator.SetFloat(AnimCfg.PARAM_INPUT_Y, dir.y);
        }

        public Vector2 GetReverseDirection(Vector2 dir)
        {
            var result = dir * (-1);
            return result;
        }

        public void Dispose()
        {
            if (_character != null) { _character = null; }
            if (_animator != null) { _animator = null; }
        }

        public void OnAfterDeserialize()
        {
        }

        public void OnBeforeSerialize()
        {
        }

    }
}