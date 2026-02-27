using PixelRay.Core;
using PixelRay.Mathematics;
using PixelRay.SceneObjects;

namespace PixelRay.Geometry;

/// <summary>
/// Planar disc
/// </summary>
public class Disc(Vec3 center, double radius, Vec3 normal, ColorRGB color) : IHittable
{
    public Vec3 Center = center;
    public double Radius = radius;
    public Vec3 Normal = normal;
    public ColorRGB Color = color;

    public bool Hit(Ray ray, double tMin, double tMax, out HitRecord hit)
    {
        hit = default;

        // same math can be used here as with planes
        // first check perpendicularity, then calculate plane intersection. Finally check if distance between center
        // and plane intersection point is <= radius
        double NDotR = Vec3.Dot(Normal, ray.Direction);
        if (Math.Abs(NDotR) < 0.0001) // check if normal and direction are perpendicular
            return false;

        double t = Vec3.Dot(Center - ray.Origin, Normal) / NDotR; // intersection with disc plane

        Vec3 point = ray.At(t);
        if ((point - Center).Norm() > Radius)
            return false;

        if (t <= tMin || t >= tMax)
            return false;

        hit.Point = point;
        hit.Normal = Normal.Unit();
        hit.T = t;
        hit.Color = Color;

        return true;
    }
}