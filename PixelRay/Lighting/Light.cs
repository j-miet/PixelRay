using PixelRay.Mathematics;

namespace PixelRay.Lighting;

/// <summary>
/// Abstraction of light objects
/// </summary>
public abstract class Light(ColorRGB color)
{
    public ColorRGB Color { get; } = color;
}