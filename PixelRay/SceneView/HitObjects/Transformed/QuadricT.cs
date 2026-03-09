using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;

namespace PixelRay.SceneView.HitObjects.Transformed;

/// <summary>
/// General quadric primitive Ax^2 + By^2 + Cz^2 + Dxy + Exz + Fyz + Gx + Hy + Iz + J = 0
/// Can describe same objects as some of the existing primitive classes e.g. unit sphere would be generated with
/// A=B=C=1, J = -r*r, set rest to 0.
/// Does not implement cutting => objects can expand indefinitely.
/// </summary>
public class QuadricT(
    double a, double b, double c,
    double d, double e, double f,
    double g, double h, double i,
    double j,
    Vec3 position,
    ColorRGB color,
    double xmin = -0.5,
    double xmax = 0.5,
    double ymin = -0.5,
    double ymax = 0.5,
    double zmin = -0.5,
    double zmax = 0.5
) : IHittable
{
    public double A = a, B = b, C = c; // square terms
    public double D = d, E = e, F = f; // product terms
    public double H = h, G = g, I = i; // linear terms
    public double J = j; // constant
    public Vec3 Position = position;
    public ColorRGB Color = color;
    public double Xmin = xmin, Xmax = xmax, Ymin = ymin, Ymax = ymax, Zmin = zmin, Zmax = zmax;

    public bool Hit(Ray ray, double tMin, double tMax, out HitRecord hit)
    {
        hit = default;

        Vec3 O = ray.Origin - Position; // local origin
        Vec3 Dir = ray.Direction;
        Ray localRay = new(O, Dir); // ray in local coordinates

        // quadric in Euclidean space can be written as P^T M P where M is a 4x4 matrix
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
        double Ox = O.X, Oy = O.Y, Oz = O.Z;
        double Dx = Dir.X, Dy = Dir.Y, Dz = Dir.Z;

        // calculate coefficients by simplifying matrix forms
        double a = A * Dx * Dx + B * Dy * Dy + C * Dz * Dz + D * Dx * Dy + E * Dx * Dz + F * Dy * Dz;
        double b = 2 * A * Ox * Dx + 2 * B * Oy * Dy + 2 * C * Oz * Dz +
            D * (Ox * Dy + Oy * Dx) + E * (Ox * Dz + Oz * Dx) + F * (Oy * Dz + Oz * Dy) +
            G * Dx + H * Dy + I * Dz;
        double c = A * Ox * Ox + B * Oy * Oy + C * Oz * Oz +
            D * Ox * Oy + E * Ox * Oz + F * Oy * Oz +
            G * Ox + H * Oy + I * Oz + J;

        double discriminant = b * b - 4 * a * c;
        if (discriminant < -Const.HitDiscriminant)
            return false;

        double sqrtD = Math.Sqrt(Math.Max(discriminant, 0.0));
        double t1 = (-b - sqrtD) / (2 * a);
        double t2 = (-b + sqrtD) / (2 * a);

        double t = double.PositiveInfinity;
        List<double> roots = [];
        Vec3 p;

        if (t1 >= tMin && t1 <= tMax) roots.Add(t1);
        if (t2 >= tMin && t2 <= tMax) roots.Add(t2);

        foreach (double candidate in roots)
        {
            p = localRay.At(candidate);
            if (p.X >= Xmin && p.X <= Xmax && // check bounding conditions
                p.Y >= Ymin && p.Y <= Ymax &&
                p.Z >= Zmin && p.Z <= Zmax)
            {
                t = candidate;
                break;
            }
        }

        if (t == double.PositiveInfinity)
            return false;

        p = localRay.At(t); // if ray.At(t) is used here, gradient would need to be calculated with respect to transform
        // instead for standard origin-centered quadrics it's simply: 
        // normal(x,y,z) = grad(x,y,z) = <2*A*x + D*y + E*z + G, 2*B*y + D*x + F*z + H, 2*C*z + E*x + F*y + I>
        Vec3 normal = new Vec3(
            2 * A * p.X + D * p.Y + E * p.Z + G,
            2 * B * p.Y + D * p.X + F * p.Z + H,
            2 * C * p.Z + E * p.X + F * p.Y + I
        )
        .Unit();

        hit.Point = ray.At(t);
        hit.SetFaceNormal(ray, normal);
        hit.T = t;
        hit.Color = Color;
        hit.Object = this;

        return true;
    }
}