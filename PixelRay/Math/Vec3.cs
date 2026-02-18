namespace PixelRay.Vec3;

public readonly struct Vec3(float x, float y, float z)
{
    public readonly float X = x;
    public readonly float Y = y;
    public readonly float Z = z;

    /// <summary>
    /// Display Vec3 object in (x, y, z) format
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return string.Format("({0}, {1}, {2})", X, Y, Z);
    }

    /// <summary>
    /// Norm/length squared
    /// </summary>
    /// <returns></returns>
    public float LengthSquared()
    {
        return X * X + Y * Y + Z * Z;
    }

    /// <summary>
    /// Norm/Length of a vector
    /// </summary>
    /// <returns></returns>
    public float Norm()
    {
        return (float)Math.Sqrt((double)LengthSquared());
    }

    /// <summary>
    /// </summary>
    /// <param name="v"></param>
    /// <param name="u"></param>
    /// <returns>Vector v+u</returns>
    public static Vec3 operator +(Vec3 v, Vec3 u)
    {
        return new Vec3(v.X + u.X, v.Y + u.Y, v.Z + u.Z);
    }

    /// <summary>
    /// </summary>
    /// <param name="v"></param>
    /// <param name="u"></param>
    /// <returns>Vector v-u</returns>
    public static Vec3 operator -(Vec3 v, Vec3 u)
    {
        return new Vec3(v.X - u.X, v.Y - u.Y, v.Z - u.Z);
    }

    /// <summary>
    /// Element-wise vector product
    /// </summary>
    /// <param name="v"></param>
    /// <param name="u"></param>
    /// <returns>Vector v * u</returns>
    public static Vec3 operator *(Vec3 v, Vec3 u)
    {
        return new Vec3(v.X * u.X, v.Y * u.Y, v.Z * u.Z);
    }

    /// <summary>
    /// Scalar multiplication
    /// </summary>
    /// <param name="t"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    public static Vec3 operator *(float t, Vec3 v)
    {
        return new Vec3(t * v.X, t * v.Y, t * v.Z);
    }

    /// <summary>
    /// Scalar multiplication
    /// </summary>
    /// <param name="v"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Vec3 operator *(Vec3 v, float t)
    {
        return t * v;
    }

    /// <summary>
    /// Scalar division
    /// </summary>
    /// <param name="v"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    /// <exception cref="DivideByZeroException">if t = 0</exception>
    public static Vec3 operator /(Vec3 v, float t)
    {
        if (t == 0)
        {
            throw new DivideByZeroException("Cannot divide a vector by scalar 0.");
        }
        return 1 / t * v;
    }

    /// <summary>
    /// Unit vector of this Vec3 object
    /// </summary>
    /// <returns>This vector with norm = 1</returns>
    /// <exception cref="DivideByZeroException">If origin vector (0, 0, 0) is used</exception>
    public Vec3 Unit()
    {
        if ((X, Y, Z).Equals((0, 0, 0)))
        {
            throw new DivideByZeroException("Origin vector has no unit.");
        }
        return new Vec3(X, Y, Z) / Norm();
    }

    /// <summary>
    /// Dot product
    /// </summary>
    /// <param name="v"></param>
    /// <param name="u"></param>
    /// <returns></returns>
    public static float Dot(Vec3 v, Vec3 u)
    {
        return v.X * u.X + v.Y * u.Y + v.Z * u.Z;
    }

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
    /// Reflection of vector v against a surface with normal n
    /// </summary>
    /// <param name="v">Vector pointing at surface with normal n</param>
    /// <param name="n">Surface normal vector</param>
    /// <returns>Reflection vector</returns>
    public static Vec3 Reflect(Vec3 v, Vec3 n)
    {
        return v - 2 * Dot(v, n) * n;
    }

}