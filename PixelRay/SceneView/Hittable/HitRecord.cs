using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Instance.Geometry;
using PixelRay.SceneView.Materials;

namespace PixelRay.SceneView.Hittable;

/// <summary>
/// Record data of ray intersecting a hittable object.
/// </summary>
public struct HitRecord
{
    public Vec3 Point;
    public Vec3 Normal;
    public double T;
    public IMaterial Material;
    public IGeometry Geometry;

    /// <summary>
    /// Checks angle between ray and normal then set normal direction based on its facing.
    /// </summary>
    public void SetFaceNormal(Ray ray, Vec3 outwardNormal)
    {
        Normal = Utils.LessThan(Vec3.Dot(ray.Direction, outwardNormal), 0) ? outwardNormal : -outwardNormal;
    }
}