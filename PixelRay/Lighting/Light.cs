using PixelRay.Mathematics;

namespace PixelRay.Lighting;

/// <summary>
/// Abstraction of light objects
/// </summary>
/// <param name="color"></param>
public abstract class Light(ColorRGB color)
{
    public ColorRGB Color { get; } = color;
}