using PixelRay.Core;
using PixelRay.Mathematics;
using PixelRay.SceneObjects;

namespace PixelRay.Geometry;

/// <summary>
/// Sphere primitive
/// </summary>
/// <param name="center"></param>
/// <param name="radius"></param>
/// <param name="color"></param>
public class Sphere(Vec3 center, double radius, ColorRGB color) : IHittable
{
    public Vec3 Center = center;
    public double Radius = radius;
    public ColorRGB Color = color;

    public bool Hit(Ray ray, double tMin, double tMax, out HitRecord hit)
    {
        hit = default;

        Vec3 oc = Center - ray.Origin;
        // coefficients of a quadratic equation produced by ray intersecting the sphere surface
        double a = Vec3.Dot(ray.Direction, ray.Direction);
        double b = -2 * Vec3.Dot(ray.Direction, oc);
        double c = Vec3.Dot(oc, oc) - Radius * Radius;

        double discriminant = b * b - 4 * a * c;
        if (discriminant < 0)
            return false;

        double sqrtDiscriminant = Math.Sqrt(discriminant);
        double root = (-b - sqrtDiscriminant) / (2 * a);

        if (root <= tMin || root >= tMax)
        {
            root = (-b + sqrtDiscriminant) / (2 * a);
            if (root <= tMin || root >= tMax)
                return false;
        }

        hit.Point = ray.At(root);
        hit.Normal = (hit.Point - Center).Unit();
        hit.T = root;
        hit.Color = Color;

        return true;
    }
}