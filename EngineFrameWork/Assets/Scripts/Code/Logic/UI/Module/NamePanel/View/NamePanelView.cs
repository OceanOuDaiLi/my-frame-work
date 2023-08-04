using GameEngine;
using Model;
using strange.extensions.mediation.impl;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class NamePanelView : EventView
    {
        private NamePanelMediator mediator;

        private Dictionary<int, UserCharacter> m_dicChar;
        private Dictionary<int, Transform> m_dicText;

        List<Character> m_vChar;

        public Transform m_textTemplate;
        private Camera m_baseCamera;

        public void BindMediator(NamePanelMediator _mediator)
        {
            mediator = _mediator;
        }

        private void Start()
        {
            m_dicText = new Dictionary<int, Transform>();
            m_dicChar = GlobalData.instance.characterModelMgr.GetAllCharacterData();
            m_textTemplate.gameObject.SetActive(false);

            m_baseCamera = CameraMgr.Instance.BaseCamera;

        }

        private void NameUpdate()
        {
            foreach (var szKey in m_dicChar.Keys)
            {
                UserCharacter instance = m_dicChar[szKey];
                Transform character = instance.Character.transform;
                Transform insText;
                if (m_dicText.ContainsKey(szKey) == false)
                {
                    var obj = Instantiate(m_textTemplate.gameObject);
                    obj.SetActive(true);
                    insText = obj.transform;
                    insText.SetParent(m_textTemplate.parent);

                    m_dicText[szKey] = insText;

                    var insMeshText = obj.GetComponent<Text>();
                    insMeshText.text = (string)instance.data[3];
                }
                else
                {
                    insText = m_dicText[szKey];
                }

                insText.position = m_baseCamera.WorldToScreenPoint(character.position);
            }
        }

        private void Update()
        {
            NameUpdate();
        }

    }
}