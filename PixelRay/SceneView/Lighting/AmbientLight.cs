using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;

namespace PixelRay.SceneView.Lighting;

/// <summary>
/// Ambient light
/// </summary>
public class AmbientLight(ColorRGB color, double intensity = 1.0) : ILight
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public ColorRGB Color { get; set; } = color;
    public double Intensity { get; set; } = intensity;

    public LightContribution Shade(Scene scene, in HitRecord hit)
    {
        return new(Shading: 1.0);
    }
}