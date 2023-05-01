using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Author：Daili.OU
/// Created Time：2022/05/06
/// Descriptions：
/// 
/// Tips of 'BrushModelLevel':
/// While adding new level,Changing BrushItem Array Len.
/// </summary>
namespace AI.Tools
{
    public enum BrushModelLevel
    {
        LOW = 0,
        HIGH = 1,
    }

    public enum BrushModelFigure
    {
        DEFAULT = 0,
        SLIM = 1,
        BIGGER = 2,
    }

    public enum BrushModelType
    {
        HEAD = 0,
        CLOTH = 1,
        LIMBS = 2,
        SHOES = 3,
    }

    [CreateAssetMenuAttribute(fileName = "UV Brush Config", menuName = "UV/ UV Brush Config", order = 1)]
    public class UvBrushConfig : ScriptableObject
    {
        [Header("hands & foot")] public float _ScaleParamLimb = 0;
        [Header("pants")] public float _ScaleParamPants = 0;

        [HideInInspector] public HeadBrushConfig heads = new HeadBrushConfig();
        [HideInInspector] public ClothBrushConfig cloth = new ClothBrushConfig();
        [HideInInspector] public LimbsBrushConfig limbs = new LimbsBrushConfig();
        [HideInInspector] public ShoessBrushConfig shoes = new ShoessBrushConfig();


        [HideInInspector] public ColorInfo colorInfo = new ColorInfo();

        // Const Path  暂时无需进行编辑部位（头部、鞋子）
        [HideInInspector] public string DEFAULT_HEAD_PATH = "Assets/Art/Model/common_figure/player_head_new/_Head_Ja_Morant3_skin_slim.prefab";

        //Normals
        [HideInInspector] public string DEFAULT_HEAD_NORMAL_TEX_PATH = "Assets/Art/Model/common_figure/player_tex/player_head_new/Ja Morant3_Head_N.png";    
        [HideInInspector] public string DEFAULT_CLOTH_NORMAL_TEX_PATH = "Assets/Art/Model/common_figure/player_tex/player_body_new/Ja Morant3_Home_UI_N.png";
        [HideInInspector] public string DEFAULT_LIMBS_NORMAL_TEX_PATH = "Assets/Art/Model/common_figure/player_tex/player_limb_new/Ja Morant3_Skin_UI_N.png";
        [HideInInspector] public string DEFAULT_SHOES_NORMAL_TEX_PATH = "Assets/Art/Model/common_figure/player_tex/player_shoes_new/Ja Morant3_Shoes_UI_N.png";

        // Mats
        [HideInInspector] public string DEFAULT_SHOE_MATERIAL_PATH = "Assets/Art/Model/common_figure/player_mat/default_shoes_mat.mat";
        [HideInInspector] public string DEFAULT_LIMB_MATERIAL_PATH = "Assets/Art/Model/common_figure/player_mat/default_limb_mat.mat";
        [HideInInspector] public string DEFAULT_CLOTH_MATERIAL_PATH = "Assets/Art/Model/common_figure/player_mat/default_body_mat.mat";


        private string DEFAULT_SHOES_PATH = "Assets/Art/Model/common_figure/player_shoes_new/_shoes_01_skin_middle.FBX";
        private string SLIM_SHOES_PATH = "Assets/Art/Model/common_figure/player_shoes_new/_shoes_01_skin_slim.FBX";
        private string BIGGER_SHOES_PATH = "Assets/Art/Model/common_figure/player_shoes_new/_shoes_01_skin_bigger.FBX";


        //默认体型、Slim体型、Bigger体型【四肢】
        private string DEFAULT_LIMB_PATH = "Assets/Art/Model/common_figure/player_limb_new/_limb_slim_01_skin_middle.FBX";
        private string SLIM_LIMB_PATH = "Assets/Art/Model/common_figure/player_limb_new/_limb_slim_01_skin_slim.FBX";
        private string BIGGER_LIMB_PATH = "Assets/Art/Model/common_figure/player_limb_new/_limb_bigger_01_skin_bigger.FBX";
        //默认体型、Slim体型、Bigger体型【上身】
        private string DEFAULT_CLOTH_PATH = "Assets/Art/Model/common_figure/player_body_new/_body_01_skin_middle.FBX";
        private string SLIM_CLOTH_PATH = "Assets/Art/Model/common_figure/player_body_new/_body_01_skin_slim.FBX";
        private string BIGGER_CLOTH_PATH = "Assets/Art/Model/common_figure/player_body_new/_body_01_skin_bigger.FBX";
        //高模未嵌入，使用默认模型
        private string DEFAULT_HEAD_H_PATH = "Assets/Art/Model/player_head/_head_AlexLen_skin.FBX";
        private string DEFAULT_CLOTH_H_PATH = "Assets/Art/Model/player_body/_body_01_skin.FBX";
        private string DEFAULT_LIMB_H_PATH = "Assets/Art/Model/player_limb/_limb_01_skin.FBX";
        private string DEFAULT_SHOES_H_PATH = "Assets/Art/Model/common_figure/player_shoes_new/_shoes_01_skin_slim.FBX";

        //ModelLevel, Set by  enum 'BrushModelLevel'
        [HideInInspector] public const int MODEL_LEVEL = 3;
        [HideInInspector] public BrushModelFigure CurLevel = 0;

        #region Public Methods

        public void SetDefault()
        {
            heads.list = new HeadBrushItem[MODEL_LEVEL];
            cloth.list = new ClothBrushItem[MODEL_LEVEL];
            limbs.list = new LimbBrushItem[MODEL_LEVEL];
            shoes.list = new ShoesBrushItem[MODEL_LEVEL];

            for (int i = 0; i < MODEL_LEVEL; i++)
            {
                BrushModelLevel level = i == 0 ? BrushModelLevel.LOW : BrushModelLevel.HIGH;
                heads.list[i] = new HeadBrushItem
                    (
                    i == 0 ? DEFAULT_HEAD_PATH : DEFAULT_HEAD_H_PATH,
                    level
                    );

                cloth.list[i] = new ClothBrushItem
                    (
                     GetClothFigurePath(i),
                     level
                    );

                limbs.list[i] = new LimbBrushItem
                    (
                     GetLimbFigurePath(i),
                     level
                    );

                shoes.list[i] = new ShoesBrushItem
                    (
                     i == 0 ? DEFAULT_SHOES_PATH : DEFAULT_SHOES_H_PATH,
                     level
                    );
            }
        }

        #region Vertex Color Methods

        public bool SetDefaultColor(BaseBrush baseBrush)
        {
            if (colorInfo.defaultClothColors.Length < 1
                && colorInfo.defaultHeadColors.Length < 1
                && colorInfo.defaultLimbsColors.Length < 1
                && colorInfo.defaultShoesColors.Length < 1)
            {

                colorInfo.defaultHeadColors = baseBrush.headRender.sharedMesh.colors;
                colorInfo.defaultClothColors = baseBrush.clothRender.sharedMesh.colors;
                colorInfo.defaultLimbsColors = baseBrush.limbsRender.sharedMesh.colors;
                colorInfo.defaultShoesColors = baseBrush.shoesRender.sharedMesh.colors;

                return true;
            }

            return false;
        }

        public void SaveChangeColor(BaseBrush baseBrush)
        {
            if (colorInfo.headColors != baseBrush.headRender.sharedMesh.colors)
            {
                colorInfo.headColors = baseBrush.headRender.sharedMesh.colors;
            }

            if (colorInfo.clothColors != baseBrush.clothRender.sharedMesh.colors)
            {
                colorInfo.clothColors = baseBrush.clothRender.sharedMesh.colors;
            }

            if (colorInfo.limbsColors != baseBrush.limbsRender.sharedMesh.colors)
            {
                colorInfo.limbsColors = baseBrush.limbsRender.sharedMesh.colors;
            }

            if (colorInfo.shoesColors != baseBrush.shoesRender.sharedMesh.colors)
            {
                colorInfo.shoesColors = baseBrush.shoesRender.sharedMesh.colors;
            }
        }

        public void SetToConfigColor(BaseBrush baseBrush)
        {
            if (colorInfo.headColors.Length > 0) 
            {
                baseBrush.headRender.sharedMesh.colors = colorInfo.headColors;
            }

            if (colorInfo.clothColors.Length > 0)
            {
                baseBrush.clothRender.sharedMesh.colors = colorInfo.clothColors;
            }

            if (colorInfo.limbsColors.Length > 0)
            {
                baseBrush.limbsRender.sharedMesh.colors = colorInfo.limbsColors;
            }

            if (colorInfo.shoesColors.Length > 0)
            {
                baseBrush.shoesRender.sharedMesh.colors = colorInfo.shoesColors;
            }
        }

        public void RevertToDefaultColor(BaseBrush baseBrush)
        {
            baseBrush.headRender.sharedMesh.colors = colorInfo.defaultHeadColors;
            baseBrush.clothRender.sharedMesh.colors = colorInfo.defaultClothColors;
            baseBrush.limbsRender.sharedMesh.colors = colorInfo.defaultLimbsColors;
            baseBrush.shoesRender.sharedMesh.colors = colorInfo.defaultShoesColors;
        }

        #endregion

        public string GetClothFigurePath(int index)
        {
            string path = string.Empty;
            switch (index)
            {
                case 0:
                    path = DEFAULT_CLOTH_PATH;
                    break;
                case 1:
                    path = SLIM_CLOTH_PATH;
                    break;
                case 2:
                    path = BIGGER_CLOTH_PATH;
                    break;
                default:
                    path = DEFAULT_CLOTH_PATH;
                    break;
            }

            return path;
        }

        public string GetLimbFigurePath(int index)
        {
            string path = string.Empty;
            switch (index)
            {
                case 0:
                    path = DEFAULT_LIMB_PATH;
                    break;

                case 1:
                    path = SLIM_LIMB_PATH;
                    break;
                case 2:
                    path = BIGGER_LIMB_PATH;
                    break;
                default:
                    path = DEFAULT_LIMB_PATH;
                    break;
            }

            return path;
        }

        public void SetModelLevel(BrushModelLevel level)
        {
            int lv = (int)level;
            heads.curLevel = cloth.curLevel = limbs.curLevel = shoes.curLevel = level;

            for (int i = 0; i < MODEL_LEVEL; i++)
            {
                heads.selectLevel[i] = lv == i;
                cloth.selectLevel[i] = lv == i;
                limbs.selectLevel[i] = lv == i;
                shoes.selectLevel[i] = lv == i;
            }
        }

        public void SetModelFigure(BrushModelFigure figure)
        {
            int fg = (int)figure;
            heads.curFigure = cloth.curFigure = limbs.curFigure = shoes.curFigure = figure;
            for (int i = 0; i < MODEL_LEVEL; i++)
            {
                heads.selectLevel[i] = fg == i;
                cloth.selectLevel[i] = fg == i;
                limbs.selectLevel[i] = fg == i;
                shoes.selectLevel[i] = fg == i;
            }
        }

        public void LoadModelBody(ref GameObject headObj, ref GameObject clothObj, ref GameObject pantsObj, ref GameObject shoesObj)
        {
            headObj = heads.list[(int)CurLevel].oriObj;
            clothObj = cloth.list[(int)CurLevel].oriObj;
            pantsObj = limbs.list[(int)CurLevel].oriObj;
            shoesObj = shoes.list[(int)CurLevel].oriObj;
        }

        public void UpdateModel(HeadBrushConfig _heads, ClothBrushConfig _cloth, LimbsBrushConfig _limbs, ShoessBrushConfig _shoes)
        {
            heads = new HeadBrushConfig();
            heads.list = _heads.list;

            cloth = new ClothBrushConfig();
            cloth.list = _cloth.list;

            limbs = new LimbsBrushConfig();
            limbs.list = _limbs.list;

            shoes = new ShoessBrushConfig();
            shoes.list = _shoes.list;


            ResetConfig(heads, _heads);
            ResetConfig(cloth, _cloth);
            ResetConfig(limbs, _limbs);
            ResetConfig(shoes, _shoes);
        }

        public void UpdateVertexColor(UvBrushConfig cfg)
        {
            colorInfo.headColors = cfg.colorInfo.clothColors;
            colorInfo.clothColors = cfg.colorInfo.clothColors;
            colorInfo.limbsColors = cfg.colorInfo.limbsColors;
            colorInfo.shoesColors = cfg.colorInfo.shoesColors;
            colorInfo.combineColors = cfg.colorInfo.combineColors;

            //set default colors
        }

        public void ResetConfig(BaseBrushConfig _ori, BaseBrushConfig _tar)
        {
            _ori.selectLevel = _tar.selectLevel;
            _ori.curLevel = _tar.curLevel;
            _ori.curFigure = _tar.curFigure;
            _ori.curMat = _tar.curMat;
        }

        public void ResetList(BaseBrushItem[] _ori, BaseBrushItem[] _tar)
        {
            for (int i = 0; i < _ori.Length; i++)
            {
                _ori[i] = _tar[i];
            }
        }

        #endregion
    }

    [Serializable]
    public class ColorInfo
    {
        public Color[] headColors = new Color[0];
        public Color[] clothColors = new Color[0];
        public Color[] limbsColors = new Color[0];
        public Color[] shoesColors = new Color[0];

        public Color[] defaultHeadColors = new Color[0];
        public Color[] defaultClothColors = new Color[0];
        public Color[] defaultLimbsColors = new Color[0];
        public Color[] defaultShoesColors = new Color[0];

        public Color[] combineColors = new Color[0];
    }

    public class BaseBrushConfig
    {
        public bool[] selectLevel;
        public BrushModelLevel curLevel;
        public BrushModelFigure curFigure;
        public Material curMat;

        public BaseBrushConfig()
        {
            curLevel = BrushModelLevel.LOW;
            curFigure = BrushModelFigure.DEFAULT;
            selectLevel = new bool[UvBrushConfig.MODEL_LEVEL] { true, false, false };
        }

        public void LoadMat(string path)
        {
            curMat = AssetDatabase.LoadAssetAtPath(path, typeof(Material)) as Material;
        }
    }

    public class HeadBrushConfig : BaseBrushConfig
    {
        public Texture2D headNormal;
        public HeadBrushItem[] list = new HeadBrushItem[0];

        public void LoadHeadNormal(string path)
        {
            headNormal = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
        }
    }

    public class ClothBrushConfig : BaseBrushConfig
    {
        public Texture2D clothNormal;
        public ClothBrushItem[] list = new ClothBrushItem[0];

        public void LoadClothNornal(string path)
        {
            clothNormal = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
        }
    }

    public class LimbsBrushConfig : BaseBrushConfig
    {
        public Texture2D limbNormal;
        public LimbBrushItem[] list = new LimbBrushItem[0];

        public void LoadLimbNornal(string path)
        {
            limbNormal = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
        }
    }

    public class ShoessBrushConfig : BaseBrushConfig
    {
        public Texture2D shoesNormal;
        public ShoesBrushItem[] list = new ShoesBrushItem[0];

        public void LoadShoesNornal(string path)
        {
            shoesNormal = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
        }
    }


    [Serializable]
    public class BaseBrushItem
    {
        public string loadPath;
        public GameObject oriObj;
        public BrushModelLevel level;
        public BrushModelType modelType;

        public BaseBrushItem(string _loadPath, BrushModelLevel _level)
        {
            loadPath = _loadPath;
            level = _level;
            oriObj = AssetDatabase.LoadAssetAtPath(loadPath, typeof(GameObject)) as GameObject;
        }
    }

    public class HeadBrushItem : BaseBrushItem
    {
        public HeadBrushItem(string _loadPath, BrushModelLevel _level) : base(_loadPath, _level)
        {
            modelType = BrushModelType.HEAD;
        }
    }

    public class ClothBrushItem : BaseBrushItem
    {
        public ClothBrushItem(string _loadPath, BrushModelLevel _level) : base(_loadPath, _level)
        {
            modelType = BrushModelType.CLOTH;
        }
    }

    public class LimbBrushItem : BaseBrushItem
    {
      
        public LimbBrushItem(string _loadPath, BrushModelLevel _level) : base(_loadPath, _level)
        {
            modelType = BrushModelType.LIMBS;
        }


    }

    public class ShoesBrushItem : BaseBrushItem
    {
      

        public ShoesBrushItem(string _loadPath, BrushModelLevel _level) : base(_loadPath, _level)
        {
            modelType = BrushModelType.SHOES;
        }


    }
}