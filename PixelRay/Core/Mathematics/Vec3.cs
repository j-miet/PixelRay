namespace PixelRay.Core.Mathematics;

/// <summary>
/// 3D vector
/// </summary>
public readonly struct Vec3
{
    public readonly double X;
    public readonly double Y;
    public readonly double Z;

    public Vec3()
    {
        X = 0;
        Y = 0;
        Z = 0;
    }

    public Vec3(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    /// <summary>
    /// Display Vec3 object in (x, y, z) format
    /// </summary>
    public override string ToString() => string.Format("({0}, {1}, {2})", X, Y, Z);

    /// <summary>
    /// Index operator overload; access vector coordinates by corresponding index: v[0] = X , v[1] = Y, v[2] = Z 
    /// </summary>
    /// <returns>Index position coordinate</returns>
    public double this[int index]
    {
        get => GetValue(index);
    }

    /// <summary>
    /// Access coordinate by index. This method itself is not very useful and is more so a helper function to allow
    /// bracket syntax i.e. v[0] = v.X, v[1] = v.Y, v[2] = v.Z
    /// </summary>
    public double GetValue(int index)
    {
        return index switch
        {
            0 => X,
            1 => Y,
            2 => Z,
            _ => throw new ArgumentException("Invalid index"),
        };
    }

    public double NormSquared() => X * X + Y * Y + Z * Z;
    public double Norm() => Math.Sqrt(NormSquared());

    // aliases for norm functions
    public double LengthSquared() => NormSquared();
    public double Length() => Norm();

    /// <summary>
    /// This vector normalized, except for zero vector return itself
    /// </summary>
    public Vec3 Unit()
    {
        if ((X, Y, Z).Equals((0, 0, 0)))
            return new();

        return new Vec3(X, Y, Z) / Norm();
    }

    public Vec3 Normalize() => Unit(); // alias for obtaining unit vectors

    /// <summary>
    /// Dot product
    /// </summary>
    public static double Dot(Vec3 v, Vec3 u) => v.X * u.X + v.Y * u.Y + v.Z * u.Z;

    /// <summary>
    /// Cross product v x u
    /// </summary>
    public static Vec3 Cross(Vec3 v, Vec3 u)
    {
        return new Vec3(v.Y * u.Z - v.Z * u.Y,
                        v.Z * u.X - v.X * u.Z,
                        v.X * u.Y - v.Y * u.X);
    }

    /// <summary>
    /// Vector projection of v on u i.e. proj_u v
    /// Vector u is normalized automatically, no need to pass u.Unit()
    /// </summary>
    public static Vec3 Proj(Vec3 v, Vec3 u)
    {
        Vec3 uNormalized = u.Unit();
        return Dot(v, uNormalized) * uNormalized;
    }

    /// <summary>
    /// Orthogonal vector projection of v on u i.e. oproj_u v
    /// </summary>
    public static Vec3 Oproj(Vec3 v, Vec3 u)
    {
        return v - Proj(v, u);
    }

    /// <summary>
    /// Sample a random number from interval [-1, 1)
    /// </summary>
    public static double RandomDouble() => Random.Shared.NextDouble() * 2 - 1;

    /// <summary>
    /// Sample a random direction vector from a surface point
    /// </summary>
    public static Vec3 RandomHemisphere(Vec3 normal)
    {
        Vec3 dir;

        do
        {
            dir = new(
                RandomDouble(),
                RandomDouble(),
                RandomDouble()
            );
        }
        while (dir.NormSquared() < MathConst.Epsilon);

        dir = dir.Unit();

        if (Dot(dir, normal) < 0)
            dir = -dir;

        return dir;
    }

    /// <summary>
    /// Linear interpolation
    /// </summary>
    public static Vec3 Lerp(Vec3 a, Vec3 b, double t) => a * (1 - t) + b * t;

    /// <summary>
    /// Reflection of vector v against a surface normal n
    /// </summary>
    /// <param name="v">Vector pointing at normal n</param>
    /// <param name="n">Outward surface normal</param>
    /// <returns>Reflection vector</returns>
    public static Vec3 Reflect(Vec3 v, Vec3 n) => v - 2 * Dot(v, n) * n;

    public static Vec3 operator +(Vec3 v, Vec3 u) => new(v.X + u.X, v.Y + u.Y, v.Z + u.Z);

    public static Vec3 operator -(Vec3 v, Vec3 u) => new(v.X - u.X, v.Y - u.Y, v.Z - u.Z);

    public static Vec3 operator *(Vec3 v, Vec3 u) => new(v.X * u.X, v.Y * u.Y, v.Z * u.Z);

    public static Vec3 operator *(double t, Vec3 v) => new(t * v.X, t * v.Y, t * v.Z);

    public static Vec3 operator *(Vec3 v, double t) => t * v;

    public static Vec3 operator -(Vec3 v) => (-1) * v;

    /// <summary>
    /// Scalar division 1/t * v
    /// </summary>
    public static Vec3 operator /(Vec3 v, double t)
    {
        return (1 / t) * v;
    }
}