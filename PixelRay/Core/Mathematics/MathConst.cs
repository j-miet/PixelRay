namespace PixelRay.Core.Mathematics;

/// <summary>
/// Math constants
/// </summary>
public static class MathConst
{
    /// <summary>
    /// ray intersections: larger values for clear separation
    /// </summary>
    public const double RayEpsilon = 1e-4;

    /// <summary>
    /// numerical tests: zero conditions, acceptable error precision (1e-6 to 1e-8). <br/>
    /// Can be used with Utils compare operators and is the default epsilon value due to its frequent use compared to
    /// other epsilons
    /// </summary>
    public const double Epsilon = 1e-8;

    /// <summary>
    /// matrix operations are sensitive, use even higher precision (>1e-10). <br/>
    /// Can be used with Utils compare operators, just remember to pass this value to override default epsilon
    /// </summary>
    public const double MatrixEpsilon = 1e-12;

    /// <summary>
    /// for picking real roots in quartic equations (e.g. Torus intersections), don't use this elsewhere!
    /// </summary>
    public const double QuarticEpsilon = 1e-6;
}