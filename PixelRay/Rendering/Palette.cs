using PixelRay.Core.Mathematics;

namespace PixelRay.Rendering;

/// <summary>
/// Simple color palette generator
/// </summary>
public class Palette(ColorRGB[] colors)
{
    public ColorRGB[] Colors { get; } = colors;

    public ColorRGB Map(ColorRGB inputColor)
    {
        double closestDistance = double.MaxValue;
        ColorRGB closestColor = Colors[0];

        foreach (ColorRGB c in Colors)
        {
            double distR = inputColor.R - c.R;
            double distG = inputColor.G - c.G;
            double distB = inputColor.B - c.B;

            double distSquared = distR * distR + distG * distG + distB * distB;

            if (distSquared < closestDistance)
            {
                closestDistance = distSquared;
                closestColor = c;
            }
        }

        return closestColor;
    }

    /// <summary>
    /// Apply ordered dithering to a pixel at (x, y)
    /// </summary>
    public static ColorRGB MapDithered(ColorRGB inputColor, int x, int y, double strength = 0.1, int ditherDim = 4)
    {
        double threshold = Dither.GetThreshold(x, y, ditherDim) - 0.5; // subtract 0.5 for even offset distribution

        ColorRGB dithered = new(
            inputColor.R + threshold * strength,
            inputColor.G + threshold * strength,
            inputColor.B + threshold * strength
        );

        return dithered;
    }
}