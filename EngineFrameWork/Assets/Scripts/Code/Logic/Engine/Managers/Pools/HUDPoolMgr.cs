using System;
using UnityEngine;
using System.Collections.Generic;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com
	Created:	2023/07/12
	Filename: 	HUDManager.cs
	Author:		DaiLi.Ou
	Descriptions: 
*********************************************************************/
namespace GameEngine
{
    [SerializeField]
    public class HUDPoolMgr : IDisposable
    {
        SpawnPool pool;
        Transform prefabDamageText;
        Transform prefabDamageTextCrit;
        Transform prefabHpBar;
        Transform prefabBubble;

        static Color[] hpBarColor = { new Color(80f / 255f, 255f / 255f, 154f / 255f), new Color(255f / 255f, 70f / 255f, 116f / 255f) };
        Dictionary<Character, HUDText> bubbleCachedDict = new Dictionary<Character, HUDText>();
        static int[][] fightNumAry = {
            new int[]{ 40, 41, 42, 43, 44, 45, 46, 47, 48, 49 },    //blue
            new int[]{ 50, 51, 52, 53, 54, 55, 56, 57, 58, 59 },    //green
            new int[]{ 60, 61, 62, 63, 64, 65, 66, 67, 68, 69 },    //orange
            new int[]{ 70, 71, 72, 73, 74, 75, 76, 77, 78, 79 },    //red
            new int[]{ 80, 81, 82, 83, 84, 85, 86, 87, 88, 89 },    //white
            new int[]{ 90, 91, 92, 93, 94, 95, 96, 97, 98, 99 }     //yellow
        };

        public HUDPoolMgr()
        {
            try
            {
                pool = PoolManager.Pools["HUD"];
                prefabDamageText = pool.prefabs["DamageText"];
                prefabDamageTextCrit = pool.prefabs["DamageTextCrit"];
                prefabHpBar = pool.prefabs["HpBar"];
                prefabBubble = pool.prefabs["BubbleText"];
            }
            catch
            {
                pool = null;
                prefabDamageText = null;
                prefabDamageTextCrit = null;
                prefabHpBar = null;
                prefabBubble = null;
            }
        }

        ~HUDPoolMgr()
        {
            Dispose();
        }

        public void Despawn(Transform inst)
        {
            if (pool == null) return;
            pool.Despawn(inst);
        }

        string GetFightNum(int num, int type)
        {
            char[] txt = num.ToString().ToCharArray();
            for (int i = 0; i < txt.Length; i++)
            {
                txt[i] = (char)(txt[i] - 8 + type * 10);
            }
            return new string(txt);
        }

        public void NewBlueText(int num, Character owner)
        {
            NewDamageText(GetFightNum(num, 0), owner, prefabDamageText);
        }

        public void NewGreenText(int num, Character owner)
        {
            NewDamageText(GetFightNum(Mathf.Abs(num), 1), owner, prefabDamageText);
        }

        public void NewOrangeText(int num, Character owner)
        {
            NewDamageText(GetFightNum(num, 2), owner, prefabDamageText);
        }

        public void NewRedText(int num, Character owner)
        {
            NewDamageText(GetFightNum(num, 3), owner, prefabDamageText);
        }

        public void NewWhiteText(int num, Character owner)
        {
            NewDamageText(GetFightNum(num, 4), owner, prefabDamageTextCrit);
        }

        public void NewYellowText(int num, Character owner)
        {
            NewDamageText(GetFightNum(num, 5), owner, prefabDamageText);
        }

        public void NewMissText(Character owner)
        {
            NewDamageText("m", owner, prefabDamageText);
        }

        void NewDamageText(string txt, Character owner, Transform prefab)
        {
            if (pool == null) return;
            //if (HUDControl.Instance.CanBeView(owner.Collider.bounds))
            //{
            //    Transform inst = pool.Spawn(prefab);
            //    HUDText hudText = inst.GetComponent<HUDText>();
            //    hudText.SetText(txt);
            //    hudText.SetTarget(owner.TransformSelf, owner.Collider.bounds);
            //    hudText.Duration = 1.6667f;
            //    hudText.pool = pool;
            //    HUDControl.Instance.AddObject(hudText);
            //}
        }

        public void NewBubble(string txt, Character owner)
        {
            if (pool == null) return;
            if (prefabBubble == null)
            {
                Debug.Log(owner);
            }
            HUDText hudText = null;
            if (bubbleCachedDict.TryGetValue(owner, out hudText))
            {
                //尚未被缓存池回收
                if (hudText != null && hudText.gameObject.activeSelf)
                {
                    hudText.SetText(txt);
                    hudText.Duration = 2f;
                    return;
                }
            }

            //Transform inst = pool.Spawn(prefabBubble);
            //hudText = inst.GetComponent<HUDText>();
            //hudText.SetText(txt);
            //hudText.SetTarget(owner.TransformSelf, owner.Collider.bounds);
            //hudText.Duration = 2f;
            //hudText.pool = pool;
            //hudText.onDespawn = () => { bubbleCachedDict.Remove(owner); };
            //HUDControl.Instance.AddObject(hudText);

            //添加或更新掉原本旧的已被回收的那个
            bubbleCachedDict[owner] = hudText;
        }

        //public HUDHpBar GetPlayerHpBar(Character owner)
        //{
        //    return GetHpBar(hpBarColor[0], owner.TransformSelf, owner.Collider.bounds);
        //}

        //public HUDHpBar GetEnemyHpBar(Character owner)
        //{
        //    return GetHpBar(hpBarColor[1], owner.TransformSelf, owner.Collider.bounds);
        //}

        //HUDHpBar GetHpBar(Color color, Transform target, Bounds targetBounds)
        //{
        //    if (pool == null) return null;
        //    Transform inst = pool.Spawn(prefabHpBar, new Vector3(9999, 9999, 9999), Quaternion.identity);
        //    HUDHpBar hudHpBar = inst.GetComponent<HUDHpBar>();
        //    hudHpBar.SetColor(color);
        //    hudHpBar.SetValue(1f);
        //    hudHpBar.SetTarget(target, targetBounds);
        //    hudHpBar.Duration = float.PositiveInfinity;
        //    HUDControl.Instance.AddObject(hudHpBar);

        //    return hudHpBar;
        //}

        //hpBar是需要手动Remove
        //public void RemoveHUDHpBar(HUDHpBar hpBar)
        //{
        //    if (hpBar == null) return;

        //    HUDControl.Instance.RemoveObject(hpBar);
        //    pool.Despawn(hpBar.Rect);
        //}

        public void Dispose()
        {
            bubbleCachedDict.Clear();
            bubbleCachedDict = null;
            prefabDamageText = null;
            prefabDamageTextCrit = null;
            prefabHpBar = null;
            prefabBubble = null;
            pool = null;
        }
    }
}