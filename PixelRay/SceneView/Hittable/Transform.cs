using PixelRay.Core;
using PixelRay.Core.Mathematics;

namespace PixelRay.SceneView.Hittable;

/// <summary>
/// Applies transform matrix to hittable object.
/// By default uses 'useAffine = true', excepting an affine transform <br/>
/// [ R     T ] <br/>
/// [ 0 0 0 1 ] <br/>
/// where R (upper-left) is 3x3 orthonormal rotation matrix and T (upper-right) is 3x1 transform vector. These 
/// represent combinations of rotation, shifting and scaling (uniform and non-uniform) thus covering majority of use 
/// cases. <br/>
/// For other linear transformations, always set 'useAffine = false' manually <br/> <br/>
/// **Final important thing** Matrix transforms are always applied FROM RIGHT TO LEFT. Keep this in mind when applying
/// them: Scale*Translate instead of Translate*Scale is likely to yield very different results. 
/// </summary>
public class Transform : IHittable
{
    public IHittable Obj;
    public Matrix4x4 TransformMatrix;
    public Matrix4x4 InverseMatrix;
    public Matrix4x4 InverseTranspose;

    public Transform(IHittable obj, Matrix4x4 transform, bool useAffine = true)
    {
        Obj = obj;
        TransformMatrix = transform;
        InverseMatrix = useAffine ? transform.InverseAffine() : transform.Inverse();
        InverseTranspose = InverseMatrix.Transpose(); // for properly inverting normals; see comment below
    }

    public bool Hit(Ray ray, double tMin, double tMax, out HitRecord hit)
    {
        Ray localRay = InverseMatrix.TransformRay(ray);

        // don't use tMin/tMax here as those from world space; instead check these conditions only after transforming 
        // is done and values are converted back to world space
        if (!Obj.Hit(localRay, Const.HitMin, double.MaxValue, out hit))
            return false;

        Vec3 worldPoint = TransformMatrix.TransformPoint(hit.Point);
        // here transpose of inverse is required instead of just inverse itself. This is because normal vectors are not 
        // exactly typical vectors: they must also preserve the perpendicularity with plane which inverting alone won't 
        // guarantee.
        // Translation, rotation and uniform scaling (= scale all components with same number) works just fine
        // without this, but others such as non-uniform scaling, shear and projections don't.
        Vec3 worldNormal = InverseTranspose.TransformVector(hit.Normal).Unit();

        double worldT = Vec3.Dot(worldPoint - ray.Origin, ray.Direction); // distance t along ray in world length
        if (worldT < tMin || worldT > tMax)
            return false;

        hit.Point = worldPoint;
        hit.T = worldT;
        hit.SetFaceNormal(ray, worldNormal);
        // hit.Color already stored in HitRecord

        return true;
    }
}

