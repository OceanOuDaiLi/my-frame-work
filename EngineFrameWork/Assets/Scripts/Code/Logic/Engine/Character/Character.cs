using Model;
using UnityEngine;

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
        private Character enemy;
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        private Transform _transformSelf = null;
        private GameObject _gameObjectSelf = null;

        // Public Unity Components.
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
        public Character Enemy
        {
            get { return enemy; }
            set { enemy = value; }
        }

        // Controls
        [HideInInspector]
        public MoveCtr MoveCtr;
        [HideInInspector]
        public AnimatorCtr AnimCtr;
        [HideInInspector]
        public CharacterProperty Property;

        private bool _initialized = false;

        #region Unity Calls

        public void Initialized(UserCharacter userCharacter)
        {
            Property = new CharacterProperty();
            Property.OnEnable(this);
            Property.SetPropertyData(userCharacter);


            if (userCharacter.isNpc)
            {
                MoveCtr = null;
            }
            else
            {
                MoveCtr = new MoveCtr();
                MoveCtr.Initialized(this);
            }

            if (AnimCtr == null) { AnimCtr = new AnimatorCtr(); }
            AnimCtr.Initialized(this);
            AnimCtr.PlayAnimationByName(AnimCfg.STAND, (int)Property.UserCharacter.data[2]);

            SpriteNode.sortingOrder = userCharacter.isNpc ? GameConfig.NpcCharacter_OrderInLayer : GameConfig.UserCharacter_OrderInLayer;

            _initialized = true;
        }

        public virtual void Update()
        {
            if (!_initialized) { return; }

            if (MoveCtr != null)
                MoveCtr.OnUpdate();
        }

        protected virtual void FixedUpdate()
        {
        }

        private void LateUpdate()
        {
        }

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
        }
    }
}
