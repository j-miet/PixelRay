using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;

namespace PixelRay.SceneView.HitObjects;

/// <summary>
/// Cylinder with bottom cap center at origin, normal (0, 1, 0) and radius 1 i.e. bottom cap is unit circle
/// at (0, 0, 0), top cap unit circle at (0, 1, 0). Both bottom and top discs are included.
/// </summary>
public class Cylinder(ColorRGB color) : IHittable
{
    public ColorRGB Color = color;
    public Vec3 Axis = new(0, 1, 0);

    public bool Hit(Ray ray, double tMin, double tMax, out HitRecord hit)
    {
        hit = default;

        Vec3 finalPoint = new();
        Vec3 finalNormal = new();
        double finalT = double.PositiveInfinity;
        bool hitAnything = false;

        Vec3 O = ray.Origin;
        Vec3 D = ray.Direction;

        double a = D.X * D.X + D.Z * D.Z;
        double b = 2 * (O.X * D.X + O.Z * D.Z);
        double c = O.X * O.X + O.Z * O.Z - 1;

        if (!Utils.IsEqual(a, 0)) // cone sides
        {
            double discriminant = b * b - 4 * a * c;
            if (Utils.GreaterThanOrEqual(discriminant, 0))
            {
                double sqrtD = Math.Sqrt(Math.Max(discriminant, 0.0));
                foreach (double t in new double[] { (-b - sqrtD) / (2 * a), (-b + sqrtD) / (2 * a) })
                {
                    if (t < tMin || t > tMax)
                        continue;

                    Vec3 rayPoint = ray.At(t);
                    double h = rayPoint.Y;
                    if (Utils.LessThan(h, 0) || Utils.GreaterThan(h, 1))
                        continue;

                    if (t < finalT)
                    {
                        hitAnything = true;
                        finalT = t;
                        finalPoint = rayPoint;
                        finalNormal = (2 * new Vec3(rayPoint.X, 0, rayPoint.Z)).Unit();
                    }
                }
            }
        }

        if (Utils.GreaterThan(ray.Direction.Y, 0)) // bottom unit disc with center is (0, 0, 0)
        {
            double t = -ray.Origin.Y / ray.Direction.Y;

            if (t >= tMin && t <= tMax)
            {
                Vec3 bottomNormal = -Axis;
                Vec3 rayPoint = ray.At(t);
                if (Utils.LessThanOrEqual(rayPoint.Norm(), 1) && t < finalT)
                {
                    hitAnything = true;
                    finalT = t;
                    finalPoint = rayPoint;
                    finalNormal = bottomNormal;
                }
            }
        }

        if (Utils.LessThanOrEqual(ray.Direction.Y, 0)) // top cap unit disc with center at (0, 1, 0)
        {
            double t = ray.Origin.Y / ray.Direction.Y;

            if (t >= tMin && t <= tMax)
            {
                Vec3 baseNormal = Axis;
                Vec3 baseCenter = Axis;
                Vec3 rayPoint = ray.At(t);
                if (Utils.LessThanOrEqual((rayPoint - baseCenter).Norm(), 1) && t < finalT)
                {
                    hitAnything = true;
                    finalT = t;
                    finalPoint = rayPoint;
                    finalNormal = baseNormal;
                }
            }
        }

        hit.Point = finalPoint;
        hit.SetFaceNormal(ray, finalNormal);
        hit.T = finalT;
        hit.Color = Color;
        hit.Object = this;

        return hitAnything;
    }
}