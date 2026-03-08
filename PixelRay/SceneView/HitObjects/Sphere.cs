using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;

namespace PixelRay.SceneView.HitObjects;

/// <summary>
/// Origin-centered unit sphere.
/// </summary>
public class Sphere(ColorRGB color) : IHittable
{
    public ColorRGB Color = color;

    public bool Hit(Ray ray, double tMin, double tMax, out HitRecord hit)
    {
        hit = default;

        double a = Vec3.Dot(ray.Direction, ray.Direction);
        double b = 2 * Vec3.Dot(ray.Direction, ray.Origin);
        double c = ray.Origin.NormSquared() - 1;

        double discriminant = b * b - 4 * a * c;
        if (discriminant < -Const.HitDiscriminant)
            return false;

        double sqrtD = Math.Sqrt(Math.Max(discriminant, 0.0));

        double t1 = (-b - sqrtD) / (2 * a);
        double t2 = (-b + sqrtD) / (2 * a);

        double t = double.PositiveInfinity;

        if (t1 >= tMin && t1 <= tMax)
            t = t1;

        if (t2 >= tMin && t2 <= tMax && t2 < t)
            t = t2;

        if (t == double.PositiveInfinity)
            return false;

        hit.Point = ray.At(t);
        Vec3 normal = hit.Point.Unit();
        hit.SetFaceNormal(ray, normal);

        hit.T = t;
        hit.Color = Color;
        hit.Object = this;

        return true;
    }
}