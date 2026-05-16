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
    public int Id { get; set; }
    public string? Name { get; set; }

    public Vec3 Position { get; set; } = position;
    public ColorRGB Color { get; set; } = color;
    public double LightRadius { get; set; } = lightRadius;
    public double Intensity { get; set; } = intensity;
    public int ShadowBands { get; set; } = shadowBands;

    public virtual LightContribution Shade(Scene scene, in HitRecord hit)
    {
        Vec3 toLight = Position - hit.Point;
        double distance = toLight.Length();
        Vec3 dir = toLight / distance;

        double shadow = Shadows.SampleShadowPointPreset(scene, hit.Point, Position, LightRadius, ShadowBands);
        double NdotL = Math.Max(0, Vec3.Dot(hit.Normal, dir));
        double attenuation = 1.0 / (0.01 + distance * distance);

        return new(
            Shading: NdotL * shadow,
            Attenuation: attenuation
        );
    }
}