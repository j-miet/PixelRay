using System.Drawing;

namespace PixelRay.Mathematics;

public readonly struct ColorRGB(double red, double green, double blue)
{
    public readonly double R = red;
    public readonly double G = green;
    public readonly double B = blue;

    /// <summary>
    /// Clamps rgb values of this color into interval [0, 1]
    /// </summary>
    /// <returns></returns>
    public ColorRGB Clamp()
    {
        return new ColorRGB(
            Math.Max(0, Math.Min(1, R)),
            Math.Max(0, Math.Min(1, G)),
            Math.Max(0, Math.Min(1, B))
        );
    }

    /// <summary>
    /// Perform quantization to rgb color value
    /// This process splits interval [0, 1] into evenly spaces ranges where each range maps into a single color.
    /// End result makes output image have more limited color palette thus making images more pixelated.
    /// </summary>
    /// <param name="levels">Amount of levels</param>
    /// <returns>Quantized color value</returns>
    public ColorRGB Quantize(int levels)
    {
        return new ColorRGB(
            Math.Floor(R * levels) / levels,
            Math.Floor(G * levels) / levels,
            Math.Floor(B * levels) / levels
        );
    }

    public static ColorRGB operator *(double scalar, ColorRGB color)
    {
        return new ColorRGB(scalar * color.R, scalar * color.G, scalar * color.B);
    }

    public static ColorRGB operator *(ColorRGB color, double scalar)
    {
        return scalar * color;
    }

    public static ColorRGB operator +(ColorRGB color1, ColorRGB color2)
    {
        return new ColorRGB(color1.R + color2.R, color1.G + color2.G, color1.B + color2.B);
    }

    public static ColorRGB operator *(ColorRGB color1, ColorRGB color2)
    {
        return new ColorRGB(color1.R * color2.R, color1.G * color2.G, color1.B * color2.B);
    }
}