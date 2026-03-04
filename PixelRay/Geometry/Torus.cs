using PixelRay.Core;
using PixelRay.Mathematics;
using PixelRay.SceneObjects;

namespace PixelRay.Geometry;

/// <summary>
/// Torus defined by center, axis normal, major radius and minor radius.
/// </summary>
public class Torus : IHittable
{
    // TODO improvements to calculations as current render speed is bad even if it's supposed to be slower than others
    public readonly Vec3 Center;
    public readonly double MajorR;
    public readonly double MinorR;
    public readonly ColorRGB Color;

    public Torus(Vec3 center, Vec3 axis, double majorRadius, double minorRadius, ColorRGB color)
    {
        Center = center;
        MajorR = majorRadius;
        MinorR = minorRadius;
        Color = color;

        vY = axis.Unit();
        // orthonormal basis with respect to torus y-axis allows moving between standard and torus bases. This way
        // intersection point is easy to calculate with respect to standard origin centered torus, and normals can
        // be calculated in local base then inverted/rotated back to standard basis
        Tools.BuildOrthonormalBasisFromY(out vX, vY, out vZ);
    }

    public bool Hit(Ray ray, double tMin, double tMax, out HitRecord hit)
    {
        hit = default;
        // torus can be expressed by a 4. degree equation (x^2 + y^2 + z^2 + R^2 - r^2)^2 - 4*R^2*(x^2 + z^2) = 0 where
        // major radius R is the distance from tube center to center of torus and minor radius r the tube radius
        // This type of torus is aligned with y-axis and origin-centered. For general axis direction, it's easier to 
        // rotate incoming rays and calculate intersection with standard y-aligned origin-centered torus then apply 
        // inverse rotation back
        // For standard torus, substituting a *normalized* ray into above equation results into a lot of
        // algebra. If O = ray origin, D = ray direction, W = Dot(O, O) - r^2 - R^2, _.X = vector x (or Y/Z) coordinate,
        // then coefficients are
        // a = 1, b = 4*Dot(O, D), c = 2*W + 4*Dot(O,D)^2 + 4*R^2 * (D.Y)^2
        // d = 4*Dot(O, D) * W + 8*R^2 * O.Y * D.Y, e = W^2 - 4*R^2 * (r^2 - (O.Y)^2)
        // Just like with axis: when using arbitrary center instead of origin, set ray origin to torus center
        // now torus center is effectively the camera where rays fly towards origin-centered torus. It also means the
        // ray hits torus at same length t it would hit in standard basis. Only normals require inverse operation as
        // they need to be rotated back to standard basis

        // first, transform ray into local torus basis
        Vec3 O = PointToLocal(ray.Origin);
        Vec3 D = DirectionToLocal(ray.Direction);

        // bounding sphere test = check if ray hits the sphere surrounding torus
        if (!HitBoundingSphere(O, D, MajorR + MinorR))
            return false;

        double OO = Vec3.Dot(O, O);
        double OD = Vec3.Dot(O, D);
        double W = OO - MinorR * MinorR - MajorR * MajorR;

        double a = 1;
        double b = 4 * OD;
        double c = 2 * W + 4 * OD * OD + 4 * MajorR * MajorR * D.Y * D.Y;
        double d = 4 * OD * W + 8 * MajorR * MajorR * O.Y * D.Y;
        double e = W * W - 4 * MajorR * MajorR * (MinorR * MinorR - O.Y * O.Y);

        double[] roots = Tools.SolveQuartic(a, b, c, d, e);
        double closest = double.PositiveInfinity;

        foreach (double t in roots)
        {
            if (t > tMin && t < tMax && t < closest)
                closest = t;
        }

        if (closest == double.PositiveInfinity)
            return false;

        hit.T = closest;
        hit.Point = ray.At(closest);
        hit.Normal = CalculateNormal(hit.Point);
        hit.Color = Color;

        return true;
    }

    // local orthonormal basis coordinates for torus
    private readonly Vec3 vX;
    private readonly Vec3 vY;
    private readonly Vec3 vZ;

    /// <summary>
    /// Calculates torus normal at given point by first calculating it locally in torus space then transforming it back
    /// to standard basis
    /// </summary>
    private Vec3 CalculateNormal(Vec3 point)
    {
        Vec3 p = PointToLocal(point);

        // normal is just gradient of torus surface equation at point p. If H = x^2 + y^2 + z^2 + R^2 - r^2 then
        // grad(x,y,z) = <4*x*(H - 2*R^2), 4*y*H, 4*z*(H - 2*R^2)>
        double H = p.NormSquared() + MajorR * MajorR - MinorR * MinorR;
        Vec3 gradLocal = new(4 * p.X * (H - 2 * MajorR * MajorR), 4 * p.Y * H, 4 * p.Z * (H - 2 * MajorR * MajorR));

        // now move from local coordinates to standard basis using change-of-basis matrix [vX vY vZ]
        return (gradLocal.X * vX + gradLocal.Y * vY + gradLocal.Z * vZ).Unit();
    }

    private static bool HitBoundingSphere(Vec3 O, Vec3 D, double radius)
    {
        // because ray D is assumed to be normalized, discriminant can be simplified
        double b = Vec3.Dot(O, D);
        double c = Vec3.Dot(O, O) - radius * radius;
        double discriminant = b * b - c;
        return discriminant >= 0;
    }

    /// <summary>
    /// Transform point to torus local coordinates
    /// </summary>
    private Vec3 PointToLocal(Vec3 p)
    {
        Vec3 d = p - Center;
        // project vector d on each coordinate axis in local basis. This operation matches that of transposing the
        // change-of-basis matrix then multiplying d with it. 
        // Why transpose instead of inverse? For orthonormal (basis) matrices these are the same process. Therefore 
        // inverting = transposing moves from standard basis to local.
        return new(Vec3.Dot(d, vX), Vec3.Dot(d, vY), Vec3.Dot(d, vZ));
    }

    /// <summary>
    /// Transform a direction vector to torus local coordinates
    /// </summary>
    private Vec3 DirectionToLocal(Vec3 d) => new(Vec3.Dot(d, vX), Vec3.Dot(d, vY), Vec3.Dot(d, vZ));
}