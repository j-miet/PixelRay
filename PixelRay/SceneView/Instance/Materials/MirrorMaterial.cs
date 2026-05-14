using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;

namespace PixelRay.SceneView.Materials;

public class MirrorMaterial(ColorRGB color, double bounce = 1.0, bool linearBounce = false) : IMaterial
{
    public ColorRGB Color { get; } = color;
    public double Bounce { get; } = bounce;
    public bool LinearBounce { get; } = linearBounce;

    public bool Scatter(Ray rayIn, HitRecord hit, out ColorRGB attenuation, out Ray scattered)
    {
        attenuation = Color;

        Vec3 origin = hit.Point + hit.Normal * MathConst.RayEpsilon;
        Vec3 dir = Vec3.Reflect(rayIn.Direction, hit.Normal);

        scattered = new Ray(origin, dir);

        return true;
    }
}