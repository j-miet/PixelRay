using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;

namespace PixelRay.SceneView.InstanceObject.Geometry;

/// <summary>
/// Plane y=0 with normal (0, 1, 0)
/// </summary>
public class Plane : IGeometry
{
    public Vec3 Normal = new(0, 1, 0);

    public bool Hit(Ray ray, Interval rayT, out HitRecord hit)
    {
        hit = default;

        if (Utils.IsEqual(ray.Direction.Y, 0))
            return false;

        double t = -ray.Origin.Y / ray.Direction.Y;

        if (!rayT.InClosed(t))
            return false;

        hit.T = t;
        hit.Point = ray.At(t);
        hit.SetFaceNormal(ray, Normal);
        hit.Geometry = this;

        return true;
    }
}