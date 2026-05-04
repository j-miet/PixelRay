using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;

namespace PixelRay.SceneView.Materials;

public class MirrorMaterial(ColorRGB color, double bounce = 1.0) : IMaterial
{
    public ColorRGB Color = color;
    public double Bounce { get; } = bounce;

    public bool Scatter(Ray rayIn, HitRecord hit, out ColorRGB attenuation, out Ray scattered)
    {
        attenuation = Color;

        Vec3 origin = hit.Point + hit.Normal * MathConst.RayEpsilon;
        Vec3 dir = Vec3.Reflect(rayIn.Direction, hit.Normal);

        scattered = new Ray(origin, dir);

        return true;
    }

    public ColorRGB Shade(HitRecord hit, Scene scene, Renderer renderer, Ray ray, int depth)
    {
        return Color.Quantize(renderer.LightingBands);
    }
}