using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;

namespace PixelRay.SceneView.Lighting;

/// <summary>
/// Spotlight, uses full angles
/// </summary>
public class SpotLight(
    Vec3 position,
    Vec3 direction,
    double outerAngle,
    double innerAngle,
    ColorRGB color,
    double intensity = 1.0,
    double lightRadius = 0,
    int shadowBands = 0
) : PointLight(position, color, intensity, lightRadius, shadowBands)
{
    public Vec3 Direction { get; set; } = direction.Unit();
    public double OuterAngle { get; set; } = outerAngle;
    public double InnerAngle { get; set; } = innerAngle <= outerAngle ? innerAngle : outerAngle;

    public override LightContribution Shade(Scene scene, in HitRecord hit)
    {
        Vec3 toLight = Position - hit.Point;
        double distance = toLight.Length();
        Vec3 dir = toLight / distance;

        double cosAngle = Vec3.Dot(-dir, Direction); // invert to make both point into same direction

        double outerCos = Math.Cos(OuterAngle * 0.5 * Math.PI / 180); // halve the input angle

        // cosine is decreasing on [0, PI/2]: angle larger than outer = point is outside cone's vision
        if (cosAngle < outerCos)
            return new LightContribution(Shading: 0.0);

        double innerCos = Math.Cos(InnerAngle * 0.5 * Math.PI / 180);
        double coneFactor;

        if (cosAngle > innerCos)
            // smaller than inner = inside the cone means max brightness
            coneFactor = 1.0;
        else
            // linear interpolation in cosine space (non-linear curve)so falloff is slower.
            // Here inner gives 1, outer gives 0
            coneFactor = (cosAngle - outerCos) / (innerCos - outerCos);

        double shadow = Shadows.SampleShadowPointPreset(scene, hit.Point, Position, LightRadius, ShadowBands);
        if (shadow <= 0)
            return new LightContribution(Shading: 0.0);

        double NdotL = Math.Max(0, Vec3.Dot(hit.Normal, dir));
        if (NdotL <= 0)
            return new LightContribution(Shading: 0.0);

        double attenuation = 1.0 / (0.01 + distance * distance);

        return new(
            Shading: NdotL * shadow * coneFactor,
            Attenuation: attenuation
        );
    }
}