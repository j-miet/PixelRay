namespace PixelRay.Core.Mathematics;

/// <summary>
/// Math constants
/// </summary>
public static class Const
{
    // Utils
    public const double QuarticEpsilon = 1e-6; // for picking real roots in quartic equations

    // Rendering
    public const double ClosestHitEpsilon = 1e-5; // when iterating over hittable objects e.g. during pixel tracing
    public const double ShadowRayIntersectOffset = 1e-4; // shadow ray normal offset
    public enum DebugMode
    {
        None,
        BlockedShadows,     // blocked shadow pixel detection
        Normals,            // visualize surface normals
        DepthHeat,          // visualize distance along ray
        ObjectID            // visualize which object was hit
    }

    // IHittable
    public const double HitMin = 1e-4; // ray tMin
    public const double HitEpsilon = 1e-8; // general epsilon for hit math: signed dot products, heights, radii, other zero value checks
    public const double HitDiscriminant = 1e-8; // quadratic discriminants
    public const double ParallelEpsilon = 1e-8; // parallel check between ray and normal
}