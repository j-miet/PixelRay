using PixelRay.Core;
using PixelRay.Mathematics;
using PixelRay.SceneObjects;

namespace PixelRay.Geometry;

/// <summary>
/// Planar disc defined by its center point, radius and normal.
/// </summary>
public class Disc(Vec3 center, Vec3 normal, double radius, ColorRGB color) : IHittable
{
    public Vec3 Center = center;
    public double Radius = radius;
    public Vec3 Normal = normal.Unit();
    public ColorRGB Color = color;

    public bool Hit(Ray ray, double tMin, double tMax, out HitRecord hit)
    {
        hit = default;
        double epsilon = 1e-4;

        // same math can be used here as with planes
        // first check perpendicularity, then calculate plane intersection. Finally check if distance between center
        // and plane intersection point is <= radius
        double NDotR = Vec3.Dot(Normal, ray.Direction);
        if (Math.Abs(NDotR) < epsilon) // check if normal and direction are perpendicular
            return false;

        double t = Vec3.Dot(Normal, Center - ray.Origin) / NDotR; // intersection with disc plane

        if (t <= tMin || t >= tMax)
            return false;

        Vec3 point = ray.At(t);
        if ((point - Center).Norm() > Radius)
            return false;

        hit.Point = point;
        hit.Normal = Vec3.Dot(ray.Direction, Normal) > 0 ? -Normal : Normal;
        hit.T = t;
        hit.Color = Color;

        return true;
    }
}