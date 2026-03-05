using PixelRay.Core.Mathematics;

namespace PixelRay.SceneView.Lighting;

/// <summary>
/// Abstraction of light objects
/// </summary>
public abstract class Light(ColorRGB color)
{
    public ColorRGB Color { get; } = color;
}