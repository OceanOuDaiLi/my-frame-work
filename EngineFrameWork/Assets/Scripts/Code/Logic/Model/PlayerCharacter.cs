using GameEngine;
using UnityEngine;

namespace Model
{
    public class PlayerCharacter : BaseCharacter
    {
        public PlayerCharacter()
        {
            ;
        }
        override public void OnInit()
        {
            base.OnInit();
            Property.CharacterType = CharacterType.MAP_PLAYER;
        }

        public override void OnCreateMono(Character ins)
        {
            base.OnCreateMono(ins);

            SetMonoName(GetResID() + "_Player");
        }
    }
}
