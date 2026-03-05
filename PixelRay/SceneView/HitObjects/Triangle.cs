using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;

namespace PixelRay.SceneView.HitObjects;

/// <summary>
/// Triangle defined by its vertices v1, v2 and v3.
/// </summary>
public class Triangle(Vec3 v1, Vec3 v2, Vec3 v3, ColorRGB color) : IHittable
{
    public Vec3 V1 = v1;
    public Vec3 V2 = v2;
    public Vec3 V3 = v3;
    public ColorRGB Color = color;

    public bool Hit(Ray ray, double tMin, double tMax, out HitRecord hit)
    {
        hit = default;
        double epsilon = 1e-4;

        // triangle intersection with ray without requiring the triangle plane using Möller-Trumbore algorithm:
        // point P on a triangle can be represented as convex combination
        // P = aV1 + bV2 + cV3 where a, b, c >= 0, a + b + c = 1
        // Furthermore substituting V1 = 1 - V2 - V3 yields
        // P = V1 + b(V2-V1) + c(V3-V1)
        // Here V2-V1 and V3-V1 are triangle's edges starting from V1. Plugging ray P(t) = O + tD in place of P and
        // marking edges as e1 = V2-V1, e2 = V3-V1 gives 
        // the final equation becomes
        // O - V1 = -tD + b*e1 + c*e2 
        // which can be described using a linear equation system
        // [ -D  e1  e2 ] [t b c]^T = O - V1 (here T is transpose so [t b c]^T is just column vector)
        Vec3 e1 = V2 - V1;
        Vec3 e2 = V3 - V1;

        // linear system can be solved using Cramer's Rule: this requires calculating some determinants for matrices
        // A neat trick is to use 'scalar triple product' to calculate determinant:
        // 1. for vectors w1, w2, w3 -> | w1  w2  w3 | = Dot(w1, Cross(w2, w3))
        // 2. triple product follows circularity: 
        //  Dot(w1, Cross(w2, w3)) = Dot(w3, Cross(w1, w2)) = Dot(w2, Cross(w3, w1))
        // 3. swapping any 2 operands negates triple product: for example 
        //  Dot(w1, Cross(w2, w3)) = -Dot(w1, Cross(w3, w2))
        // Using all of this together:
        // | -D  e1  e2 | = Dot(-D, Cross(e1, e2)) = -Dot(D, Cross(e1, e2)) = Dot(e1, Cross(D, e2))
        // This gives both determinant and the angle cosine of ray and one of the edges  
        Vec3 rayCrossE2 = Vec3.Cross(ray.Direction, e2); // system matrix determinant
        double determinant = Vec3.Dot(e1, rayCrossE2);

        if (Math.Abs(determinant) < epsilon) // if ray and normal are parallel
            return false;

        // to solve linear system using Cramer's Rule, subdeterminants are required. These are calculated by replacing
        // matrix column of a corresponding unknown by right-side vector O - V1.
        // so if 
        // | -D  e1  e2 | =: det(A) (main determinant) 
        // |  O-V1  e1  e2 | =: det(A_1) (corresponds to t)
        // |  -D  O-V1  e2 | =: det(A_2) (corresponds to b)
        // |  -D  e1  O-V1 | =: det(A_3) (corresponds to c)
        // then solutions are easy to derive as det(A_i)/det(A), i = 1, 2, 3

        Vec3 v1ToOrigin = ray.Origin - V1;

        // det(A_2) = Dot(-D, Cross(O-V1, e2)) = Dot(O-V1, Cross(D, e2))
        double b = Vec3.Dot(v1ToOrigin, rayCrossE2) / determinant;
        if (b < 0 || b > 1) // check convex combination requirements
            return false;

        // det(A_3) = Dot(-D, Cross(e1, O-V1)) = Dot(D, Cross(O-V1, e1))
        Vec3 v1oCrossE1 = Vec3.Cross(v1ToOrigin, e1);
        double c = Vec3.Dot(ray.Direction, v1oCrossE1) / determinant;
        if (c < 0 || b + c > 1) // check convex combination reqs
            return false;

        // det(A_1) = Dot(O-V1, Cross(e1, e2)) = -Dot(e2, Cross(e1, O-V1)) = Dot(e2, Cross(O-V1, e1)).
        double t = Vec3.Dot(e2, v1oCrossE1) / determinant;
        if (t <= tMin || t >= tMax)
            return false;

        hit.Point = ray.At(t);
        Vec3 normal = Vec3.Cross(e1, e2).Unit();
        hit.Normal = Vec3.Dot(ray.Direction, normal) > 0 ? -normal : normal;
        hit.T = t;
        hit.Color = Color;

        return true;
    }
}