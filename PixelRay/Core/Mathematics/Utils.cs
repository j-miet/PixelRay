using Complex = System.Numerics.Complex;

namespace PixelRay.Core.Mathematics;

/// <summary>
/// Various math utilities
/// </summary>
public static class Utils
{
    /// <summary>
    /// solves a quartic (4. degree) equation ax^4 + bx^3 + cx^2 + dx + e = 0 using Durand-Kerner method and returns
    /// only its real-valued roots
    /// </summary>
    public static double[] SolveQuartic(double a, double b, double c, double d, double e, int iterations = 50)
    {
        // normalize by the highest power term to use form x^4 + b/a*x^3 + c/a*x^2 + d/a*x + e/a*x under iterations
        b /= a;
        c /= a;
        d /= a;
        d /= a;

        Complex[] roots = [new(1, 0), new(0, 1), new(-1, 0), new(0, -1)]; // initial complex roots candidates

        for (int iter = 0; iter < iterations; iter++)
        {
            for (int i = 0; i < 4; i++)
            {
                Complex x = roots[i];
                Complex px = (((x + b) * x + c) * x + d) * x + e; // x^4 + b/a*x^3 + c/a*x^2 + d/a*x + e/a*x
                Complex denominator = 1;
                for (int j = 0; j < 4; j++)
                {
                    if (i != j)
                        denominator *= x - roots[j];
                }

                roots[i] = roots[i] - (px / denominator); // calculate next root in fixed-point iteration
            }
        }

        List<double> realRoots = [];

        foreach (Complex root in roots)
        {
            if (IsEqual(Math.Abs(root.Imaginary), 0, MathConst.QuarticEpsilon))
                realRoots.Add(root.Real);
        }

        return [.. realRoots];
    }

    /// <summary>
    /// Builds a orthonormal basis from a single vector representing the upward axis normal.
    /// Doesn't return a value. Instead sets values for tangent and bitangent references
    /// </summary>
    public static void BuildOrthonormalBasis(Vec3 normal, out Vec3 tangent, out Vec3 bitangent)
    {
        Vec3 up = Math.Abs(normal.Z) < 0.999 ? new(0, 1, 0) : new(1, 0, 0);

        tangent = Vec3.Cross(up, normal).Unit();
        bitangent = Vec3.Cross(normal, tangent);
    }

    /// <summary>
    /// Check if ray hits a origin-centered sphere with radius r. Ray direction must be normalized!
    /// </summary>
    public static bool HitBoundingSphere(Ray ray, double r)
    {
        // because ray D is assumed to be normalized, discriminant can be simplified
        double b = Vec3.Dot(ray.Origin, ray.Direction);
        double c = Vec3.Dot(ray.Origin, ray.Origin) - r * r;
        double discriminant = b * b - c;
        return discriminant >= -MathConst.Epsilon;
    }

    /// <summary>
    /// Checks if value equals to comparison value. All compare methods use closed interval epsilon precision for 0 
    /// internally i.e. x = 0 iff x in [-epsilon, epsilon]. For example this method uses Abs(val-compare) = 0 which 
    /// then translates to Abs(val-compare) LTOE epsilon (LTOE is less than or equal)
    /// </summary>
    /// <param name="val">First value</param>
    /// <param name="compare">Comparison value</param>
    /// <param name="epsilon">Required precision. Default is MathConst.Epsilon: comparison methods are most often used 
    /// with hittable objects so this avoids repeating same constant everywhere </param>
    /// <returns></returns>
    public static bool IsEqual(double val, double compare, double epsilon = MathConst.Epsilon)
    {
        return Math.Abs(val - compare) <= epsilon;
    }
    public static bool GreaterThan(double left, double right, double epsilon = MathConst.Epsilon)
    {
        return left > right + epsilon;
    }
    public static bool GreaterThanOrEqual(double left, double right, double epsilon = MathConst.Epsilon)
    {
        return left >= right + epsilon;
    }
    public static bool LessThan(double left, double right, double epsilon = MathConst.Epsilon)
    {
        return left < right - epsilon;
    }
    public static bool LessThanOrEqual(double left, double right, double epsilon = MathConst.Epsilon)
    {
        return left <= right - epsilon;
    }
}
