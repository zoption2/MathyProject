using System;
using UnityEngine;

public static class ColorConverterExtensions 
{
    /// <summary>
    /// Convert from Color to HEX string in #RRGGBB format
    /// </summary>
    public static string ToHexString(this Color c) => $"#{(byte)c.r:X2}{(byte)c.g:X2}{(byte)c.b:X2}";

    /// <summary>
    /// Convert from Color to HEX string in #RRGGBBAA format
    /// </summary>
    public static string ToHexAString(this Color c) => $"#{(byte)c.r:X2}{(byte)c.g:X2}{(byte)c.b:X2}{(byte)c.a:X2}";

    /// <summary>
    /// Convert from Color to string in RGBA(R, G, B) format
    /// </summary>
    public static string ToRGBString(this Color c) => $"RGB({c.r}, {c.g}, {c.b})";

    /// <summary>
    /// Convert from Color to string in RGBA(R, G, B, A) format
    /// </summary>
    public static string ToRGBAString(this Color c) => $"RGBA({c.r}, {c.g}, {c.b}, { c.a/(double)Byte.MaxValue : N2})";

    /// <summary>
    /// Convert from Hex(in #RRGGBB format) to Color
    /// </summary>
    /// <param name="colorHex"></param>
    public static Color FromHexString(this Color c, string colorHex) 
    {
        colorHex = colorHex.Replace("#", "");
        byte r = Convert.ToByte(colorHex.Substring(0, 2), 16);
        byte g = Convert.ToByte(colorHex.Substring(2, 2), 16);
        byte b = Convert.ToByte(colorHex.Substring(4, 2), 16);
        return new Color(r,g,b);
    }

    /// <summary>
    /// Convert from Hex(in #RRGGBBAA format) to Color with alpha channel 
    /// </summary>
    /// <param name="colorHex">Color hex</param>
    public static Color FromHexAString(this Color c, string colorHex)
    {
        colorHex = colorHex.Replace("#", "");
        byte r = Convert.ToByte(colorHex.Substring(0, 2), 16);
        byte g = Convert.ToByte(colorHex.Substring(2, 2), 16);
        byte b = Convert.ToByte(colorHex.Substring(4, 2), 16);
        byte a = Convert.ToByte(colorHex.Substring(6, 2), 16);
        return new Color(r, g, b, a);
    }
}
