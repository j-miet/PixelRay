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
    public static double[] SolveQuartic(double a, double b, double c, double d, double e)
    {
        // normalize by the highest power term to use form x^4 + b/a*x^3 + c/a*x^2 + d/a*x + e/a*x under iterations
        b /= a;
        c /= a;
        d /= a;
        d /= a;

        Complex[] roots = [new(1, 0), new(0, 1), new(-1, 0), new(0, -1)]; // initial complex roots candidates

        for (int iter = 0; iter < 50; iter++)
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
            if (Math.Abs(root.Imaginary) < Const.QuarticEpsilon) // if imaginary part falls under error
                realRoots.Add(root.Real);
        }

        return [.. realRoots]; // unpack list into an array
    }

    /// <summary>
    /// Builds a orthonormal basis from a single vector representing y-axis of new basis.
    /// Sets reference values of v1 and v3 as x- and z-axis vectors respectively. This means the new basis can be used
    /// to rotate between standard basis and new basis with v2 as the y-axis correspondent.
    /// </summary>
    /// <param name="w">Reference to vector which becomes analogous to x-axis under new system</param>
    /// <param name="u">Y-axis of new basis</param>
    /// <param name="v">Reference to new z-axis vector</param>
    public static void BuildOrthonormalBasisFromY(out Vec3 xAxisRef, Vec3 yAxis, out Vec3 zAxisRef)
    {
        // helper vector for creating orthogonal basis. Checks x-coordinate of axis vector and ensures that it's not
        // too aligned with helper, otherwise cross product might create a vector with norm almost zero.
        Vec3 helper = Math.Abs(yAxis.X) > 0.9 ? new Vec3(0, 1, 0) : new Vec3(1, 0, 0);

        // update x and z references; cross product creates orthogonal vector
        xAxisRef = Vec3.Cross(helper, yAxis).Unit();
        zAxisRef = Vec3.Cross(yAxis, xAxisRef); // both vectors already normalized means cross product is too
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
        return discriminant >= -Const.HitDiscriminant;
    }
}
