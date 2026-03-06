using PixelRay.Core;
using PixelRay.Core.Mathematics;

namespace PixelRay.SceneView.Hittable;

/// <summary>
/// Applies transform matrix to hittable object.
/// By default uses 'useAffine = true', excepting an affine transform <br />
/// [ R     T ] <br />
/// [ 0 0 0 1 ] <br />
/// where R (upper-left) is 3x3 orthonormal rotation matrix and T (upper-right) is 3x1 transform vector. These 
/// represent combinations of rotation, shifting and scaling thus covering majority of use cases. <br />
/// For general linear transformation, always set 'useAffine = false' manually
/// </summary>
public class Transform : IHittable
{
    public IHittable Obj;
    public Matrix4x4 TransformMatrix;
    public readonly bool Affine;
    public Matrix4x4 InverseMatrix;
    public Matrix4x4 InverseTranspose;

    public Transform(IHittable obj, Matrix4x4 transform, bool useAffine = true)
    {
        Obj = obj;
        TransformMatrix = transform;
        Affine = useAffine;
        if (Affine)
        {
            InverseMatrix = transform.InverseAffine();
        }
        else
        {
            InverseMatrix = transform.Inverse();
        }
        InverseTranspose = InverseMatrix.Transpose(); // for properly inverting normals; see comment below
    }

    public bool Hit(Ray ray, double tMin, double tMax, out HitRecord hit)
    {
        hit = default;

        Ray localRay = InverseMatrix.TransformRay(ray);

        if (!Obj.Hit(localRay, tMin, tMax, out HitRecord localHit))
            return false;

        // here transpose of inverse is required instead of just inverse itself. This is because normal vectors are not 
        // exactly typical vectors: they must also preserve the perpendicularity with plane which inverting alone won't 
        // guarantee.
        // Translation, rotation and uniform scaling (= scale all components with same number) works just fine
        // without this, but others such as shear and projections don't.
        Vec3 worldNormal = InverseTranspose.TransformVector(localHit.Normal).Unit();

        if (Vec3.Dot(worldNormal, ray.Direction) > 0)
            worldNormal = -worldNormal;

        hit.Point = TransformMatrix.TransformPoint(localHit.Point);
        hit.Normal = worldNormal;
        hit.T = localHit.T;
        hit.Color = localHit.Color;

        return true;
    }
}

