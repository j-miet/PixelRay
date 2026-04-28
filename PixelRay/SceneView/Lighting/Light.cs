using PixelRay.Core.Mathematics;

namespace PixelRay.SceneView.Lighting;

/// <summary>
/// Abstraction of light objects. <br/>
/// Lights should follow the principle of hit point -> light e.g. define any light
/// direction vectors from hit point to light source, invert to move from light source to hit point.
/// </summary>
public abstract class Light(ColorRGB color)
{
    public ColorRGB Color { get; } = color;
}