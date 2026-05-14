using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;

namespace PixelRay.SceneView.InstanceObject.Materials;

/// <summary>
/// General material which can re-emit light by projections and diffusion.
/// </summary>
/// <param name="color">Material color</param>
/// <param name="reflectivity">Distribution of projections and diffusion. Values closer to 0 make direction scatter
/// randomly, values around 1 project rays w.r.t normals, producing mirror surfaces.</param>
/// <param name="roughness">Sharpness/blur i.e. strength of diffusion. 0 is sharp, 1 is very rough/diffused. Note that
/// if reflectivity is already very low, this has small/no noticeable effect.</param>
/// <param name="bounce">Distribution of light that keeps continuing after hi. 0 = absorbs all so nothing leaves, 1 = 
/// all of it bounces off. Good for making dim/darker vs detailed/colored projections with high reflectivity</param>
public class SurfaceMaterial(
    ColorRGB color,
    double reflectivity = 0.0,
    double roughness = 0.0,
    double bounce = 0.0,
    bool linearBounce = false
) : IMaterial
{
    public ColorRGB Color { get; } = color;
    public double Bounce { get; } = bounce;
    public bool LinearBounce { get; } = linearBounce;

    public double Reflectivity = reflectivity;
    public double Roughness = roughness;

    public bool Scatter(Ray rayIn, HitRecord hit, out ColorRGB attenuation, out Ray scattered)
    {
        attenuation = Color;
        double r = Random.Shared.NextDouble();

        Vec3 origin = hit.Point + hit.Normal * MathConst.RayEpsilon;

        Vec3 reflectDir = Vec3.Reflect(rayIn.Direction, hit.Normal);
        Vec3 diffuseDir = Vec3.RandomHemisphere(hit.Normal);

        Vec3 direction;

        if (r < Reflectivity)
        {
            direction = reflectDir;

            if (Roughness > 0)
                direction = Vec3.Lerp(direction, diffuseDir, Roughness).Unit();

            attenuation *= Reflectivity;
        }
        else
        {
            direction = diffuseDir;

            attenuation *= 1.0 - Reflectivity;
        }

        scattered = new Ray(origin, direction);

        return true;
    }
}