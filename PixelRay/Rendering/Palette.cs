using PixelRay.Mathematics;

namespace PixelRay.Rendering;

public class Palette(ColorRGB[] colors)
{
    public ColorRGB[] Colors { get; } = colors;

    public ColorRGB Map(ColorRGB input)
    {
        double closestDistance = double.MaxValue;
        ColorRGB closestColor = Colors[0];

        foreach (ColorRGB c in Colors)
        {
            double distR = input.R - c.R;
            double distG = input.G - c.G;
            double distB = input.B - c.B;

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