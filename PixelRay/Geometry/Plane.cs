using PixelRay.Core;
using PixelRay.Mathematics;
using PixelRay.SceneObjects;

namespace PixelRay.Geometry;

/// <summary>
/// A plane in 3d space
/// </summary>
/// <param name="point"></param>
/// <param name="normal"></param>
/// <param name="color"></param>
public class Plane(Vec3 point, Vec3 normal, ColorRGB color) : IHittable
{
    public Vec3 Point = point;
    public Vec3 Normal = normal.Unit();
    public ColorRGB Color = color;

    public bool Hit(Ray ray, double tMin, double tMax, out HitRecord hit)
    {
        hit = default;

        // plane is defined by all points x such that Dot(n, x - Point) = 0
        // Substituting ray R(t) into x yields Dot(n, O + t*D - Point) = 0 which can be simplified into
        // Dot(Point - O, n) / Dot(n, D)
        // Thus denominator of this term is Dot(n, d), which also tells the angle between ray and plane.
        double denominator = Vec3.Dot(Normal, ray.Direction);
        if (Math.Abs(denominator) < 0.0001) // normal and direction are perpendicular
            return false;

        double root = Vec3.Dot(Point - ray.Origin, Normal) / denominator;

        if (root <= tMin || root >= tMax)
            return false;

        hit.Point = ray.At(root);
        hit.Normal = Normal;
        hit.T = root;
        hit.Color = Color;

        return true;
    }
}