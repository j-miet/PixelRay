using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;
using PixelRay.SceneView.Instance.Geometry;

namespace PixelRay.SceneView.HitObjects;

/// <summary>
/// Origin-centered unit sphere.
/// </summary>
public class Sphere : IGeometry
{
    public bool Hit(Ray ray, Interval rayT, out HitRecord hit)
    {
        hit = default;

        Vec3 oc = ray.Origin;
        double a = Vec3.Dot(ray.Direction, ray.Direction);
        double halfB = Vec3.Dot(oc, ray.Direction);
        double c = Vec3.Dot(oc, oc) - 1;

        double discriminant = halfB * halfB - a * c;
        if (Utils.LessThan(discriminant, 0))
            return false;

        double sqrtD = Math.Sqrt(Math.Max(discriminant, 0.0));

        double t = (-halfB - sqrtD) / a;
        if (!rayT.InClosed(t))
        {
            t = (-halfB + sqrtD) / a;
            if (!rayT.InClosed(t))
                return false;
        }

        hit.T = t;
        hit.Point = ray.At(t);
        hit.SetFaceNormal(ray, hit.Point.Unit());
        hit.Geometry = this;

        return true;
    }
}