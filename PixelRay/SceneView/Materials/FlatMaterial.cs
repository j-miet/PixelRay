using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;
using PixelRay.SceneView.Lighting;

namespace PixelRay.SceneView.Materials;

/// <summary>
/// Basic material which can re-emit light by reflections (projections) and diffusion (sampled/randomized)
/// Modulates reflection with base color, preventing it becoming full mirror even when value is set to 1.0
/// Also allows diffusion which randomly samples a direction. The color strength can similarly be adjusted.
/// </summary>
/// <param name="reflectivity">How much the surface reflects light. Ranges from 0 to 1, default is 0</param>
/// <param name="diffuseStrength"How much light the surface reflects diffusely. Ranges from 0 to 1, default is 0</param>
public class FlatMaterial(ColorRGB color, double reflectivity = 0.0, double diffuseStrength = 0.0) : IMaterial
{
    public ColorRGB Color = color;
    public double Reflectivity = reflectivity;

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

        if (Reflectivity > 0.0 && Reflectivity <= 1.0 && depth < renderer.MaxBounces)
        {
            Vec3 reflectDir = Vec3.Reflect(ray.Direction, hit.Normal).Unit();
            Vec3 reflectOrigin = hit.Point + hit.Normal * MathConst.RayEpsilon;

            Ray reflectRay = new(reflectOrigin, reflectDir);

            ColorRGB reflectColor = renderer.Trace(reflectRay, scene, depth + 1);

            finalColor = finalColor * (1 - Reflectivity) + reflectColor * Color * Reflectivity;
        }

        if (diffuseStrength > 0 && diffuseStrength <= 1.0 && depth < renderer.MaxBounces)
        {
            Vec3 bounceDir = Vec3.RandomHemisphere(hit.Normal);
            Vec3 bounceOrigin = hit.Point + hit.Normal * MathConst.RayEpsilon;

            Ray bounceRay = new(bounceOrigin, bounceDir);

            ColorRGB bounceColor = renderer.Trace(bounceRay, scene, depth + 1);

            finalColor += bounceColor * Color * diffuseStrength;
        }

        return finalColor;
    }
}