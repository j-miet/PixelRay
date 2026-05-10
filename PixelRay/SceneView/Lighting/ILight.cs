using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;

namespace PixelRay.SceneView.Lighting;

/// <summary>
/// Abstraction of light objects. <br/>
/// Lights should follow the principle of hit point -> light e.g. define any light
/// direction vectors from hit point to light source, invert to move from light source to hit point.
/// </summary>
public interface ILight
{
    public ColorRGB Color { get; }
    public double Intensity { get; }

    public LightContribution Shade(
        Scene scene,
        in HitRecord hit
    );
}