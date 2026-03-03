using PixelRay.Core;
using PixelRay.Mathematics;
using PixelRay.SceneObjects;

namespace PixelRay.Geometry;

/// <summary>
/// A sphere defined by center and radius.
/// </summary>
public class Sphere(Vec3 center, double radius, ColorRGB color) : IHittable
{
    public Vec3 Center = center;
    public double Radius = radius;
    public ColorRGB Color = color;

    public bool Hit(Ray ray, double tMin, double tMax, out HitRecord hit)
    {
        hit = default;

        Vec3 co = ray.Origin - Center;
        // coefficients of a quadratic equation produced by ray intersecting the sphere surface.
        // normally double a = Vec3.Dot(ray.Direction, ray.Direction), but ray.D is normalized so it's always 1.
        double b = 2 * Vec3.Dot(ray.Direction, co);
        double c = Vec3.Dot(co, co) - Radius * Radius;

        double discriminant = b * b - 4 * c; // remove a from here (and also from root calculations)
        if (discriminant < 0)
            return false;

        double sqrtDiscriminant = Math.Sqrt(discriminant);
        double t = (-b - sqrtDiscriminant) / 2;

        if (t <= tMin || t >= tMax)
        {
            t = (-b + sqrtDiscriminant) / 2;
            if (t <= tMin || t >= tMax)
                return false;
        }

        hit.Point = ray.At(t);
        hit.Normal = (hit.Point - Center).Unit();
        hit.T = t;
        hit.Color = Color;

        return true;
    }
}