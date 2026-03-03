using PixelRay.Core;
using PixelRay.Mathematics;
using PixelRay.SceneObjects;

namespace PixelRay.Geometry;

/// <summary>
/// Finite length cone with base included.
/// Defined by apex point, axis normal, radius of base and height from apex to base.
/// Important: cones with small radii might become pitch black due dot products of normal and lighting vanishing.
/// Use higher lightingbands and/or ambient values to combat this OR just use a bit larger radius.
/// </summary>
public class Cone(Vec3 apexPoint, Vec3 axis, double baseRadius, double height, ColorRGB color) : IHittable
{
    public Vec3 Apex = apexPoint;
    public double Radius = baseRadius;
    public double Height = height;
    public Vec3 Axis = axis.Unit();
    public ColorRGB Color = color;

    public bool Hit(Ray ray, double tMin, double tMax, out HitRecord hit)
    {
        hit = default;
        double epsilon = 1e-8;

        Vec3 finalPoint = new();
        Vec3 finalNormal = new();
        double finalT = double.MaxValue;
        bool hitAnything = false;

        // Cone can be constructed by defining its head/apex point A, radius of base R and axis unit normal N.
        // if P is any point then checking if it lies on cone surface can be done by checking the radial cone distance.
        // this distance is P-A and it's the hypotenuse of a right triangle with height being some scaling of N and
        // base the length of orthogonal projection of P-A on this scaled normal
        // Then if the angle between scaler normal and P-A is theta, one can formulate the identity
        // sin(theta) = cos(theta)*tan(theta) <=> |oproj_N (P-A)| = Dot(P-A, N)*tan(theta)
        // because
        // 1. sin(theta) = |P-A| * |oproj_N (P-A)|
        // 2. cos(theta) = Dot(P-A, N) / (|P-A| * |N|) = Dot(P-A, N) / |P-A| because N is normalized
        // 3. tan(theta) = sin(theta)/cos(theta) -> term |P-A| gets cancelled out when multiplied by cos(theta) != 0
        // Also: theta = arctan(r/h) where r is base radius, h distance from apex to base center
        // Squaring this and substituting ray in place of P yields quite a bit of algebra and yields a quadratic with
        // coefficients. Here D = ray direction, O = ray origin, ao = O - A
        // a = |oproj_N (D)|^2 - Dot(d, N)^2 * tan^2(theta)
        // b = 2 * (Dot(oproj_N (D), oproj_N (ao)) - Dot(D, N) * Dot(ao, N) * tan^2(theta))
        // c = |oproj_N (ao)|^2 - Dot(ao, N)^2 * tan^2(theta)
        Vec3 ao = ray.Origin - Apex;
        Vec3 oprojDonAxis = Vec3.Oproj(ray.Direction, Axis);
        Vec3 oprojAOonAxis = Vec3.Oproj(ao, Axis);
        double rDotAxis = Vec3.Dot(ray.Direction, Axis);
        double aoDotAxis = Vec3.Dot(ao, Axis);
        double tanTheta = Radius / Height;
        double tTheta2 = tanTheta * tanTheta;

        double a = oprojDonAxis.NormSquared() - tTheta2 * rDotAxis * rDotAxis;
        double b = 2 * (Vec3.Dot(oprojDonAxis, oprojAOonAxis) - tTheta2 * rDotAxis * aoDotAxis);
        double c = oprojAOonAxis.NormSquared() - tTheta2 * aoDotAxis * aoDotAxis;

        if (Math.Abs(a) > epsilon) // cone sides
        {
            double discriminant = b * b - 4 * a * c;
            if (discriminant >= 0 - epsilon)
            {
                double sqrtD = Math.Sqrt(discriminant);
                foreach (double t in new double[] { (-b - sqrtD) / (2 * a), (-b + sqrtD) / (2 * a) })
                {
                    if (t <= tMin || t >= tMax) continue;

                    Vec3 rayPoint = ray.At(t);
                    Vec3 ApexToRay = rayPoint - Apex;
                    double h = Vec3.Dot(ApexToRay, Axis);
                    if (h < 0 || h > Height) continue;

                    if (t < finalT)
                    {
                        hitAnything = true;
                        finalT = t;
                        finalPoint = rayPoint;
                        // cone normal
                        Vec3 oprojRayOnApex = Vec3.Oproj(ApexToRay, Axis);
                        finalNormal = (oprojRayOnApex - oprojRayOnApex.Norm() * tanTheta * Axis).Unit();
                    }
                }
            }
        }

        if (Vec3.Dot(Axis, ray.Direction) <= -epsilon) // cone disc
        {
            Vec3 baseNormal = Axis;
            Vec3 baseCenter = Apex + Height * Axis;
            double t = Vec3.Dot(baseNormal, baseCenter - ray.Origin) / Vec3.Dot(baseNormal, ray.Direction);
            Vec3 rayPoint = ray.At(t);

            if (t >= tMin && t <= tMax && (rayPoint - baseCenter).Norm() <= Radius)
            {
                if (t < finalT)
                {
                    hitAnything = true;
                    finalT = t;
                    finalPoint = rayPoint;
                    finalNormal = baseNormal;
                }
            }
        }


        hit.Point = finalPoint;
        hit.Normal = finalNormal;
        hit.T = finalT;
        hit.Color = Color;

        return hitAnything;
    }
}