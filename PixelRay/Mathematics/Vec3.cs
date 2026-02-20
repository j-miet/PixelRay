namespace PixelRay.Mathematics;

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
    /// <returns></returns>
    public override string ToString() => string.Format("({0}, {1}, {2})", X, Y, Z);

    /// <summary>
    /// Index operator overload; access vector coordinates by corresponding index: v[0] = X , v[1] = Y, v[2] = Z 
    /// </summary>
    /// <param name="index"></param>
    /// <returns>Index position coordinate</returns>
    public double this[int index]
    {
        get => GetValue(index);
    }

    /// <summary>
    /// Access coordinate by index. This method itself is not very useful and is more so a helper function to allow
    /// bracket syntax i.e. v[0] = v.X, v[1] = v.Y, v[2] = v.Z
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
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

    /// <summary>
    /// Norm/length squared
    /// </summary>
    /// <returns></returns>
    public double LengthSquared() => X * X + Y * Y + Z * Z;

    /// <summary>
    /// Norm/Length of a vector
    /// </summary>
    /// <returns></returns>
    public double Norm() => Math.Sqrt(LengthSquared());

    /// <summary>
    /// </summary>
    /// <param name="v"></param>
    /// <param name="u"></param>
    /// <returns>Vector v+u</returns>
    public static Vec3 operator +(Vec3 v, Vec3 u) => new(v.X + u.X, v.Y + u.Y, v.Z + u.Z);

    /// <summary>
    /// </summary>
    /// <param name="v"></param>
    /// <param name="u"></param>
    /// <returns>Vector v-u</returns>
    public static Vec3 operator -(Vec3 v, Vec3 u) => new(v.X - u.X, v.Y - u.Y, v.Z - u.Z);

    /// <summary>
    /// Element-wise vector product
    /// </summary>
    /// <param name="v"></param>
    /// <param name="u"></param>
    /// <returns>Vector v * u</returns>
    public static Vec3 operator *(Vec3 v, Vec3 u) => new(v.X * u.X, v.Y * u.Y, v.Z * u.Z);

    /// <summary>
    /// Scalar multiplication
    /// </summary>
    /// <param name="t"></param>
    /// <param name="v"></param>
    /// <returns>Vector t*v</returns>
    public static Vec3 operator *(double t, Vec3 v) => new(t * v.X, t * v.Y, t * v.Z);

    /// <summary>
    /// Scalar multiplication
    /// </summary>
    /// <param name="v"></param>
    /// <param name="t"></param>
    /// <returns>Vector t*v</returns>
    public static Vec3 operator *(Vec3 v, double t) => t * v;

    /// <summary>
    /// Vector inverse
    /// </summary>
    /// <param name="v"></param>
    /// <returns>Vector -v</returns>
    public static Vec3 operator -(Vec3 v) => (-1) * v;

    /// <summary>
    /// Scalar division
    /// </summary>
    /// <param name="v"></param>
    /// <param name="t"></param>
    /// <returns>1/t * v; if t = 0, the zero vector</returns>
    public static Vec3 operator /(Vec3 v, double t)
    {
        if (t == 0)
        {
            return new();
        }
        return 1 / t * v;
    }

    /// <summary>
    /// Unit vector of this Vec3 object
    /// </summary>
    /// <returns>This vector with norm = 1, except for zero vector itself</returns>
    public Vec3 Unit()
    {
        if ((X, Y, Z).Equals((0, 0, 0)))
        {
            return new();
        }
        return new Vec3(X, Y, Z) / Norm();
    }

    /// <summary>
    /// Dot product
    /// </summary>
    /// <param name="v"></param>
    /// <param name="u"></param>
    /// <returns></returns>
    public static double Dot(Vec3 v, Vec3 u) => v.X * u.X + v.Y * u.Y + v.Z * u.Z;

    /// <summary>
    /// Cross product
    /// </summary>
    /// <param name="v">Left vector</param>
    /// <param name="u">Right vector</param>
    /// <returns>Cross product vector v x u</returns>
    public static Vec3 Cross(Vec3 v, Vec3 u)
    {
        return new Vec3(v.Y * u.Z - v.Z * u.Y,
                        v.Z * u.X - v.X * u.Z,
                        v.X * u.Y - v.Y * u.X);
    }

    /// <summary>
    /// Reflection of vector v against a surface normal n
    /// </summary>
    /// <param name="v">Vector pointing at surface normal n</param>
    /// <param name="n">Surface normal vector</param>
    /// <returns>Reflection vector</returns>
    public static Vec3 Reflect(Vec3 v, Vec3 n) => v - 2 * Dot(v, n) * n;

}