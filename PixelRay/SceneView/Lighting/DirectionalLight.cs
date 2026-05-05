using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;

namespace PixelRay.SceneView.Lighting;

/// <summary>
/// A directional light ray
/// Light direction is from hit point TO light
/// </summary>
public class DirectionalLight(Vec3 direction, ColorRGB color, double intensity = 1.0) : ILight
{
    public Vec3 Direction { get; } = direction.Unit();
    public ColorRGB Color { get; } = color;
    public double Intensity { get; } = intensity;

    public double Shade(Scene scene, in HitRecord hit)
    {
        Vec3 dir = Direction.Normalize();

        double shadow = Shadows.SampleShadowDirectional(scene, hit.Point, dir);
        double NdotL = Math.Max(0, Vec3.Dot(hit.Normal, dir));

        return NdotL * shadow * Intensity;
    }
}