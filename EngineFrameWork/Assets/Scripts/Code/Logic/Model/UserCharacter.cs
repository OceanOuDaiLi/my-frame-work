using GameEngine;
using System.Collections.Generic;

namespace Model
{
    public class UserCharacter
    {
        public bool isNpc { get; set; }                     // 是否npc
        public int instanceId { get; set; }                 // 实例id
        public List<object> data { get; set; }              // 服务器数据
        public bool isCreated { get; set; }                 // 是否已创建


        public Character Character { get; set; }            // 角色示例
        public BaseCharacter baseCharacter { get; set; }    // 配置数据

        // 测试数据
        public int frontBack { get; set; }                  // 0 前排，1 后排
        public int pos { get; set; }                        // 从做到右位置

        // other data .. 
    }
}
