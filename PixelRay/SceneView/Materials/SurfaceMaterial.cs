using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;
using PixelRay.SceneView.Lighting;

namespace PixelRay.SceneView.Materials;

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
    double bounce = 0.0
) : IMaterial
{
    public ColorRGB Color = color;
    public double Reflectivity = reflectivity;
    public double Roughness = roughness;
    public double Bounce { get; } = bounce;

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

    public ColorRGB Shade(HitRecord hit, Scene scene, Renderer renderer, Ray ray, int depth)
    {
        ColorRGB finalColor = new(0, 0, 0);

        foreach (Light light in scene.Lights)
        {
            if (light is AmbientLight ambient)
            {
                finalColor += Color * ambient.Color * ambient.Intensity;
            }
            else if (light is DirectionalLight directionalLight)
            {
                Vec3 lightDir = directionalLight.Direction.Unit();

                if (!Renderer.CheckShadow(scene, in hit, lightDir))
                {
                    double NdotL = Math.Max(0, Vec3.Dot(hit.Normal, lightDir));
                    ColorRGB quantized = (NdotL * Color).Quantize(renderer.LightingBands);
                    ColorRGB contribution = quantized * directionalLight.Color;

                    finalColor += contribution;
                }
            }
            else if (light is PointLight pointLight)
            {
                Vec3 toLight = pointLight.Position - hit.Point;
                double distance = toLight.Length();
                Vec3 lightDir = toLight / distance; // must be unit vector!

                if (!Renderer.CheckShadow(scene, in hit, lightDir, distance))
                {
                    // this will reduce light quickly but smoothly, making it look natural
                    double attenuation = pointLight.Intensity / (distance * distance);

                    double NdotL = Math.Max(0, Vec3.Dot(hit.Normal, lightDir));
                    ColorRGB quantized = (NdotL * Color).Quantize(renderer.LightingBands);
                    ColorRGB contribution = quantized * pointLight.Color * attenuation;

                    finalColor += contribution;
                }
            }
        }

        return finalColor;
    }
}