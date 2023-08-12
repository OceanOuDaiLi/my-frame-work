
/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2023/07/12
	Filename: 	AnimCfg.cs
	Author:		DaiLi.Ou
	Descriptions: 动画模块使用的动画名称列举
*********************************************************************/
namespace GameEngine
{
    public class AnimCfg
    {
        // Animator Parameters
        public const string PARAM_INPUT_X = "InputX";
        public const string PARAM_INPUT_Y = "InputY";

        public const string PARAM_TRIGGER_STAND = "Stand";
        public const string PARAM_TRIGGER_CHASEING = "Chaseing";
        public const string PARAM_TRIGGER_ATTACK = "Attack";
        public const string PARAM_TRIGGER_DEFEND = "Defend";
        public const string PARAM_TRIGGER_BEATTACKED = "BeAttacked";

        public const string PARAM__BOOL_MOVEING = "Moveing";
        public const string PARAM_BOOL_CHASE_BACK = "ChaseBack";

        // Animation Name
        public const string RUN = "Run";
        public const string STAND = "Stand";
    }
}