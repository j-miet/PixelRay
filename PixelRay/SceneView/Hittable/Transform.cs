using PixelRay.Core;
using PixelRay.Core.Mathematics;

namespace PixelRay.SceneView.Hittable;

/// <summary>
/// Applies transform matrix to hittable object.
/// Excepts an affine 4x4 transform matrix<br/>
/// [ R     T ] <br/>
/// [ 0 0 0 1 ] <br/>
/// where R (upper-left) is 3x3 matrix and T (upper-right) is 3x1 transform vector. These 
/// represent linear affine 3D transforms. <br/> <br/>
/// **Important** Matrix transforms are always applied FROM RIGHT TO LEFT. Keep this in mind when applying
/// them: Scale*Translate instead of Translate*Scale is likely to yield very different results. 
/// </summary>
public class Transform : IHittable
{
    public IHittable Obj;
    public Matrix4x4 TransformMatrix;
    public Matrix4x4 InverseMatrix;
    public Matrix4x4 InverseTranspose;

    public Transform(IHittable obj, Matrix4x4 transform)
    {
        Obj = obj;
        TransformMatrix = transform;
        InverseMatrix = transform.InverseAffine();
        InverseTranspose = InverseMatrix.Transpose(); // for properly inverting normals; see comment below
    }

    public bool Hit(Ray ray, Interval rayT, out HitRecord hit)
    {
        Ray localRay = InverseMatrix.TransformRay(ray);
        Interval localRayT = new(MathConst.RayEpsilon, double.MaxValue);

        // don't use tMin/tMax here as those are from world space; instead check these conditions only after 
        // transforming is done locally and values are converted back to world space
        if (!Obj.Hit(localRay, localRayT, out hit))
            return false;

        Vec3 worldPoint = TransformMatrix.TransformPoint(hit.Point);
        // here transpose of inverse is required instead of just inverse itself. This is because normal vectors are not 
        // exactly typical vectors: they must also preserve the perpendicularity with intersection plane which 
        // inverting alone won't guarantee.
        // Translation and uniform scaling (= scale all components with same number) works just fine
        // without this, but others such as rotation*, non-uniform scaling, shear and projections don't.
        // *Pure rotations, i.e. rotation matrix is orthonormal, also work because transpose of inverse equals to itself
        // for such matrices.
        Vec3 worldNormal = InverseTranspose.TransformVector(hit.Normal).Unit();

        // projecting vector P-rayOrigin onto rayDirection and taking this length gives the distance in world space
        double worldT = Vec3.Dot(worldPoint - ray.Origin, ray.Direction);
        if (!localRayT.InClosed(worldT)) // now check the range using global boundaries
            return false;

        hit.Point = worldPoint;
        hit.T = worldT;
        hit.SetFaceNormal(ray, worldNormal);
        // other data already stored inside during local hit handling

        return true;
    }
}

