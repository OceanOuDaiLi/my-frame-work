
using System;
using System.Collections.Generic;
using Proto.Login;
using Proto.Player;
using Proto.Scene;
using Proto.Npc;
using Proto.Battle;
using Proto.Bag;


public class ProtoConst
{

	protected static Dictionary<short, Type> m_dicS2CProto = new Dictionary<short, Type>
	{
		{102 , typeof(RspRegisterAccount)  },
		{104 , typeof(RspLogin)  },
		{106 , typeof(RspCreateRole)  },
		{108 , typeof(RspEnterGame)  },
		{110 , typeof(NtfQuickUpdateList)  },
		{112 , typeof(RspVerifyIDCard)  },
		{114 , typeof(NtfKickOutFd)  },
		{116 , typeof(NtfVerifyInfo)  },
		{202 , typeof(RspHeartBeat)  },
		{203 , typeof(NtfPlayerProp)  },
		{204 , typeof(NtfPlayerInfoSync)  },
		{212 , typeof(RspGM)  },
		{224 , typeof(RspRename)  },
		{226 , typeof(NtfClientConfig)  },
		{228 , typeof(RspSaveConfig)  },
		{230 , typeof(NtfKickOutPlayer)  },
		{238 , typeof(NtfLoadFinish)  },
		{248 , typeof(RspGetPlayerInfo)  },
		{252 , typeof(RspUseCDKey)  },
		{272 , typeof(RspGetServerTime)  },
		{302 , typeof(NtfMapTrack)  },
		{304 , typeof(NtfMapAddPlayer)  },
		{306 , typeof(NtfMapAddSummon)  },
		{308 , typeof(NtfMapDelSummon)  },
		{310 , typeof(NtfMapAddNpc)  },
		{312 , typeof(NtfMapAddGoods)  },
		{313 , typeof(NtfHeroGoto)  },
		{314 , typeof(NtfHeroEnterScene)  },
		{402 , typeof(RspNpcLook)  },
		{404 , typeof(NtfNpcTalk)  },
		{406 , typeof(RspNpcRespond)  },
		{501 , typeof(NtfBattleStart)  },
		{502 , typeof(NtfEnterBattle)  },
		{503 , typeof(NtfBattleEnd)  },
		{504 , typeof(NtfRoundStart)  },
		{505 , typeof(NtfRoundCommand)  },
		{506 , typeof(NtfRoundFight)  },
		{507 , typeof(NtfRoundEnd)  },
		{508 , typeof(NtfTurnStart)  },
		{509 , typeof(NtfTurnEnd)  },
		{510 , typeof(NtfCommandSkillStart)  },
		{511 , typeof(NtfCommandSkillEnd)  },
		{512 , typeof(NtfCommandAttackStart)  },
		{513 , typeof(NtfCommandAttackEnd)  },
		{514 , typeof(NtfCommandUseItemStart)  },
		{515 , typeof(NtfCommandUseItemEnd)  },
		{516 , typeof(NtfCommandSummonStart)  },
		{517 , typeof(NtfCommandSummonEnd)  },
		{518 , typeof(NtfCommandEscapeStart)  },
		{519 , typeof(NtfCommandEscapeEnd)  },
		{520 , typeof(NtfActionSkillStart)  },
		{521 , typeof(NtfActionSkillEnd)  },
		{522 , typeof(NtfActionAttackStart)  },
		{523 , typeof(NtfActionAttackEnd)  },
		{524 , typeof(NtfActionUseItemStart)  },
		{525 , typeof(NtfActionUseItemEnd)  },
		{526 , typeof(NtfActionSummonStart)  },
		{527 , typeof(NtfActionSummonEnd)  },
		{528 , typeof(NtfActionEscapeStart)  },
		{529 , typeof(NtfActionEscapeEnd)  },
		{538 , typeof(NtfAssaultStart)  },
		{539 , typeof(NtfAssaultEnd)  },
		{540 , typeof(NtfPerformInfo)  },
		{541 , typeof(NtfPerformShout)  },
		{542 , typeof(NtfPerformRun)  },
		{543 , typeof(NtfAttackInfo)  },
		{544 , typeof(NtfAttackRun)  },
		{545 , typeof(NtfPopUpNumber)  },
		{701 , typeof(NtfBagInfo)  },
		{702 , typeof(NtfItemInfo)  },
		{703 , typeof(NtfItemUpdate)  },
		{704 , typeof(NtfItemDel)  },
		{705 , typeof(RspUseItem)  },

	};

	protected static Dictionary<string, short> m_dicC2SProto = new Dictionary<string, short>
	{
		{"Proto.Login.ReqRegisterAccount" , 101},
		{"Proto.Login.ReqLogin" , 103},
		{"Proto.Login.ReqCreateRole" , 105},
		{"Proto.Login.ReqEnterGame" , 107},
		{"Proto.Login.ReqVerifyIDCard" , 111},
		{"Proto.Player.ReqHeartBeat" , 201},
		{"Proto.Player.ReqGM" , 211},
		{"Proto.Player.ReqRename" , 223},
		{"Proto.Player.ReqSaveConfig" , 227},
		{"Proto.Player.ReqGetPlayerInfo" , 247},
		{"Proto.Player.ReqUseCDKey" , 251},
		{"Proto.Player.ReqGetServerTime" , 271},
		{"Proto.Scene.ReqMapMove" , 301},
		{"Proto.Npc.ReqNpcLook" , 401},
		{"Proto.Npc.ReqNpcRespond" , 405},
		{"Proto.Battle.ReqSetCommandSkill" , 530},
		{"Proto.Battle.ReqSetCommandAttack" , 531},
		{"Proto.Battle.ReqSetCommandAI" , 532},
		{"Proto.Battle.ReqSetCommandUseItem" , 533},
		{"Proto.Battle.ReqSetCommandSummon" , 534},
		{"Proto.Battle.ReqSetCommandDefend" , 535},
		{"Proto.Battle.ReqSetCommandEscape" , 536},
		{"Proto.Battle.ReqSetCommandProtect" , 537},
		{"Proto.Bag.ReqUseItem" , 705},

	};
}
