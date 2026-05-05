using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;

namespace PixelRay.SceneView.Lighting;

/// <summary>
/// Light source which emits light evenly to all directions with specified intensity
/// </summary>
/// <param name="position">Light source/center</param>
/// <param name="color"></param>
/// <param name="intensity"></param>
/// <param name="radius"></param>
/// <param name="attenuation"></param>
public class PointLight(
    Vec3 position,
    ColorRGB color,
    double intensity = 1.0,
    double radius = 0.0,
    double attenuation = 1.0
) : ILight
{
    public Vec3 Position { get; } = position;
    public ColorRGB Color { get; } = color;
    public double Radius { get; } = radius;
    public double Intensity { get; } = intensity;
    public double Attenuation { get; } = attenuation;

    public double Shade(Scene scene, in HitRecord hit)
    {
        Vec3 toLight = Position - hit.Point;
        double distance = toLight.Length();
        Vec3 dir = toLight / distance;

        double shadow = Shadows.SampleShadowPoint(scene, hit.Point, Position, Radius);
        double NdotL = Math.Max(0, Vec3.Dot(hit.Normal, dir));
        double attenuation = Intensity / (Attenuation * distance * distance);

        return NdotL * shadow * attenuation;
    }
}