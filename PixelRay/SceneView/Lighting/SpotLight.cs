using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;

namespace PixelRay.SceneView.Lighting;

public class SpotLight(
    Vec3 position,
    Vec3 direction,
    double angle,
    double innerAngle,
    ColorRGB color,
    double intensity,
    double radius = 0,
    double attenuation = 1.0
) : PointLight(position, color, intensity, radius, attenuation)
{
    public Vec3 Direction { get; } = direction.Unit();
    public double Angle { get; } = angle;
    public double InnerAngle { get; } = innerAngle;

    public new double Shade(Scene scene, in HitRecord hit)
    {
        Vec3 toLight = Position - hit.Point;
        double distance = toLight.Length();
        Vec3 dir = toLight / distance;

        double cosAngle = Vec3.Dot(-dir, Direction); // invert to make both point into same direction

        // convert outer angle to radians, also divide by 2 for half-angle
        double outer = Math.Cos(Angle * 0.5 * Math.PI / 180);

        // cosine is decreasing on [0, PI/2]: angle larger than outer = point is outside cone's vision
        if (cosAngle < outer)
            return 0.0;

        double coneFactor;
        // inner angle to radians + halving
        double inner = Math.Cos(InnerAngle * 0.5 * Math.PI / 180);

        if (cosAngle > inner)
            // smaller than inner = inside the cone means max brightness
            coneFactor = 1;
        else
            // linear interpolation: inner gives max, outer gives min
            coneFactor = (cosAngle - outer) / (inner - outer);

        double shadow = Shadows.SampleShadowPoint(scene, hit.Point, Position, Radius);
        if (shadow <= 0)
            return 0;

        double NdotL = Math.Max(0, Vec3.Dot(hit.Normal, dir));
        if (NdotL <= 0)
            return 0;

        double attenuation = 1.0 / (Attenuation * distance * distance);

        return NdotL * shadow * coneFactor * attenuation;
    }
}