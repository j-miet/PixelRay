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
}