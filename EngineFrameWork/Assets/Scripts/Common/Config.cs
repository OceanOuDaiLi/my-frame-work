
public class Config
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

    #region Scene Name
    public const string Scene_Launch = "Launch";
    public const string Scene_Main = "Main";
    #endregion

    public const string REGEX_EMAIL = "^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{2,5})\\s*$";

}
