namespace PixelRay.Core.Mathematics;

/// <summary>
/// Math constants
/// </summary>
public static class Const
{
    // Utils
    public const double QuarticEpsilon = 1e-6; // for picking real roots in quartic equations

    // Rendering
    public const double ClosestHitEpsilon = 1e-5;
    public const double RayIntersectOffset = 1e-4; // shadow ray normal offset
    public enum RenderMode
    {
        Default,
        DebugShadows,
        DebugNormals
    }

    // IHittable
    public const double HitMin = 1e-4; // ray tMin
    public const double HitEpsilon = 1e-8; // general epsilon for hit math: signed dot products, heights, radii, other zero value checks
    public const double HitDiscriminant = 1e-8; // quadratic discriminants
    public const double ParallelEpsilon = 1e-8; // parallel check between ray and normal
}