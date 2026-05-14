using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;

namespace PixelRay.SceneView.InstanceObject.Geometry;

/// <summary>
/// Axis-aligned box
/// Preserves axes in it's local coordinate system => rotations don't work!
/// </summary>
/// <param name="minBounds">Lower x, y and z bounds</param>
/// <param name="maxBounds">Upper x, y and z bounds</param>
public class AABox(Vec3 minBounds, Vec3 maxBounds) : IGeometry
{
    public Vec3 MinBounds = minBounds;
    public Vec3 MaxBounds = maxBounds;

    public bool Hit(Ray ray, Interval rayT, out HitRecord hit)
    {
        hit = default;

        if (!IntersectAABB(ray, MinBounds, MaxBounds, out double t))
            return false;

        if (!rayT.InClosed(t))
            return false;

        hit.T = t;
        hit.Point = ray.At(t);
        hit.SetFaceNormal(ray, CalculateNormal(hit.Point, MinBounds, MaxBounds));
        hit.Geometry = this;

        return true;
    }

    private static bool IntersectAABB(Ray ray, Vec3 MinBounds, Vec3 MaxBounds, out double hitT)
    {
        hitT = 0.0;

        double tMin = double.NegativeInfinity;
        double tMax = double.PositiveInfinity;

        if (
            !Slab(ray.Origin.X, ray.Direction.X, new Interval(MinBounds.X, MaxBounds.X), ref tMin, ref tMax) ||
            !Slab(ray.Origin.Y, ray.Direction.Y, new Interval(MinBounds.Y, MaxBounds.Y), ref tMin, ref tMax) ||
            !Slab(ray.Origin.Z, ray.Direction.Z, new Interval(MinBounds.Z, MaxBounds.Z), ref tMin, ref tMax)
        )
            return false;

        hitT = tMin >= 0.0 ? tMin : tMax;
        return tMax >= Math.Max(tMin, 0.0);
    }

    private static bool Slab(
        double origin,
        double direction,
        Interval bounds,
        ref double tMin,
        ref double tMax
    )
    {
        // check if ray is parallel to slab by checking corresponding axis coordinate
        // if true then ray has to lie inside slab
        if (Utils.IsEqual(direction, 0))
        {
            if (!bounds.InClosed(origin))
                return false;

            return true;
        }

        // to check if ray axis coordinate lands inside slab: when does the 1D line hit the boundary e.g.
        // O + t*D = B => t = (B - O) / D
        double invD = 1.0 / direction;
        double t0 = (bounds.Min - origin) * invD;
        double t1 = (bounds.Max - origin) * invD;

        // if direction is negative, switch intersection parameters
        if (t0 > t1)
            (t0, t1) = (t1, t0);

        tMin = Math.Max(tMin, t0);
        tMax = Math.Min(tMax, t1);

        return tMin <= tMax;
    }

    private static Vec3 CalculateNormal(Vec3 point, Vec3 minBounds, Vec3 maxBounds)
    {
        if (Utils.IsEqual(point.X, minBounds.X))
            return new Vec3(-1, 0, 0);
        if (Utils.IsEqual(point.X, maxBounds.X))
            return new Vec3(1, 0, 0);

        if (Utils.IsEqual(point.Y, minBounds.Y))
            return new Vec3(0, -1, 0);
        if (Utils.IsEqual(point.Y, maxBounds.Y))
            return new Vec3(0, 1, 0);

        if (Utils.IsEqual(point.Z, minBounds.Z))
            return new Vec3(0, 0, -1);

        return new(0, 0, 1);
    }
}