using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;

namespace PixelRay.SceneView.Lighting;

/// <summary>
/// A directional light ray
/// Light direction is from hit point TO light
/// </summary>
public class DirectionalLight(Vec3 direction, ColorRGB color, double intensity = 1.0) : ILight
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public Vec3 Direction { get; set; } = direction.Unit();
    public ColorRGB Color { get; set; } = color;
    public double Intensity { get; set; } = intensity;

    public LightContribution Shade(Scene scene, in HitRecord hit)
    {
        Vec3 dir = Direction.Unit();

        double shadow = Shadows.SampleShadowDirectional(scene, hit.Point, hit.Normal, dir);
        double NdotL = Math.Max(0, Vec3.Dot(hit.Normal, dir));

        return new(Shading: NdotL * shadow);
    }
}