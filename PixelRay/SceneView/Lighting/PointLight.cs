using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;

namespace PixelRay.SceneView.Lighting;

/// <summary>
/// Light source which emits light evenly to all directions with specified intensity
/// </summary>
public class PointLight(
    Vec3 position,
    ColorRGB color,
    double intensity = 1.0,
    double lightRadius = 0.0,
    int shadowBands = 0
) : ILight
{
    public Vec3 Position { get; } = position;
    public ColorRGB Color { get; } = color;
    public double LightRadius { get; } = lightRadius;
    public double Intensity { get; } = intensity;
    public int ShadowBands { get; } = shadowBands;

    public virtual double Shade(Scene scene, in HitRecord hit)
    {
        Vec3 toLight = Position - hit.Point;
        double distance = toLight.Length();
        Vec3 dir = toLight / distance;

        double shadow = Shadows.SampleShadowPointPreset(scene, hit.Point, Position, LightRadius, ShadowBands);
        double NdotL = Math.Max(0, Vec3.Dot(hit.Normal, dir));
        double attenuation = Intensity / (distance * distance);

        return NdotL * shadow * attenuation;
    }
}