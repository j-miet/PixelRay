using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;

namespace PixelRay.SceneView.HitObjects.Transformed;

/// <summary>
/// Finite length cone with base included.
/// Defined by apex point, axis normal, radius of base and height from apex to base.
/// Important: cones with small radii might become pitch black due dot products of normal and lighting vanishing.
/// Use higher lightingbands and/or ambient values to combat this OR just use a bit larger radius.
/// </summary>
public class ConeT(Vec3 apexPoint, Vec3 axis, double baseRadius, double height, ColorRGB color) : IHittable
{
    public Vec3 Apex = apexPoint;
    public double Radius = baseRadius;
    public double Height = height;
    public ColorRGB Color = color;

    public Vec3 Axis { get => _axis; set => _axis = value.Unit(); }

    public bool Hit(Ray ray, double tMin, double tMax, out HitRecord hit)
    {
        hit = default;

        Vec3 finalPoint = new();
        Vec3 finalNormal = new();
        double finalT = double.PositiveInfinity;
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
        Vec3 oprojDonAxis = Vec3.Oproj(ray.Direction, _axis);
        Vec3 oprojAOonAxis = Vec3.Oproj(ao, _axis);
        double rDotAxis = Vec3.Dot(ray.Direction, _axis);
        double aoDotAxis = Vec3.Dot(ao, _axis);
        double tanTheta = Radius / Height;
        double tTheta2 = tanTheta * tanTheta;

        double a = oprojDonAxis.NormSquared() - tTheta2 * rDotAxis * rDotAxis;
        double b = 2 * (Vec3.Dot(oprojDonAxis, oprojAOonAxis) - tTheta2 * rDotAxis * aoDotAxis);
        double c = oprojAOonAxis.NormSquared() - tTheta2 * aoDotAxis * aoDotAxis;

        if (Math.Abs(a) > Const.HitEpsilon) // cone sides
        {
            double discriminant = b * b - 4 * a * c;
            if (discriminant >= -Const.HitDiscriminant)
            {
                double sqrtD = Math.Sqrt(Math.Max(discriminant, 0.0));
                foreach (double t in new double[] { (-b - sqrtD) / (2 * a), (-b + sqrtD) / (2 * a) })
                {
                    if (t < tMin || t > tMax) continue;

                    Vec3 rayPoint = ray.At(t);
                    Vec3 ApexToRay = rayPoint - Apex;
                    double h = Vec3.Dot(ApexToRay, _axis);
                    if (h < -Const.HitEpsilon || h > Height + Const.HitEpsilon) continue;

                    if (t < finalT)
                    {
                        hitAnything = true;
                        finalT = t;
                        finalPoint = rayPoint;
                        // cone normal
                        Vec3 oprojRayOnApex = Vec3.Oproj(ApexToRay, _axis);
                        finalNormal = (oprojRayOnApex - oprojRayOnApex.Norm() * tanTheta * _axis).Unit();
                    }
                }
            }
        }
        if (Vec3.Dot(_axis, ray.Direction) < -Const.HitEpsilon) // cone disc
        {
            Vec3 baseNormal = _axis;
            Vec3 baseCenter = Apex + Height * _axis;
            double t = Vec3.Dot(baseNormal, baseCenter - ray.Origin) / Vec3.Dot(baseNormal, ray.Direction);
            Vec3 rayPoint = ray.At(t);

            if (t >= tMin && t <= tMax && (rayPoint - baseCenter).Norm() <= Radius + Const.HitEpsilon)
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
        hit.SetFaceNormal(ray, finalNormal);
        hit.T = finalT;
        hit.Color = Color;
        hit.Object = this;

        return hitAnything;
    }

    private Vec3 _axis = axis;
}