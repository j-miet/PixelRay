using PixelRay.Lighting;
using PixelRay.Mathematics;
using PixelRay.Rendering;
using PixelRay.SceneObjects;
using PixelRay.Debug;

namespace PixelRay.Core;

/// <summary>
/// Image rendering class
/// </summary>
/// <param name="width">Resolution width</param>
/// <param name="height">Resolution height></param>
/// <param name="palette">Used color palette. If you wish to use no palette, pass 'new Palette([])' instead</param>
/// <param name="lightingBands">Amount of lighting quantization levels. Default is 1</param>
/// <param name="ambientFactor">Base amount of lighting in environment, takes scalar values [0, 1]. Default is 0</param>
public class Renderer(int width, int height, Palette palette, int lightingBands = 1, double ambientFactor = 0)
{
    /// <summary>
    /// Render a scene through a camera and load image into a buffer.
    /// </summary>
    /// <param name="upScale">Image upscaling factor using nearest-neighbor scaling</param>
    public FrameBuffer Render(Scene scene, Camera camera, int upScale = 1, bool debugMode = false)
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
                ColorRGB color;
                if (debugMode)
                {
                    color = BlockedShadows.TraceDebug(ray, scene);
                }
                else
                {
                    color = Trace(ray, scene);
                }
                buffer.SetPixel(x, y, color.Clamp());
            }
        }

        return buffer;
    }

    private readonly int _width = width;
    private readonly int _height = height;
    private readonly Palette _palette = palette;
    private readonly int _lightingBands = (lightingBands >= 0) ? lightingBands : 0;
    private readonly double _ambient = (ambientFactor >= 0 && ambientFactor <= 1) ? ambientFactor : 0;

    private readonly ColorRGB _backGroundColor = new(0.1, 0.1, 0.1);
    private const double _epsilon = 1e-4;
    private const double _tMin = 1e-4;

    /// <summary>
    /// Trace a ray and return its corresponding viewport pixel color.
    /// </summary>
    private ColorRGB Trace(Ray ray, Scene scene)
    {
        double closest = double.MaxValue;
        bool hitAnything = false;
        HitRecord closestHit = default;

        foreach (IHittable obj in scene.Objects)
        {
            if (obj.Hit(ray, _tMin, closest, out HitRecord hit))
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
                if (!CheckShadow(closestHit, scene, directionalLight))
                {
                    double lightAngle = Math.Max(0, Vec3.Dot(closestHit.Normal, -directionalLight.Direction));
                    double diffused = _ambient + (1 - _ambient) * lightAngle;
                    ColorRGB quantized = (diffused * closestHit.Color).Quantize(_lightingBands);
                    ColorRGB contribution = quantized * directionalLight.Color;

                    finalColor += contribution + closestHit.Color * _ambient;
                    // TODO add optional quantizing + mode of quantizing: individually for each light source or only
                    // after summing all lights
                }
            }
        }

        if (_palette.Colors.Length == 0)
            return finalColor;
        return _palette.Map(finalColor);
    }

    /// <summary>
    /// Check if space between traced pixel and a directional light ray is blocked by any scene object.
    /// </summary>
    private static bool CheckShadow(HitRecord hit, Scene scene, DirectionalLight light)
    {
        Vec3 origin = hit.Point + hit.Normal * _epsilon; // slightly nudge the normal to avoid surface self-collision
        Ray shadowRay = new(origin, -light.Direction);

        foreach (IHittable obj in scene.Objects)
        {
            if (obj.Hit(shadowRay, _tMin, double.MaxValue, out _))
                return true;
        }

        return false;
    }
}