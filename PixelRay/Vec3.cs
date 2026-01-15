global using Point3 = PixelRay.Vec3; // alias for Vec3; useful when referring to a point instead of a vector

namespace PixelRay
{
    /// <summary>
    /// Class for 3D vectors
    /// </summary>
    public class Vec3
    {
        private (double x, double y, double z) e;

        public Vec3()
        {
            e = (0, 0, 0);
        }

        public Vec3(double e0, double e1, double e2)
        {
            e = (e0, e1, e2);
        }

        public (double x, double y, double z) Get
        {
            get => e;
        }

        public double X
        {
            get => e.x;
            set => e.x = value;
        }

        public double Y
        {
            get => e.y;
            set => e.y = value;
        }

        public double Z
        {
            get => e.z;
            set => e.z = value;
        }

        public double GetValue(int index)
        {
            return index switch
            {
                0 => e.x,
                1 => e.y,
                2 => e.z,
                _ => throw new ArgumentException("Invalid index"),
            };
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", e.x, e.y, e.z);
        }

        public double Length()
        {
            return Math.Sqrt(LengthSquared());
        }

        public double LengthSquared()
        {
            return e.x * e.x + e.y * e.y + e.z * e.z;
        }

        // index operator overload. Then: Vec3 v = new() -> v[0] has value v.X, v[1] has v.Y and v[2] has v.Z
        public double this[int index]
        {
            get => GetValue(index);
        }

        // arithmetic operation overloading requires use of 'static' in C#
        // inverse
        public static Vec3 operator -(Vec3 v)
        {
            return new Vec3(-v[0], -v[1], -v[2]);
        }

        // subtract
        public static Vec3 operator -(Vec3 v, Vec3 w)
        {
            return new Vec3(v[0] - w[0], v[1] - w[1], v[2] - w[2]);
        }

        // sum
        public static Vec3 operator +(Vec3 v, Vec3 w)
        {
            return new Vec3(v[0] + w[0], v[1] + w[1], v[2] + w[2]);
        }

        // left scalar product
        public static Vec3 operator *(double t, Vec3 v)
        {
            return new Vec3(t * v[0], t * v[1], t * v[2]);
        }

        // right scalar product
        public static Vec3 operator *(Vec3 v, double t)
        {
            return t * v;
        }

        //scalar division
        public static Vec3 operator /(Vec3 v, double t)
        {
            return 1 / t * v;
        }

        //element-wise multiplication
        public static Vec3 operator *(Vec3 v, Vec3 w)
        {
            return new Vec3(v[0] * w[0], v[1] * w[1], v[2] * w[2]);
        }

        // dot product
        public static double Dot(Vec3 v, Vec3 w)
        {
            return v[0] * w[0] + v[1] * w[1] + v[2] * w[2];
        }

        // cross product
        public static Vec3 Cross(Vec3 v, Vec3 w)
        {
            return new Vec3(v[1] * w[2] - v[2] * w[1],
                            v[2] * w[0] - v[0] * w[2],
                            v[0] * w[1] - v[1] * w[0]);
        }

        // unit vector
        public static Vec3 Unit(Vec3 v)
        {
            return v / v.Length();
        }
    }
}
