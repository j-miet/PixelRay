using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;

namespace PixelRay.SceneView.InstanceObject.Geometry;

/// <summary>
/// Cone with apex at origin, normal (0, 1, 0) and radius 1. Thus cone extends outward to positive y-axis from 
/// (0, 0, 0) to unit circle with center (0, 1, 0). Base cap/disc is included.
/// </summary>
public class Cone : IGeometry
{
    public Vec3 Axis = new(0, 1, 0);

    public bool Hit(Ray ray, Interval rayT, out HitRecord hit)
    {
        hit = default;

        Vec3 finalPoint = new();
        Vec3 finalNormal = new();
        double finalT = double.PositiveInfinity;
        bool hitAnything = false;

        Vec3 O = ray.Origin;
        Vec3 D = ray.Direction;

        double a = D.X * D.X + D.Z * D.Z - D.Y * D.Y;
        double b = 2 * (O.X * D.X + O.Z * D.Z - O.Y * D.Y);
        double c = O.X * O.X + O.Z * O.Z - O.Y * O.Y;

        if (!Utils.IsEqual(a, 0)) // cone sides
        {
            double discriminant = b * b - 4 * a * c;
            if (Utils.GreaterThanOrEqual(discriminant, 0))
            {
                double sqrtD = Math.Sqrt(Math.Max(discriminant, 0.0));
                foreach (double t in new double[] { (-b - sqrtD) / (2 * a), (-b + sqrtD) / (2 * a) })
                {
                    if (!rayT.InClosed(t))
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
                        finalNormal = (2 * new Vec3(rayPoint.X, -rayPoint.Y, rayPoint.Z)).Unit();
                    }
                }
            }
        }
        if (Utils.LessThan(ray.Direction.Y, 0)) // cone unit disc
        {
            double t = ray.Origin.Y / ray.Direction.Y;

            if (rayT.InClosed(t))
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

        hit.T = finalT;
        hit.Point = finalPoint;
        hit.SetFaceNormal(ray, finalNormal);
        hit.Geometry = this;

        return hitAnything;
    }
}