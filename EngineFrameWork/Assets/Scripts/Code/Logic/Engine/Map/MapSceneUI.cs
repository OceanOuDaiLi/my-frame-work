using Model;
using System;
using FrameWork;
using UnityEngine;
using System.Collections.Generic;

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
        private Character m_insHero;

        private float m_fTagDistance = 40;

        private float m_fMiniDistance = 30;

        public Transform m_objDotRoot;
        private bool m_bMoving;

        private List<Transform> dotList = new List<Transform>();

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

            dotList = new List<Transform>();

            App.Instance.On("HeroOnMove", OnHeroMove);

            //主角地图移动
            InputCatcher.Ins.Event.AddListenerClick(m_map_root.gameObject, OnClick);
        }

        private void OnClick(InputCatcher.SingleClickData inputData)
        {
            HeroCharacter hero = GlobalData.instance.characterModelMgr.GetMapPlayerCharacter();
            GlobalData.instance.characterModelMgr.SetChooseCharacter(null);

            Vector3 worldPos = CameraMgr.Instance.BaseCamera.ScreenToWorldPoint(inputData.screenPosition);
            Vector3 vMapPos = transform.InverseTransformPoint(worldPos);
            hero.Character.MoveCtr.GotoAndDo(vMapPos);
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
            if (m_bMoving)
            {
                UpdateHeroMoveDot();

                //判断角色是否在屏幕内
                UpdateActiveCharacterInScreen();
            }
        }

        private void UpdateActiveCharacterInScreen()
        {
            if (m_bMoving)
            {
                var characterList = GlobalData.instance.characterModelMgr.GetAllCharacterData();
                foreach (var insChar in characterList.Values)
                {
                    var character = insChar.Character;
                    if (character == null)
                    {
                        continue;
                    }
                    bool bInScreen = IsObjectVisible(character.gameObject);
                    character.gameObject.SetActive(bInScreen);
                }

            }
        }

        private bool IsObjectVisible(GameObject obj)
        {
            Vector3 viewportPos = CameraMgr.Instance.BaseCamera.WorldToViewportPoint(obj.transform.position);
            if (viewportPos.x >= 0 && viewportPos.x <= 1 && viewportPos.y >= 0 && viewportPos.y <= 1 && viewportPos.z > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void OnHeroMove(object sender, EventArgs e)
        {
            m_bMoving = true;
            CreateHeroMovePathDot();
        }

        private void CreateHeroMovePathDot()
        {
            ClearPathTagImage();

            if (m_insHero == null)
                m_insHero = GlobalData.instance.characterModelMgr.GetMapPlayerCharacter().Character;

            if (m_insHero.GetCurPath().Count > 0)
            {
                List<Vector2> tPath = m_insHero.GetCurPath();
                Vector2 heroPos = m_insHero.GetPosition();

                List<Vector2> miniPath = tPath;
                for (int i = 0; i < miniPath.Count - 1; i++)
                {
                    float length = Vector2.Distance(miniPath[i], miniPath[i + 1]);
                    Vector2 subPos = miniPath[i + 1] - miniPath[i];
                    int tagNum = Mathf.CeilToInt(length / m_fTagDistance) - 1;

                    float addX = subPos.x / tagNum;
                    float addY = subPos.y / tagNum;
                    for (int k = 0; k < tagNum; k++)
                    {
                        Vector2 tagPos = new Vector2(miniPath[i].x + addX * k, miniPath[i].y + addY * k);
                        CreatePathTagImage(tagPos);
                    }

                    CreatePathTagImage(miniPath[i + 1]);
                }
            }
        }

        private void UpdateHeroMoveDot()
        {
            Vector2 vHeroPos = m_insHero.GetPosition();
            float miniMapX = vHeroPos.x;
            float miniMapY = vHeroPos.y;

            m_bMoving = false;
            for (int i = 0; i < dotList.Count; i++)
            {
                GameObject tarObj = dotList[i].gameObject;
                if (tarObj.activeSelf)
                {
                    m_bMoving = true;
                    Vector2 imagePos = tarObj.transform.position;
                    float length = Vector2.Distance(imagePos, new Vector2(miniMapX, miniMapY));
                    if (length <= m_fMiniDistance)
                    {
                        tarObj.SetActive(false);
                        for (int k = 0; k <= i; k++)
                        {
                            dotList[k].gameObject.SetActive(false);
                        }
                        break;
                    }
                }
            }
        }

        private void CreatePathTagImage(Vector2 tagPos)
        {
            var dot = MapPool.Ins.SpawnMapCell(GameConfig.MAP_DOT_PATH_OBJ);
            dot.SetParent(m_objDotRoot);
            dot.position = tagPos;
            dotList.Add(dot);

        }

        private void ClearPathTagImage()
        {
            foreach (var item in dotList)
            {
                MapPool.Ins.DespawnMapCell(item);
            }
            dotList = new List<Transform>();

            // todo: 进入idle状态后，也需要执行一次 ClearPathTagImage，否则，没有将使用结束后的放回缓冲池

        }
        #endregion
    }
}