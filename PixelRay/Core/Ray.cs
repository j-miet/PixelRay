using PixelRay.Mathematics;

namespace PixelRay.Core;

/// <summary>
/// Ray function
/// </summary>
public readonly struct Ray(Vec3 origin, Vec3 direction)
{
    public readonly Vec3 Origin = origin;
    public readonly Vec3 Direction = direction;

    /// <summary>
    /// Returns a ray position R(t) = O + t*d for scalar t
    /// </summary>
    public Vec3 At(double t)
    {
        return Origin + t * Direction;
    }
}