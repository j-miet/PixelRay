using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;

namespace PixelRay.SceneView.HitObjects;

/// <summary>
/// Plane y=0 with normal (0, 1, 0)
/// </summary>
public class Plane(ColorRGB color) : IHittable
{
    public ColorRGB Color = color;
    public Vec3 Normal = new(0, 1, 0);

    public bool Hit(Ray ray, double tMin, double tMax, out HitRecord hit)
    {
        hit = default;

        if (Utils.IsEqual(ray.Direction.Y, 0))
            return false;

        double t = -ray.Origin.Y / ray.Direction.Y;

        if (t < tMin || t > tMax)
            return false;

        hit.Point = ray.At(t);
        hit.SetFaceNormal(ray, Normal);
        hit.T = t;
        hit.Color = Color;
        hit.Object = this;

        return true;
    }
}