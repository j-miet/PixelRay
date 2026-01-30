namespace PixelRay;

/// <summary>
/// Class for 3D vectors. Can also be used for any 3D metric such as colors, points in space etc.
/// </summary>
public class Vec3
{
    public Vec3()
    {
        Data = (0, 0, 0);
    }

    public Vec3(double x, double y, double z)
    {
        Data = (x, y, z);
    }

    public (double, double, double) Data { get; }

    public double X { get => Data.Item1; }
    public double Y { get => Data.Item2; }
    public double Z { get => Data.Item3; }

    public override string ToString()
    {
        return string.Format("({0}, {1}, {2})", Data.Item1, Data.Item2, Data.Item3);
    }

    public double Length()
    {
        return Math.Sqrt(LengthSquared());
    }

    public double LengthSquared()
    {
        return Data.Item1 * Data.Item1 + Data.Item2 * Data.Item2 + Data.Item3 * Data.Item3;
    }

    /// <summary>
    /// Index operator overload; access vector coordinates by corresponding index: 0 for x , 1 for y, 2 for z 
    /// </summary>
    /// <param name="index"></param>
    /// <returns>Index position coordinate</returns>
    public double this[int index]
    {
        get => GetValue(index);
    }

    // arithmetic operation overloading requires use of 'static' in C#

    /// <summary>
    /// Vector inverse; use it with syntax -v
    /// </summary>
    /// <param name="v"></param>
    /// <returns>Vector -v</returns>
    public static Vec3 operator -(Vec3 v)
    {
        return new Vec3(-v.X, -v.Y, -v.Z);
    }

    /// <summary>
    /// Subtracts vector w from v
    /// </summary>
    /// <param name="v"></param>
    /// <param name="w"></param>
    /// <returns>Vector v - w</returns>
    public static Vec3 operator -(Vec3 v, Vec3 w)
    {
        return new Vec3(v.X - w.X, v.Y - w.Y, v.Z - w.Z);
    }

    /// <summary>
    /// Sum of vectors
    /// </summary>
    /// <param name="v"></param>
    /// <param name="w"></param>
    /// <returns>Vector v + w</returns>
    public static Vec3 operator +(Vec3 v, Vec3 w)
    {
        return new Vec3(v.X + w.X, v.Y + w.Y, v.Z + w.Z);
    }

    /// <summary>
    /// Left scalar product
    /// </summary>
    /// <param name="t"></param>
    /// <param name="v"></param>
    /// <returns>t * v</returns>
    public static Vec3 operator *(double t, Vec3 v)
    {
        return new Vec3(t * v.X, t * v.Y, t * v.Z);
    }

    /// <summary>
    /// Right scalar product
    /// </summary>
    /// <param name="v"></param>
    /// <param name="t"></param>
    /// <returns>t * v</returns>
    public static Vec3 operator *(Vec3 v, double t)
    {
        return t * v;
    }

    /// <summary>
    /// Scalar division
    /// </summary>
    /// <param name="v"></param>
    /// <param name="t"></param>
    /// <returns>(1/t) * v</returns>
    public static Vec3 operator /(Vec3 v, double t)
    {
        if (t == 0)
        {
            throw new DivideByZeroException("Cannot divide a vector by zero!");
        }
        return 1 / t * v;
    }

    //element-wise multiplication
    /// <summary>
    /// Element-wise multiplication
    /// </summary>
    /// <param name="v"></param>
    /// <param name="w"></param>
    /// <returns>Element-wise product vector</returns>
    public static Vec3 operator *(Vec3 v, Vec3 w)
    {
        return new Vec3(v.X * w.X, v.Y * w.Y, v.Z * w.Z);
    }

    /// <summary>
    /// Dot product
    /// </summary>
    /// <param name="v"></param>
    /// <param name="w"></param>
    /// <returns>Dot product of v and w</returns>
    public static double Dot(Vec3 v, Vec3 w)
    {
        return v.X * w.X + v.Y * w.Y + v.Z * w.Z;
    }

    /// <summary>
    /// Cross product
    /// </summary>
    /// <param name="v"></param>
    /// <param name="w"></param>
    /// <returns>Cross product v x w</returns>
    public static Vec3 Cross(Vec3 v, Vec3 w)
    {
        return new Vec3(v.Y * w.Z - v.Z * w.Y,
                        v.Z * w.X - v.X * w.Z,
                        v.X * w.Y - v.Y * w.X);
    }

    /// <summary>
    /// Unit vector
    /// </summary>
    /// <param name="v"></param>
    /// <returns>Unit vector of v</returns>
    public static Vec3 Unit(Vec3 v)
    {
        double len = v.Length();
        if (len == 0)
        {
            throw new DivideByZeroException("Zero vector has no unit vector!");
        }
        return v / v.Length();
    }

    private double GetValue(int index)
    {
        return index switch
        {
            0 => Data.Item1,
            1 => Data.Item2,
            2 => Data.Item3,
            _ => throw new ArgumentException("Invalid index"),
        };
    }
}

