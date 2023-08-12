using UI;
using GameEngine;
using UnityEngine;

namespace Model
{
    public class NpcCharacter : BaseCharacter
    {
        public NpcCharacter()
        {
            ;
        }

        override public void OnInit()
        {
            base.OnInit();
            Property.CharacterType = CharacterType.MAP_NPC;
        }

        public override void OnCreateMono(Character ins)
        {
            base.OnCreateMono(ins);

            SetMonoName(GetResID() + "_Npc");

            ins.AddClickEvent(OnClick);
        }

        private void OnClick(InputCatcher.SingleClickData clickData)
        {
            var objClick = clickData.clickTarget;
            GlobalData.instance.characterModelMgr.SetChooseCharacter(Character);

            var hero = GlobalData.instance.characterModelMgr.GetMapPlayerCharacter().Character;
            Vector3 worldPos = CameraMgr.Instance.BaseCamera.ScreenToWorldPoint(clickData.screenPosition);
            hero.MoveCtr.GotoAndDo(worldPos, 5, () =>
            {
                //Ãæ¶ÔÖ÷½Ç
                Play(AnimCfg.STAND, (hero.MoveCtr.CurDir + 4) % 8);

                UIConfig npcChatView = new UIConfig();
                npcChatView.floaderName = "npcchat";
                npcChatView.prefabName = "npcchat";
                npcChatView.hideAllBefore = false;
                npcChatView.fullScreen = false;
                UIMgr.Ins.OpenUI(npcChatView, (s) =>
                {

                });
                CDebug.Log("Talk to " + objClick.transform.parent.name);
            });

            CDebug.Log("Goto : " + objClick.transform.parent.name);
        }
    }
}
