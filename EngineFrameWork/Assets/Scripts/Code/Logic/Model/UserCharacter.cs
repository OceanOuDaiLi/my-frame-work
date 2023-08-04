using GameEngine;
using System.Collections.Generic;

namespace Model
{
    public class UserCharacter
    {
        public bool isNpc { get; set; }                     // �Ƿ�npc
        public int instanceId { get; set; }                 // ʵ��id
        public List<object> data { get; set; }              // ����������
        public bool isCreated { get; set; }                 // �Ƿ��Ѵ���


        public Character Character { get; set; }            // ��ɫʾ��
        public BaseCharacter baseCharacter { get; set; }    // ��������

        // ��������
        public int frontBack { get; set; }                  // 0 ǰ�ţ�1 ����
        public int pos { get; set; }                        // ��������λ��

        // other data .. 
    }
}
