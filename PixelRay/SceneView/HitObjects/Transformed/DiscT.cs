using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;

namespace PixelRay.SceneView.HitObjects.Transformed;

/// <summary>
/// Planar disc defined by its center point, radius and normal.
/// </summary>
public class DiscT(Vec3 center, Vec3 normal, double radius, ColorRGB color) : IHittable
{
    public Vec3 Center = center;
    public double Radius = radius;
    public ColorRGB Color = color;

    public Vec3 Normal { get => _normal; set => _normal = value.Unit(); }

    public bool Hit(Ray ray, double tMin, double tMax, out HitRecord hit)
    {
        hit = default;

        // same math can be used here as with planes
        // first check perpendicularity, then calculate plane intersection. Finally check if distance between center
        // and plane intersection point is <= radius
        double NDotR = Vec3.Dot(_normal, ray.Direction);
        if (Math.Abs(NDotR) < Const.ParallelEpsilon) // check if normal and direction are perpendicular
            return false;

        double t = Vec3.Dot(_normal, Center - ray.Origin) / NDotR; // intersection with disc plane

        if (t < tMin || t > tMax)
            return false;

        Vec3 point = ray.At(t);
        if ((point - Center).Norm() > Radius + Const.HitEpsilon)
            return false;

        hit.Point = point;
        hit.SetFaceNormal(ray, _normal);
        hit.T = t;
        hit.Color = Color;
        hit.Object = this;

        return true;
    }

    private Vec3 _normal = normal;
}