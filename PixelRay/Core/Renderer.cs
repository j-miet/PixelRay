using PixelRay.Lighting;
using PixelRay.Mathematics;
using PixelRay.Rendering;
using PixelRay.SceneObjects;

namespace PixelRay.Core;

/// <summary>
/// For rendering images
/// </summary>
/// <param name="width">Resolution width</param>
/// <param name="height">Resolution height></param>
/// <param name="palette">Used color palette. If you wish to use no palette, pass 'new Palette([])' instead</param>
/// <param name="lightingBands">Amount of lighting quantization levels. Default value is 1</param>
public class Renderer(int width, int height, Palette palette, int lightingBands = 1)
{
    /// <summary>
    /// Render a scene through a camera and load image into a buffer.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="camera"></param>
    /// <param name="upScale">Image upscaling factor using nearest-neighbor scaling</param>
    /// <returns></returns>
    public FrameBuffer Render(Scene scene, Camera camera, int upScale = 1)
    {
        int scaledW = upScale * _width;
        int scaledH = upScale * _height;
        FrameBuffer buffer = new(scaledW, scaledH);

        for (int y = 0; y < scaledH; y++)
        {
            for (int x = 0; x < scaledW; x++)
            {
                int baseX = (int)Math.Floor(x / (double)upScale);
                int baseY = (int)Math.Floor(y / (double)upScale);

                Ray ray = camera.GetRay(baseX, baseY);
                ColorRGB color = Trace(ray, scene);
                buffer.SetPixel(x, y, color.Clamp());
            }
        }

        return buffer;
    }

    private readonly int _width = width;
    private readonly int _height = height;
    private readonly Palette _palette = palette;
    private readonly int _lightingBands = lightingBands;

    private readonly ColorRGB _backGroundColor = new(0.1, 0.1, 0.1);

    /// <summary>
    /// Trace a ray and return its corresponding viewport pixel color.
    /// </summary>
    /// <param name="ray"></param>
    /// <param name="scene"></param>
    /// <returns></returns>
    private ColorRGB Trace(Ray ray, Scene scene)
    {
        double closest = double.MaxValue;
        bool hitAnything = false;
        HitRecord closestHit = default;

        foreach (IHittable obj in scene.Objects)
        {
            if (obj.Hit(ray, 0.001, closest, out HitRecord hit))
            {
                closest = hit.T;
                hitAnything = true;
                closestHit = hit;
            }
        }

        if (!hitAnything)
            return _backGroundColor;

        ColorRGB finalColor = new(0, 0, 0);

        foreach (Light light in scene.Lights)
        {
            if (light is DirectionalLight directionalLight)
            {
                if (!IsInShadow(closestHit, scene, directionalLight))
                {
                    double lightAngle = Math.Max(0, Vec3.Dot(closestHit.Normal, -directionalLight.Direction));
                    ColorRGB quantized = (lightAngle * closestHit.Color).Quantize(_lightingBands);
                    ColorRGB contribution = quantized * directionalLight.Color;

                    finalColor += contribution;
                    // you can add separate parameter for sum quantizing i.e. instead of individual quantize, skip this
                    // step here and call Quantize before returning value
                }
            }
        }

        if (_palette.Colors.Length == 0)
            return finalColor;
        return _palette.Map(finalColor);
    }

    /// <summary>
    /// Check if space between traced pixel and light source is blocked by any scene object.
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="scene"></param>
    /// <returns></returns>
    private static bool IsInShadow(HitRecord hit, Scene scene, DirectionalLight light)
    {
        Vec3 origin = hit.Point + hit.Normal * 0.001; // slightly nudge the normal to avoid surface self-collision
        Ray shadowRay = new(origin, -light.Direction);

        foreach (IHittable obj in scene.Objects)
        {
            if (obj.Hit(shadowRay, 0.001, double.MaxValue, out _))
                return true;
        }

        return false;
    }
}