using GameEngine;
using UnityEngine;

namespace Model
{
    public class HeroCharacter : BaseCharacter
    {
        public HeroCharacter()
        {
            ;
        }

        public override void OnInit()
        {
            base.OnInit();
            Property.CharacterType = CharacterType.MAP_HERO;

            Property.MonoProperty.ActionName = AnimCfg.STAND;
            Property.MonoProperty.Dir = 0;
            Property.MonoProperty.Speed = 350.0f;
        }

        public override void OnCreateMono(Character ins)
        {
            base.OnCreateMono(ins);

            SetMonoName(GetResID() + "_Hero");

            //主角不接受点击事件
            ins.GetComponentInChildren<BoxCollider2D>().enabled = false;
        }

    }
}
