using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;

namespace PixelRay.SceneView.HitObjects;

/// <summary>
/// General quadric primitive Ax^2 + By^2 + Cz^2 + Dxy + Exz + Fyz + Gx + Hy + Iz + J = 0
/// Can describe same objects as some of the existing primitive classes e.g. unit sphere would be generated with
/// A=B=C=1, J = -r*r, set rest to 0.
/// </summary>
public class Quadric(
    ColorRGB color,
    double a, double b, double c,
    double d, double e, double f,
    double g, double h, double i,
    double j,
    Vec3 minBounds,
    Vec3 maxBounds
) : IHittable
{
    public ColorRGB Color = color;
    public double A = a, B = b, C = c; // square terms
    public double D = d, E = e, F = f; // product terms
    public double H = h, G = g, I = i; // linear terms
    public double J = j; // constant
    public Vec3 MinBounds = minBounds;
    public Vec3 MaxBounds = maxBounds;

    public bool Hit(Ray ray, Interval rayT, out HitRecord hit)
    {
        hit = default;

        // a quadric in Euclidean space can be written as P^T M P where M is a 4x4 matrix
        // M = [  A  D/2 E/2 G/2 ]
        //     | D/2  B  F/2 H/2 |
        //     | E/2 F/2  C  I/2 |
        //     [ G/2 H/2 I/2  J  ]
        // and P = (x, y, z) is (row) vector, P^T its transpose
        // (In literature this usually has multipliers of 2 with D, E, F, G, H, I to cancel denominator terms)
        // This can be written in compact form using matrices: if W is the upper-left 3x3 matrix of M (top-left is A, 
        // bottom-right is C) and L = (G, H, I) then P^T M P = P^T W P + L^T P + J 
        // Now substituting P = O + t*Q (Q ray direction as D already used) yields a quadratic with following 
        // coefficients:
        // a = Q^T W Q, b = 2*O^T W Q + L^T Q, c = O^T W O + L^T O + J

        double Ox = ray.Origin.X, Oy = ray.Origin.Y, Oz = ray.Origin.Z;
        double Dx = ray.Direction.X, Dy = ray.Direction.Y, Dz = ray.Direction.Z;

        // calculate coefficients from matrix forms
        double a = A * Dx * Dx + B * Dy * Dy + C * Dz * Dz + D * Dx * Dy + E * Dx * Dz + F * Dy * Dz;
        double b = 2 * A * Ox * Dx + 2 * B * Oy * Dy + 2 * C * Oz * Dz +
            D * (Ox * Dy + Oy * Dx) + E * (Ox * Dz + Oz * Dx) + F * (Oy * Dz + Oz * Dy) +
            G * Dx + H * Dy + I * Dz;
        double c = A * Ox * Ox + B * Oy * Oy + C * Oz * Oz +
            D * Ox * Oy + E * Ox * Oz + F * Oy * Oz +
            G * Ox + H * Oy + I * Oz + J;

        if (Utils.IsEqual(a, 0))
            return false;

        double discriminant = b * b - 4 * a * c;
        if (Utils.LessThan(discriminant, 0))
            return false;

        double sqrtD = Math.Sqrt(Math.Max(discriminant, 0.0));
        double t1 = (-b - sqrtD) / (2 * a);
        double t2 = (-b + sqrtD) / (2 * a);

        double t = double.PositiveInfinity;
        List<double> roots = [];
        Vec3 p;

        if (rayT.InClosed(t1)) roots.Add(t1);
        if (rayT.InClosed(t2)) roots.Add(t2);

        foreach (double candidate in roots)
        {
            p = ray.At(candidate);
            if (p.X >= MinBounds.X && p.X <= MaxBounds.X && // check bounding conditions
                p.Y >= MinBounds.Y && p.Y <= MaxBounds.Y &&
                p.Z >= MinBounds.Z && p.Z <= MaxBounds.Z)
            {
                t = candidate;
                break;
            }
        }

        if (t == double.PositiveInfinity)
            return false;

        p = ray.At(t);
        // normal(x,y,z) = grad(x,y,z) = <2*A*x + D*y + E*z + G, 2*B*y + D*x + F*z + H, 2*C*z + E*x + F*y + I>
        Vec3 normal = new Vec3(
            2 * A * p.X + D * p.Y + E * p.Z + G,
            2 * B * p.Y + D * p.X + F * p.Z + H,
            2 * C * p.Z + E * p.X + F * p.Y + I
        )
        .Unit();

        hit.Point = p;
        hit.SetFaceNormal(ray, normal);
        hit.T = t;
        hit.Color = Color;
        hit.Object = this;

        return true;
    }
}