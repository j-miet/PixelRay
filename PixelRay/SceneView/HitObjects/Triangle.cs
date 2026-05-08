using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;
using PixelRay.SceneView.Materials;

namespace PixelRay.SceneView.HitObjects;

/// <summary>
/// Triangle defined by its vertices v1, v2 and v3.
/// </summary>
public class Triangle(Vec3 v1, Vec3 v2, Vec3 v3, IMaterial material) : IHittable
{
    public Vec3 V1 = v1;
    public Vec3 V2 = v2;
    public Vec3 V3 = v3;
    public IMaterial Material = material;

    public bool Hit(Ray ray, Interval rayT, out HitRecord hit)
    {
        hit = default;

        // triangle intersection with ray without requiring the triangle plane using Möller-Trumbore algorithm:
        Vec3 e1 = V2 - V1;
        Vec3 e2 = V3 - V1;

        Vec3 rayCrossE2 = Vec3.Cross(ray.Direction, e2); // system matrix determinant written as cross product
        double determinant = Vec3.Dot(e1, rayCrossE2);

        if (Utils.IsEqual(determinant, 0)) // if ray and normal are parallel
            return false;

        Vec3 v1ToOrigin = ray.Origin - V1;

        // subdeterminants for Cramer's Rule
        // det(A_2) = Dot(-D, Cross(O-V1, e2)) = Dot(O-V1, Cross(D, e2))
        double b = Vec3.Dot(v1ToOrigin, rayCrossE2) / determinant;
        if (Utils.LessThan(b, 0) || Utils.GreaterThan(b, 1)) // check convex combination requirements
            return false;

        // det(A_3) = Dot(-D, Cross(e1, O-V1)) = Dot(D, Cross(O-V1, e1))
        Vec3 v1oCrossE1 = Vec3.Cross(v1ToOrigin, e1);
        double c = Vec3.Dot(ray.Direction, v1oCrossE1) / determinant;
        if (Utils.LessThan(c, 0) || Utils.GreaterThan(b + c, 1)) // check convex combination
            return false;

        // det(A_1) = Dot(O-V1, Cross(e1, e2)) = -Dot(e2, Cross(e1, O-V1)) = Dot(e2, Cross(O-V1, e1)).
        double t = Vec3.Dot(e2, v1oCrossE1) / determinant;
        if (!rayT.InClosed(t))
            return false;

        hit.T = t;
        hit.Point = ray.At(t);
        hit.SetFaceNormal(ray, Vec3.Cross(e1, e2).Unit());
        hit.Material = Material;
        hit.Object = this;

        return true;
    }
}