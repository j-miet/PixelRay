using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;

namespace PixelRay.SceneView.HitObjects.Transformed;

/// <summary>
/// Plane defined by a point and a normal.
/// </summary>
public class PlaneT(Vec3 point, Vec3 normal, ColorRGB color) : IHittable
{
    public Vec3 Point = point;
    public ColorRGB Color = color;

    public Vec3 Normal { get => _normal; set => _normal = value.Unit(); }

    public bool Hit(Ray ray, double tMin, double tMax, out HitRecord hit)
    {
        hit = default;
        // plane is defined by all points x such that Dot(n, x - Point) = 0
        // Substituting ray R(t) into x yields Dot(n, O + t*D - Point) = 0 which can be simplified into
        // Dot(n, Point - O) / Dot(n, D)
        // denominator term Dot(n, d) also tells the angle between ray and plane.
        double NDotR = Vec3.Dot(_normal, ray.Direction);
        if (Math.Abs(NDotR) < Const.ParallelEpsilon) // check if normal and direction are perpendicular
            return false;

        double t = Vec3.Dot(_normal, Point - ray.Origin) / NDotR;

        if (t < tMin || t > tMax)
            return false;

        hit.Point = ray.At(t);
        hit.SetFaceNormal(ray, _normal);
        hit.T = t;
        hit.Color = Color;
        hit.Object = this;

        return true;
    }

    private Vec3 _normal = normal;
}