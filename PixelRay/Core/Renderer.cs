using PixelRay.Mathematics;
using PixelRay.SceneObjects;

namespace PixelRay.Core;

/// <summary>
/// </summary>
/// <param name="width">Resolution width</param>
/// <param name="height"Resolution height></param>
/// <param name="lightingBands">Amount of lighting quantization levels</param>
/// <param name="lightDirection">Fixed lighting direction</param>
public class Renderer(int width, int height, int lightingBands, Vec3 lightDirection)
{
    /// <summary>
    /// Render a scene through a camera and load image into a buffer.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="camera"></param>
    /// <returns></returns>
    public FrameBuffer Render(Scene scene, Camera camera)
    {
        FrameBuffer buffer = new(_width, _height);

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                Ray ray = camera.GetRay(x, y);
                ColorRGB color = Trace(ray, scene);
                buffer.SetPixel(x, y, color.Clamp());
            }
        }

        return buffer;
    }

    private readonly int _width = width;
    private readonly int _height = height;
    private readonly int _lightingBands = lightingBands;
    private readonly Vec3 _lightDirection = lightDirection.Unit();

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
        HitRecord closestHit = default; // give default state without initializing

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
            return new ColorRGB(0.1, 0.1, 0.15);

        if (IsInShadow(closestHit, scene))
            return closestHit.Color * 0;

        double light = Math.Max(0, Vec3.Dot(closestHit.Normal, -lightDirection)); // light intensity based on angle;
        return (closestHit.Color * light).Quantize(_lightingBands);
    }

    /// <summary>
    /// Check if space between traced pixel and light source is blocked by any scene object.
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="scene"></param>
    /// <returns></returns>
    private bool IsInShadow(HitRecord hit, Scene scene)
    {
        Vec3 origin = hit.Point + hit.Normal * 0.001; // slightly nudge the normal to avoid surface self-collision
        Ray shadowRay = new(origin, -_lightDirection);

        foreach (IHittable obj in scene.Objects)
        {
            if (obj.Hit(shadowRay, 0.001, double.MaxValue, out _))
                return true;
        }

        return false;
    }
}