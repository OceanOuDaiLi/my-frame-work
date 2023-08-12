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
    public class CharacterAnimCallback : MonoBehaviour
    {
        private Animator animator;

        private Character owner = null;
        public Character Owner
        {
            get
            {
                if (owner == null)
                {
                    owner = GetComponent<Character>();
                    if (owner == null) owner = GetComponentInParent<Character>();
                }
                return owner;
            }
        }

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        //private void OnDisable()
        //{

        //}

        public void DefendStart()
        {

        }

        public void DefendEnd()
        {
            Owner.DefendEnd();
        }

        public void BeAttackEnd()
        {
            Owner.BeAttackedEnd();
        }

        public void AttackEnd(int attackIdx)
        {
            Owner.AttackEnd(attackIdx);
        }

        public void AttackStart(int attackIdx)
        {
            Owner.AttackStart(attackIdx);
        }

    }
}
