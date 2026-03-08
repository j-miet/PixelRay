using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;

namespace PixelRay.SceneView.HitObjects.Transformed;

/// <summary>
/// Finite length cylinder with bottom and top disc/cap included.
/// Defined by bottom disc center point, axis normal, radius and height/distance to top disc. 
/// </summary>
public class CylinderT(Vec3 baseCenter, Vec3 axis, double radius, double height, ColorRGB color) : IHittable
{
    public Vec3 Center = baseCenter;
    public double Radius = radius;
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

        // TODO remove excessive comments and write a short summary of what algorithm does

        // define any point P and a cylinder by its center C, radius R and normal N
        // then cylinder can be constructed by setting a line ray with origin C and direction N and taking all points
        // which have exact distance R to the closest point on the line i.e. point projected onto axis
        // To find general intersection equation for point and cylinder, the idea is as follows:
        // 1. calculate vector V := P-C 
        // 2. use vector projection proj_N (V) to project point P on the a line perpendicular to N. 
        // 3. calculate orthogonal projection oproj_N (V)= V - proj_N (V): because projection is done with 
        // respect to N, this result vector preserves the cylinder direction and thus the radial distance of P to 
        // closest axis point
        // 4. then just check if orthogonal projection lies on the R radius circle. |oproj_N (V)| = R
        // |oproj_N (V)|^2 = | V - proj_N (V)|^2 = | V - ((Dot(V, N)/|N|^2) * V^2 | ^2
        // because N is normalized the condition to check is | V- Dot(V, N)*V |^2 = R^2
        // Now substituing the ray R(t) = O + t*D in place of P results into a lot of algebra and eventually a 
        // quadratic equation
        // however using the fact that N is normalized will simplify this a lot. Define co = O - C. 
        // End results for quadratic terms a, b and c thus become
        // a = |oproj_N (D)|^2, b = 2*Dot(oproj_N (co), oproj_N (D)), c = |oproj_N (co)|^2 - R^2
        Vec3 co = ray.Origin - Center;
        Vec3 oprojDonAxis = Vec3.Oproj(ray.Direction, _axis);
        Vec3 oprojCOonAxis = Vec3.Oproj(co, _axis);
        double a = oprojDonAxis.NormSquared();
        double b = 2 * Vec3.Dot(oprojCOonAxis, oprojDonAxis);
        double c = oprojCOonAxis.NormSquared() - Radius * Radius;

        if (a > Const.HitEpsilon) // sides
        {
            double discriminant = b * b - 4 * a * c;
            if (discriminant >= -Const.HitDiscriminant)
            {
                double sqrtD = Math.Sqrt(Math.Max(discriminant, 0.0));
                foreach (double t in new double[] { (-b - sqrtD) / (2 * a), (-b + sqrtD) / (2 * a) })
                {
                    if (t < tMin || t > tMax) continue;

                    Vec3 rayPoint = ray.At(t);
                    Vec3 centerToRay = rayPoint - Center;
                    double h = Vec3.Dot(centerToRay, _axis);
                    if (h < -Const.HitEpsilon || h > Height + Const.HitEpsilon) continue; // generate only finite cylinders

                    if (t < finalT) // important for picking only the closest ray hit amongst side + caps
                    {
                        finalT = t;
                        finalPoint = rayPoint;
                        Vec3 normal = (centerToRay - _axis * h).Unit(); // same as Vec3.Oproj(centerToRay, Axis).Unit();
                        finalNormal = Vec3.Dot(ray.Direction, normal) > Const.HitEpsilon ? -normal : normal;
                        hitAnything = true;
                    }
                }
            }
        }

        if (Vec3.Dot(_axis, ray.Direction) > Const.HitEpsilon) // bottom disc
        {
            Vec3 bottomNormal = -_axis;
            Vec3 bottomCenter = Center;
            double t = Vec3.Dot(bottomNormal, bottomCenter - ray.Origin) / Vec3.Dot(bottomNormal, ray.Direction);
            Vec3 rayPoint = ray.At(t);

            if (t >= tMin && t <= tMax && (rayPoint - bottomCenter).Norm() <= Radius + Const.HitEpsilon)
            {
                if (t < finalT)
                {
                    hitAnything = true;
                    finalT = t;
                    finalPoint = rayPoint;
                    finalNormal = bottomNormal;
                }
            }
        }

        if (Vec3.Dot(_axis, ray.Direction) < -Const.HitEpsilon) // top disc
        {
            Vec3 topNormal = _axis;
            Vec3 topCenter = Center + Height * topNormal;
            double t = Vec3.Dot(topNormal, topCenter - ray.Origin) / Vec3.Dot(topNormal, ray.Direction);
            Vec3 rayPoint = ray.At(t);

            if (t >= tMin && t <= tMax && (rayPoint - topCenter).Norm() <= Radius + Const.HitEpsilon)
            {
                if (t < finalT)
                {
                    hitAnything = true;
                    finalT = t;
                    finalPoint = rayPoint;
                    finalNormal = topNormal;
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