using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;

namespace PixelRay.SceneView.InstanceObject.Geometry;

/// <summary>
/// Object with base geometry
/// </summary>
public interface IGeometry
{
    bool Hit(Ray ray, Interval rayT, out HitRecord hit);
    // AABB BoundingBox(); later for BVH
}