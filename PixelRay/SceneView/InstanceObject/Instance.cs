using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;
using PixelRay.SceneView.InstanceObject.Geometry;
using PixelRay.SceneView.InstanceObject.Materials;

namespace PixelRay.SceneView.InstanceObject;

/// <summary>
/// Scene instance object
/// </summary>
/// <param name="geometry">Geometry object</param>
/// <param name="material">Material</param>
/// <param name="transform">Transform to modify base geometry (position, size/shape, rotation)</param>
public class Instance(
    IGeometry geometry,
    IMaterial material,
    Transform transform
) : IHittable
{
    // these are for Lua scripting
    public int Id;
    public string? Name;

    public IGeometry Geometry = geometry;
    public IMaterial Material = material;
    public Transform BaseTransform = new(transform.LocalToWorld)
    {
        Position = transform.Position
    };
    public Transform Transform = transform;

    public bool Hit(Ray ray, Interval rayT, out HitRecord hit)
    {
        Ray localRay = Transform.ToLocal(ray);
        Interval localRayT = new(MathConst.RayEpsilon, double.MaxValue);

        // don't use tMin/tMax here as those are from world space; instead check these conditions only after 
        // transforming is done locally and values are converted back to world space
        if (!Geometry.Hit(localRay, localRayT, out hit))
            return false;

        Vec3 worldPoint = Transform.ToWorldPoint(hit.Point);
        // here transpose of inverse is required instead of just inverse itself. This is because normal vectors are not 
        // exactly typical vectors: they must also preserve the perpendicularity with intersection plane which 
        // inverting alone won't guarantee.
        // Translation and uniform scaling (= scale all components with same number) works just fine
        // without this, but others such as rotation*, non-uniform scaling, shear and projections don't.
        // *Pure rotations, i.e. rotation matrix is orthonormal, also work because transpose of inverse equals to itself
        // for such matrices.
        Vec3 worldNormal = Transform.ToWorldNormal(hit.Normal).Unit();

        // projecting vector P-rayOrigin onto rayDirection and taking this length gives the distance in world space
        // also direction is normalized internally
        double worldT = Vec3.Dot(worldPoint - ray.Origin, ray.Direction);
        if (!localRayT.InClosed(worldT)) // now check the range using global boundaries
            return false;

        hit.Point = worldPoint;
        hit.T = worldT;
        hit.Material = Material;
        hit.SetFaceNormal(ray, worldNormal);
        // other data already stored inside during local hit handling

        return true;
    }
}