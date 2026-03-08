using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;

namespace PixelRay.SceneView.HitObjects.Transformed;

/// <summary>
/// A sphere defined by center and radius.
/// </summary>
public class SphereT(Vec3 center, double radius, ColorRGB color) : IHittable
{
    public Vec3 Center = center;
    public double Radius = radius;
    public ColorRGB Color = color;

    public bool Hit(Ray ray, double tMin, double tMax, out HitRecord hit)
    {
        hit = default;

        Vec3 co = ray.Origin - Center;
        // coefficients of a quadratic equation produced by ray intersecting the sphere surface.
        // normally double a = Vec3.Dot(ray.Direction, ray.Direction), but ray.D is normalized so a is always 1.
        double b = 2 * Vec3.Dot(ray.Direction, co);
        double c = Vec3.Dot(co, co) - Radius * Radius;

        double discriminant = b * b - 4 * c; // remove a from here (and also from root calculations)
        if (discriminant < -Const.HitDiscriminant)
            return false;

        double sqrtD = Math.Sqrt(Math.Max(discriminant, 0.0));
        double t1 = (-b - sqrtD) / 2;
        double t2 = (-b + sqrtD) / 2;

        double t = double.PositiveInfinity;

        if (t1 >= tMin && t1 <= tMax)
            t = t1;
        if (t2 >= tMin && t2 <= tMax && t2 < t)
            t = t2;
        if (t == double.PositiveInfinity)
            return false;

        hit.Point = ray.At(t);
        Vec3 normal = (hit.Point - Center).Unit();
        hit.SetFaceNormal(ray, normal);
        hit.T = t;
        hit.Color = Color;
        hit.Object = this;

        return true;
    }
}