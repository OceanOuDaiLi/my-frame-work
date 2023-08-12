/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2023/08/10
	Filename: 	RunStateBehaviour.cs
	Author:		DaiLi.Ou
	Descriptions: Character Run FSM
*********************************************************************/

using UnityEngine;

namespace GameEngine
{
    public class RunStateBehaviour : BaseStateBehaviour
    {
        bool isPlayer = false;
        public override void OnInitialize()
        {
            this.isPlayer = owner.Property.CharacterType == CharacterType.MAP_PLAYER;
        }

        public override void OnLocalStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            // cant not move. if owner,is not CharacterType.MAP_PLAYER.
            if (!isPlayer) return;

            // if character has created. [MoveCtr]s' update chould using animators' mono update.

        }
    }
}
