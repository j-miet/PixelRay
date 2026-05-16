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
    // these are for Lua scripting
    public int Id { get; set; }
    public string? Name { get; set; }

    public ColorRGB Color { get; set; }
    public double Intensity { get; set; }

    public LightContribution Shade(
        Scene scene,
        in HitRecord hit
    );
}