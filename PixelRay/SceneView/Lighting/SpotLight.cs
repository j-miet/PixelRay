using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;

namespace PixelRay.SceneView.Lighting;

/// <summary>
/// Spotlight, uses half-angles (e.g. 20 degrees = 40 degree full cone). Angle is limited to 90 degrees, otherwise
/// visuals get distorted/unintuitive.
/// </summary>
public class SpotLight(
    Vec3 position,
    Vec3 direction,
    double angle,
    double innerAngle,
    ColorRGB color,
    double intensity = 1.0,
    double radius = 0
) : PointLight(position, color, intensity, radius)
{
    public Vec3 Direction { get; } = direction.Unit();
    public double Angle { get; } = angle;
    public double InnerAngle { get; } = innerAngle <= angle ? innerAngle : angle;

    public override double Shade(Scene scene, in HitRecord hit)
    {
        Vec3 toLight = Position - hit.Point;
        double distance = toLight.Length();
        Vec3 dir = toLight / distance;

        double cosAngle = Vec3.Dot(-dir, Direction); // invert to make both point into same direction

        double outerHalfAngle = Math.Cos(Angle * Math.PI / 180);
        outerHalfAngle = Math.Min(outerHalfAngle, 89.99);

        // cosine is decreasing on [0, PI/2]: angle larger than outer = point is outside cone's vision
        if (cosAngle < outerHalfAngle)
            return 0.0;

        double innerHalfAngle = Math.Cos(InnerAngle * Math.PI / 180);
        innerHalfAngle = Math.Min(innerHalfAngle, outerHalfAngle);
        double coneFactor;

        if (cosAngle > innerHalfAngle)
            // smaller than inner = inside the cone means max brightness
            coneFactor = 1.0;
        else
            // linear interpolation in cosine space (non-linear curve)so falloff is slower.
            // Here inner gives 1, outer gives 0
            coneFactor = (cosAngle - outerHalfAngle) / (innerHalfAngle - outerHalfAngle);

        double shadow = Shadows.SampleShadowPoint(scene, hit.Point, Position, Radius);
        if (shadow <= 0)
            return 0.0;

        double NdotL = Math.Max(0, Vec3.Dot(hit.Normal, dir));
        if (NdotL <= 0)
            return 0.0;

        double attenuation = Intensity / (distance * distance);

        return NdotL * shadow * coneFactor * attenuation;
    }
}