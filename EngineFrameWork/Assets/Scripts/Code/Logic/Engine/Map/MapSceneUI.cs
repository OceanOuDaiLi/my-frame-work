using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/********************************************************************
	Copyright © 2018 - 2050 by ShengMa.
	Created:	2023/07/31
	Filename: 	MapSceneUI.cs
	Author:		ShengMa
	Descriptions: 
*********************************************************************/
namespace GameEngine
{
    public class MapSceneUI : MonoBehaviour
    {
        public Transform m_map_root;
        public Transform m_map_sub_bg;
        public Transform m_map_sub_mask;
        public int m_nMapId;
        public BoxCollider2D m_boxCollider2D;

        private bool m_bMaskLoad = false;
        private Vector2 m_vecMapSize;

        private int m_nPixelPerUnit = 1;

        private int m_nCellPixelSize = 512;

        public List<Transform> m_vecSubMap;

        private SceneMap m_insScene;

        #region Unity Calls

        private void Init()
        {
            m_insScene = GlobalData.instance.sceneModelMgr.SceneMap;

            m_vecMapSize = m_boxCollider2D.size;

            Vector2 rootPos = new Vector2(-m_vecMapSize.x / 2, -m_vecMapSize.y / 2);
            m_map_sub_bg.localPosition = rootPos;
            m_map_sub_mask.localPosition = rootPos;

            //坐标归零 便于坐标计算
            m_map_root.position = new Vector2(m_vecMapSize.x / 2, m_vecMapSize.y / 2);
        }

        private void ProcessMapRes()
        {
            float fCellSize = m_nCellPixelSize / m_nPixelPerUnit;
            float fMapWidth = m_insScene.m_nWidth / m_nPixelPerUnit;
            float fMapHeight = m_insScene.m_nHeight / m_nPixelPerUnit;
            int nMaxWGrid = (int)Math.Ceiling(fMapWidth / fCellSize);
            int nMaxHGrid = (int)Math.Ceiling(fMapHeight / fCellSize);

            int nResCount = 1;
            string szSubName;
            for (int h = 0; h < nMaxHGrid; h++)
                for (int w = 0; w < nMaxWGrid; w++)
                {
                    szSubName = string.Format("{0:00}", nResCount);
                    Transform objSub = m_map_sub_bg.Find(szSubName);
                    SpriteRenderer insSubSprite = objSub.GetComponent<SpriteRenderer>();
                    float width = insSubSprite.sprite.rect.width / insSubSprite.sprite.pixelsPerUnit;
                    float height = insSubSprite.sprite.rect.height / insSubSprite.sprite.pixelsPerUnit;

                    float fX = (w * fCellSize + width / 2);// / fMapWidth;
                    float fY = (fMapHeight - (h * fCellSize + height / 2));// / fMapHeight;

                    Vector2 vecRealPos = new Vector2(fX, fY);
                    objSub.localPosition = vecRealPos;

                    nResCount++;
                }
        }

        private void ProcessMaskRes()
        {
            foreach (var mask in m_insScene.m_vecMaskInfo)
            {
                int nIndex = int.Parse(mask.szFileName);
                if (m_vecSubMap.Count > nIndex - 1)
                {
                    Transform objSub = m_vecSubMap[nIndex - 1];
                    SpriteRenderer render = objSub.GetComponent<SpriteRenderer>();

                    float fX = (render.sprite.rect.width / 2 + mask.x) / m_nPixelPerUnit;
                    float fY = (mask.y - render.sprite.rect.height / 2) / m_nPixelPerUnit;
                    objSub.localPosition = new Vector2(fX, fY);
                    Color color = render.color;
                    color.a = 0.5f;
                    render.color = color;
                    //由于mask的锚点在中间 所以需要转换到中间靠下的位置 这样人物遮挡关系才对
                    render.sortingOrder = (int)(m_vecMapSize.y - (fY - render.sprite.rect.height / 2));

                }
                else
                {
                    Debug.LogError("m_vecSubMap.Count < nIndex - 1");
                }

                //render.size = new Vector2(mask.width, mask.height);

            }
        }

        private void Start()
        {
            Init();

            ProcessMapRes();

            ProcessMaskRes();
        }

        private void Awake()
        {

        }

        private void OnDestroy()
        {

        }

        private void Update()
        {
            /*        if (m_bMaskLoad == false && GameMgr.Ins.SceneMgr.SceneMap.m_vecMaskInfo.Count > 0)
                    {
                        //CreateMask(GameMgr.Ins.SceneMapInstance.m_vecMaskInfo);
                        m_bMaskLoad = true;
                    }*/
        }

        #endregion
    }
}