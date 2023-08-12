using Model;
using GameEngine;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using strange.extensions.mediation.impl;

namespace UI
{
    public class NamePanelView : EventView
    {
        private NamePanelMediator mediator;

        private Dictionary<int, BaseCharacter> m_dicChar;
        private Dictionary<int, Transform> m_dicText;

        List<Character> m_vChar;

        private Camera m_baseCamera;
        private bool showNamePanel = false;

        public void BindMediator(NamePanelMediator _mediator)
        {
            mediator = _mediator;
        }

        protected override void Start()
        {
            base.Start();
            m_dicText = new Dictionary<int, Transform>();
            m_dicChar = GlobalData.instance.characterModelMgr.GetAllCharacterData();
            //m_textTemplate.gameObject.SetActive(false);

            showNamePanel = true;
        }

        public void OnGameFightStart()
        {
            showNamePanel = false;
            foreach (var item in m_dicText)
            {
                item.Value.gameObject.SetActive(false);
            }
        }

        public void OnGameFightEnd()
        {

        }


        //待优化 todo shenma
        private void NameUpdate()
        {
            var listKeys = m_dicChar.Keys;
            foreach (var szKey in m_dicChar.Keys)
            {
                BaseCharacter instance = m_dicChar[szKey];

                Character insChar = instance.Character;
                if (insChar == null || insChar.isActiveAndEnabled == false)
                {
                    RemoveName(szKey);
                    continue;
                }

                Transform character = insChar.transform;
                Transform insText;
                if (m_dicText.ContainsKey(szKey) == false)
                {
                    insText = UIPool.Ins.SpawnUI(GameConfig.UI_NAMEPANEL);
                    insText.SetParent(transform);
                    m_dicText[szKey] = insText;

                    var insMeshText = insText.GetComponent<Text>();
                    insMeshText.text = instance.Property.ConfigProperty.name;
                }
                else
                {
                    insText = m_dicText[szKey];
                }

                if (m_baseCamera == null)
                    m_baseCamera = CameraMgr.Instance.BaseCamera;

                insText.position = m_baseCamera.WorldToScreenPoint(character.position);
            }
        }

        private void RemoveName(int nKey)
        {
            if (m_dicText.ContainsKey(nKey))
            {
                Transform insText = m_dicText[nKey];
                UIPool.Ins.DespawnUI(insText);
                m_dicText.Remove(nKey);
            }
        }

        private void LateUpdate()
        {
            if (!showNamePanel) { return; }
            NameUpdate();
        }

        public void OnDestroy()
        {
            foreach (var item in m_dicText.Values)
            {
                UIPool.Ins.DespawnUI(item);
            }
            m_dicText.Clear();
            m_dicText = null;
        }

    }
}