using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;
using PixelRay.SceneView.Materials;

namespace PixelRay.SceneView.HitObjects;

/// <summary>
/// Unit origin-centered disc on plane y=0 with normal (0, 1, 0)
/// </summary>
public class Disc(IMaterial material) : IHittable
{
    public IMaterial Material = material;
    public Vec3 Normal = new(0, 1, 0);

    public bool Hit(Ray ray, Interval rayT, out HitRecord hit)
    {
        hit = default;

        if (Utils.IsEqual(ray.Direction.Y, 0))
            return false;

        double t = -ray.Origin.Y / ray.Direction.Y;

        if (!rayT.InClosed(t))
            return false;

        Vec3 point = ray.At(t);
        if (Utils.GreaterThan(point.Norm(), 1))
            return false;

        hit.T = t;
        hit.Point = point;
        hit.SetFaceNormal(ray, Normal);
        hit.Material = Material;
        hit.Object = this;

        return true;
    }
}