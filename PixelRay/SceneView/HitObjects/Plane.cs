using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;

namespace PixelRay.SceneView.HitObjects;

/// <summary>
/// Plane defined by a point and a normal.
/// </summary>
public class Plane(Vec3 point, Vec3 normal, ColorRGB color) : IHittable
{
    public Vec3 Point = point;
    public Vec3 Normal = normal.Unit();
    public ColorRGB Color = color;

    public bool Hit(Ray ray, double tMin, double tMax, out HitRecord hit)
    {
        hit = default;
        double epsilon = 1e-4;

        // plane is defined by all points x such that Dot(n, x - Point) = 0
        // Substituting ray R(t) into x yields Dot(n, O + t*D - Point) = 0 which can be simplified into
        // Dot(n, Point - O) / Dot(n, D)
        // denominator term Dot(n, d) also tells the angle between ray and plane.
        double NDotR = Vec3.Dot(Normal, ray.Direction);
        if (Math.Abs(NDotR) < epsilon) // check if normal and direction are perpendicular
            return false;

        double t = Vec3.Dot(Normal, Point - ray.Origin) / NDotR;

        if (t <= tMin || t >= tMax)
            return false;

        hit.Point = ray.At(t);
        hit.Normal = Vec3.Dot(ray.Direction, Normal) > 0 ? -Normal : Normal;
        hit.T = t;
        hit.Color = Color;

        return true;
    }
}