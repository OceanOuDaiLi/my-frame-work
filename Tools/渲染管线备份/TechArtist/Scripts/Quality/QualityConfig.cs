namespace TechArtist.QualitySetting
{
    /// <summary>
    /// 材质品质
    /// </summary>
    public enum EMaterialQuality
    {
        QUALITY_HIGH = 0,
        QUALITY_MEDIUM = 1,
        QUALITY_LOW = 2,
        QUALITY_LOWEST = 3
    }

    /// <summary>
    /// 机型品质
    /// </summary>
    public enum ModelQuality
    {
        QUALITY_HIGH = 0,
        QUALITY_MEDIUM = 1,
        QUALITY_LOW = 2,
        QUALITY_LOWEST = 3
    }

    public class ShaderQualityKeywords
    {
        public const string QUALITY_HIGH = "QUALITY_HIGH";
        public const string QUALITY_MEDIUM = "QUALITY_MEDIUM";
        public const string QUALITY_LOW = "QUALITY_LOW";
        public const string QUALITY_LOWEST = "QUALITY_LOWEST";
    }
}