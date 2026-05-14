using PixelRay.Core;
using PixelRay.Core.Mathematics;

namespace PixelRay.SceneView.Hittable;

/// <summary>
/// Transform matrix for hittable object.
/// Excepts an affine 4x4 transform matrix<br/>
/// [ R     T ] <br/>
/// [ 0 0 0 1 ] <br/>
/// where R (upper-left) is 3x3 matrix and T (upper-right) is 3x1 transform vector. These 
/// represent linear affine 3D transforms. <br/> <br/>
/// **Important** Matrix transforms are always applied FROM RIGHT TO LEFT. Keep this in mind when applying
/// them: Scale*Translate instead of Translate*Scale is likely to yield very different results. 
/// </summary>
public struct Transform
{
    public Matrix4x4 LocalToWorld;
    public Matrix4x4 WorldToLocal;
    public Matrix4x4 NormalMatrix;

    public Transform(Matrix4x4 localToWorld)
    {
        LocalToWorld = localToWorld;
        WorldToLocal = localToWorld.InverseAffine();
        NormalMatrix = WorldToLocal.Transpose();
    }

    public readonly Ray ToLocal(Ray ray)
    {
        return WorldToLocal.TransformRay(ray);
    }

    public readonly Vec3 ToWorldPoint(Vec3 p)
    {
        return LocalToWorld.TransformPoint(p);
    }

    public readonly Vec3 ToWorldNormal(Vec3 n)
    {
        return NormalMatrix.TransformVector(n).Unit();
    }
}