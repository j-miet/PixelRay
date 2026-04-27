using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;
using PixelRay.SceneView.Lighting;

namespace PixelRay.SceneView.Materials;

/// <summary>
/// Material that reflects light to a single direction with respect to surface normal (i.e. standard vector projection)
/// </summary>
/// <param name="color"></param>
/// <param name="reflectivity"></param>
public class FlatMaterial(ColorRGB color, double reflectivity = 0.0) : Material
{
    public ColorRGB Color = color;
    public double Reflectivity = reflectivity;
    // add ambient lighting as a custom attribute?

    public override ColorRGB Shade(HitRecord hit, Scene scene, Renderer renderer, Ray ray, int depth)
    {
        ColorRGB finalColor = new(0, 0, 0);

        foreach (Light light in scene.Lights)
        {
            if (light is DirectionalLight directionalLight)
            {
                if (!Renderer.CheckShadow(scene, in hit, directionalLight))
                {
                    double lightAngle = Math.Max(0, Vec3.Dot(hit.Normal, -directionalLight.Direction));
                    double diffused = renderer.AmbientFactor + (1 - renderer.AmbientFactor) * lightAngle;
                    ColorRGB quantized = (diffused * Color).Quantize(renderer.LightingBands);
                    ColorRGB contribution = quantized * directionalLight.Color;

                    finalColor += contribution;
                }
            }
        }

        finalColor += Color * renderer.AmbientFactor;

        if (Reflectivity > 0.0 && depth < renderer.MaxDepth)
        {
            Vec3 reflectDir = Vec3.Reflect(ray.Direction, hit.Normal).Unit();
            Vec3 reflectOrigin = hit.Point + hit.Normal * MathConst.RayEpsilon;

            Ray reflectRay = new(reflectOrigin, reflectDir);

            ColorRGB reflectColor = renderer.Trace(reflectRay, scene, depth + 1);

            finalColor = finalColor * (1 - Reflectivity) + reflectColor * Reflectivity;
        }

        return finalColor;
    }
}