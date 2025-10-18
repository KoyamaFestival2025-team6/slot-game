public enum ZodiacSign
{
    // 春
    Aries,        // 牡羊座
    Taurus,       // 牡牛座
    Gemini,       // 双子座
    
    // 夏
    Cancer,       // 蟹座
    Leo,          // 獅子座
    Virgo,        // 乙女座
    
    // 秋
    Libra,        // 天秤座
    Scorpio,      // 蠍座
    Sagittarius,  // 射手座
    
    // 冬
    Capricorn,    // 山羊座
    Aquarius,     // 水瓶座
    Pisces        // 魚座
}

public static class ZodiacSignExtensions
{
    public const int TotalSigns = 12;
    
    /// <summary>
    /// 次の星座を取得
    /// </summary>
    public static ZodiacSign Next(this ZodiacSign sign)
    {
        return (ZodiacSign)(((int)sign + 1) % TotalSigns);
    }
    
    /// <summary>
    /// 前の星座を取得
    /// </summary>
    public static ZodiacSign Previous(this ZodiacSign sign)
    {
        return (ZodiacSign)(((int)sign - 1 + TotalSigns) % TotalSigns);
    }
    
    /// <summary>
    /// 日本語名を取得
    /// </summary>
    public static string GetJapaneseName(this ZodiacSign sign)
    {
        return sign switch
        {
            ZodiacSign.Aries => "牡羊座",
            ZodiacSign.Taurus => "牡牛座",
            ZodiacSign.Gemini => "双子座",
            ZodiacSign.Cancer => "蟹座",
            ZodiacSign.Leo => "獅子座",
            ZodiacSign.Virgo => "乙女座",
            ZodiacSign.Libra => "天秤座",
            ZodiacSign.Scorpio => "蠍座",
            ZodiacSign.Sagittarius => "射手座",
            ZodiacSign.Capricorn => "山羊座",
            ZodiacSign.Aquarius => "水瓶座",
            ZodiacSign.Pisces => "魚座",
            _ => "不明"
        };
    }
}