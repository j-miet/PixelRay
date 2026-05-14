using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;
using PixelRay.SceneView.Instance.Geometry;

namespace PixelRay.SceneView.HitObjects;

/// <summary>
/// Origin-centered torus with normal (0, 1, 0) and specified major and minor radii
/// </summary>
public class Torus(double majorRadius, double minorRadius) : IGeometry
{
    public double MajorR = majorRadius;
    public double MinorR = minorRadius;

    public bool Hit(Ray ray, Interval rayT, out HitRecord hit)
    {
        hit = default;

        double totalRadius = MinorR + MajorR;
        if (!Utils.HitBoundingSphere(ray, totalRadius))
            return false;

        Vec3 O = ray.Origin;
        Vec3 D = ray.Direction;
        double OO = Vec3.Dot(O, O);
        double OD = Vec3.Dot(O, D);
        double W = OO - MinorR * MinorR - MajorR * MajorR;

        double a = 1;
        double b = 4 * OD;
        double c = 2 * W + 4 * OD * OD + 4 * MajorR * MajorR * D.Y * D.Y;
        double d = 4 * OD * W + 8 * MajorR * MajorR * O.Y * D.Y;
        double e = W * W - 4 * MajorR * MajorR * (MinorR * MinorR - O.Y * O.Y);

        double[] roots = Utils.SolveQuartic(a, b, c, d, e);
        double closest = double.PositiveInfinity;

        foreach (double t in roots)
        {
            if (rayT.InClosed(t) && t < closest)
                closest = t;
        }

        if (closest == double.PositiveInfinity)
            return false;

        hit.T = closest;
        hit.Point = ray.At(closest);
        hit.SetFaceNormal(ray, CalculateNormal(hit.Point).Unit());
        hit.Geometry = this;

        return true;
    }

    private Vec3 CalculateNormal(Vec3 p)
    {
        double H = p.NormSquared() + MajorR * MajorR - MinorR * MinorR;
        return new(4 * p.X * (H - 2 * MajorR * MajorR), 4 * p.Y * H, 4 * p.Z * (H - 2 * MajorR * MajorR));
    }
}