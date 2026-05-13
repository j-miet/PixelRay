using PixelRay.Core.Mathematics;

namespace PixelRay.Tests;

public static class CoreHelper
{
    public static bool NearlyEqual(double a, double b, double eps = 1e-10)
    {
        return Math.Abs(a - b) < eps;
    }

    public static bool IsUnit(Vec3 v, double eps = 1e-10)
    {
        return Math.Abs(v.Length() - 1.0) < eps;
    }

    public static bool NearlyEqual(Vec3 a, Vec3 b, double eps = 1e-10)
    {
        return
            Math.Abs(a.X - b.X) < eps &&
            Math.Abs(a.Y - b.Y) < eps &&
            Math.Abs(a.Z - b.Z) < eps;
    }

    public static bool NearlyEqual(Matrix4x4 a, Matrix4x4 b, double eps = 1e-10)
    {
        return
            Math.Abs(a.M00 - b.M00) < eps &&
            Math.Abs(a.M01 - b.M01) < eps &&
            Math.Abs(a.M02 - b.M02) < eps &&
            Math.Abs(a.M03 - b.M03) < eps &&

            Math.Abs(a.M10 - b.M10) < eps &&
            Math.Abs(a.M11 - b.M11) < eps &&
            Math.Abs(a.M12 - b.M12) < eps &&
            Math.Abs(a.M13 - b.M13) < eps &&

            Math.Abs(a.M20 - b.M20) < eps &&
            Math.Abs(a.M21 - b.M21) < eps &&
            Math.Abs(a.M22 - b.M22) < eps &&
            Math.Abs(a.M23 - b.M23) < eps &&

            Math.Abs(a.M30 - b.M30) < eps &&
            Math.Abs(a.M31 - b.M31) < eps &&
            Math.Abs(a.M32 - b.M32) < eps &&
            Math.Abs(a.M33 - b.M33) < eps;
    }
}