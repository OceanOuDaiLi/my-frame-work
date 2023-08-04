
public class GameConfig
{
    #region Global Pool Name
    public const string PROP_POOL_NAME = "PropPool";
    public const string EFFECT_POOL_NAME = "EffectPool";
    public const string UI_POOL_NAME = "UIPool";
    #endregion

    /// <summary>
    /// 帧率
    /// </summary>
    public enum TargetFrameRate : byte
    {
        FPS30,
        FPS60
    }

    public const string REGEX_EMAIL = "^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{2,5})\\s*$";

    #region Sorting Order inLayer

    // UI Canvas Sorting Order
    public const int UICanvas_SortOrder = 3;
    public const int FightCanvas_SortOrder = 4;
    public const int CommonUICanvas_SortOrder = 5;

    // 3D 
    public const int MapSubBg_OrderInLayer = 2;
    public const int NpcCharacter_OrderInLayer = 3;
    public const int UserCharacter_OrderInLayer = 4;
    public const int MapMask_OrderInLayer = 6;

    public const int FightTeamCharacter_Front_OrderInLayer = 5;
    public const int FightTeamCharacter_Back_OrderInLayer = 6;

    public const int FightEnemyCharacter_Front_OrderInLayer = 6;
    public const int FightEnemyCharacter_Back_OrderInLayer = 5;

    #endregion
}
