using PixelRay.Core.Mathematics;
using PixelRay.Debug;
using PixelRay.Rendering;
using PixelRay.SceneView.Camera;
using PixelRay.SceneView.Hittable;
using PixelRay.SceneView.Lighting;
using PixelRay.SceneView.Scene;
using static PixelRay.Const;

namespace PixelRay.Core;

/// <summary>
/// Image rendering class
/// </summary>
/// <param name="width">Resolution width</param>
/// <param name="height">Resolution height></param>
/// <param name="palette">Used color palette. If you wish to use no palette, pass 'new Palette([])' instead</param>
/// <param name="lightingBands">Amount of lighting quantization levels. Default is 1</param>
/// <param name="ambientFactor">Base amount of lighting in environment, takes scalar values [0, 1]. Default is 0</param>
public class Renderer(
    int width,
    int height,
    Palette palette,
    int lightingBands = 1,
    double ambientFactor = 0
)
{
    public ColorRGB BackGroundColor = new(0.1, 0.1, 0.1);

    /// <summary>
    /// Render a scene through camera and return pixel buffer of rendered image.
    /// </summary>
    /// <param name="upScale">Image upscaling factor using nearest-neighbor scaling</param>
    /// <param name="debugMode">What debug mode is used. Pass value as DebugMode enum, which are: None, BlockedShadows,
    /// Normals, DepthHeat, ObjectId </
    /// param>
    public FrameBuffer Render(Scene scene, Camera camera, int upScale = 1, DebugMode mode = DebugMode.None)
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
                ColorRGB color = mode switch
                {
                    DebugMode.None => Trace(ray, scene),
                    DebugMode.BlockedShadows => BlockedShadows.TraceDebug(ray, scene),
                    _ => DebugRender.Debug(ray, scene, mode),
                };
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

    /// <summary>
    /// Trace a ray and return its corresponding viewport pixel color.
    /// </summary>
    private ColorRGB Trace(Ray ray, Scene scene)
    {
        double closestT = double.MaxValue;
        bool hitAnything = false;
        HitRecord closestHit = default;

        foreach (IHittable obj in scene.Objects)
        {
            if (obj.Hit(ray, new Interval(MathConst.RayEpsilon, closestT), out HitRecord hit))
            {
                // Only accept hits slightly closer than the current closest
                if (hit.T + MathConst.RayEpsilon < closestT)
                {
                    hitAnything = true;
                    closestT = hit.T;
                    closestHit = hit;
                }
            }
        }

        if (!hitAnything)
            return BackGroundColor;

        return ApplyLighting(scene, in closestHit);
    }

    /// <summary>
    /// Apply lighting to traced pixel
    /// </summary>
    private ColorRGB ApplyLighting(Scene scene, in HitRecord closestHit)
    {
        ColorRGB finalColor = new(0, 0, 0);

        foreach (Light light in scene.Lights)
        {
            if (light is DirectionalLight directionalLight)
            {
                if (!CheckShadow(in closestHit, scene, directionalLight))
                {
                    double lightAngle = Math.Max(0, Vec3.Dot(closestHit.Normal, -directionalLight.Direction));
                    double diffused = _ambient + (1 - _ambient) * lightAngle;
                    ColorRGB quantized = (diffused * closestHit.Color).Quantize(_lightingBands);
                    ColorRGB contribution = quantized * directionalLight.Color;

                    finalColor += contribution;
                    // TODO add optional quantizing + mode of quantizing: individually for each light source or only
                    // after summing all lights
                }
            }
        }
        finalColor += closestHit.Color * _ambient;

        if (_palette.Colors.Length == 0)
            return finalColor;
        return _palette.Map(finalColor);
    }


    /// <summary>
    /// Check if space between traced pixel and a directional light ray is blocked by any scene object.
    /// </summary>
    private static bool CheckShadow(in HitRecord hit, Scene scene, DirectionalLight light)
    {
        Vec3 origin = hit.Point + hit.Normal * MathConst.RayEpsilon; // slight nudge to avoid surface self-collision
        Ray shadowRay = new(origin, -light.Direction);
        Interval rayT = new(MathConst.RayEpsilon, double.MaxValue);

        foreach (IHittable obj in scene.Objects)
        {
            if (!ReferenceEquals(obj, hit.Object) && obj.Hit(shadowRay, rayT, out HitRecord shadow) &&
                shadow.T > MathConst.RayEpsilon)
            {
                return true;
            }
        }

        return false;
    }
}