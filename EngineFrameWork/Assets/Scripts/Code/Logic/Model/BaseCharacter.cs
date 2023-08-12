using GameEngine;
using UnityEngine;

namespace Model
{
    public class BaseCharacter
    {
        public int InstanceId { get; set; }                             // 实例Id
        public Character Character { get; set; }                        // 角色mono实例

        public CharacterProperty Property { get; set; }                 // 角色配置属性

        public void Init(CharacterProperty property)
        {
            Property = property;
            OnInit();
        }

        public virtual void OnInit()
        {

        }

        public virtual void OnCreateMono(Character ins)
        {
            Character = ins;
            RefreshData();
        }

        public virtual void OnDispose()
        {
            // 引用类型 置空
        }

        public void SetCharacter(Character character)
        {
            Character = character;
        }

        public void Play(string szActionName, int nDir)
        {
            Property.MonoProperty.ActionName = szActionName;
            Property.MonoProperty.Dir = nDir;
            if (Character != null)
            {
                Character.Play(szActionName, nDir);
            }
        }

        public void SetHeadFlag(HeadFlagCfg szFlag)
        {
            Property.MonoProperty.HeadFlag = szFlag;
            if (Character != null)
            {
                Character.SetHeadFlag(szFlag);
            }
        }

        public void SetSortOrder(int nOrder)
        {
            Property.MonoProperty.SortOrder = nOrder;
            if (Character != null)
            {
                Character.UpdateSortLayer(nOrder);
            }
        }

        public void SetMonoName(string szName)
        {
            Property.MonoProperty.MonoName = szName;
            if (Character != null)
            {
                Character.SetMonoName(szName);
            }

        }

        public void SetParent(Transform transform, bool bWorld = true)
        {
            if (Character != null)
            {
                Character.TransformSelf.SetParent(transform, bWorld);
            }
        }

        public int GetResID()
        {
            return Property.ConfigProperty.resId;
        }

        public string GetResPath()
        {
            return Property.ConfigProperty.prefabPath;
        }

        public void RefreshData()
        {
            if (Character == null)
            {
                return;
            }

            Character.RefreshData(Property);
        }

        public string GetName()
        {
            return Property.ConfigProperty.name;
        }

        public void SetSpeed(float fSpeed)
        {
            Property.MonoProperty.Speed = fSpeed;
            if (Character != null)
            {
                Character.SetSpeed(fSpeed);
            }
        }
    }
}
