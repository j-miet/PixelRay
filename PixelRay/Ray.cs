namespace PixelRay;

/// <summary>
/// Ray function P(t) = A + t*b.
/// </summary>
/// <param name="origin">Ray origin point (A) i.e. eye point</param>
/// <param name="direction">Ray direction vector (b)</param>
public class Ray(Point3 origin, Vec3 direction)
{
    public Point3 Origin { get => _origin; }
    public Vec3 Direction { get => _direction; }

    /// <summary>
    /// Returns a ray position P(t) = A + t*b for given scalar t.
    /// </summary>
    /// <param name="t"></param>
    /// <returns>Point P(t)</returns>
    public Point3 At(double t)
    {
        return Origin + t * Direction;
    }

    private readonly Point3 _origin = origin;
    private readonly Vec3 _direction = direction;
}

