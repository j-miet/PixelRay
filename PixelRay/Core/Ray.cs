using PixelRay.Core.Mathematics;

namespace PixelRay.Core;

/// <summary>
/// Ray function
/// </summary>
public readonly struct Ray(Vec3 origin, Vec3 direction)
{
    public readonly Vec3 Origin = origin;
    public readonly Vec3 Direction = direction.Unit();

    /// <summary>
    /// Returns a ray position R(t) = O + t*d
    /// </summary>
    public Vec3 At(double t)
    {
        return Origin + t * Direction;
    }
}