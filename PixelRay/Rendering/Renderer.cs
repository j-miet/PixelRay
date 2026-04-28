using PixelRay.Core.Mathematics;
using PixelRay.Debug;
using PixelRay.Rendering;
using PixelRay.SceneView;
using PixelRay.SceneView.Hittable;
using PixelRay.SceneView.Lighting;
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
    int lightingBands = 4,
    double ambientFactor = 0,
    int maxDepth = 1 // keep this very low, preferably at 1, for pixelated look
)
{
    public ColorRGB BackGroundColor = new(0, 0, 0);
    public readonly int LightingBands = lightingBands;
    public readonly double AmbientFactor = ambientFactor;
    public readonly int MaxDepth = maxDepth;

    /// <summary>
    /// Render a scene through camera and return pixel buffer of rendered image.
    /// </summary>
    /// <param name="upScale">Image upscaling factor using nearest-neighbor scaling</param>
    /// <param name="debugMode">What debug mode is used. Pass value as DebugMode enum, which are: None, BlockedShadows,
    /// Normals, DepthHeat, ObjectId 
    /// </param>
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

    /// <summary>
    /// Trace a single ray, returning the final pixel color
    /// </summary>
    public ColorRGB Trace(Ray ray, Scene scene, int depth = 0)
    {
        if (depth > MaxDepth)
            return new ColorRGB(0, 0, 0);

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

        if (!hitAnything || closestHit.Material is null)
            return BackGroundColor;

        ColorRGB shaded = closestHit.Material.Shade(closestHit, scene, this, ray, depth);

        if (_palette.Colors.Length == 0)
            return shaded;
        return _palette.Map(shaded);
    }

    /// <summary>
    /// Check if a hit point get blocked from a light source
    /// </summary>
    /// <param name="lightDirection">Direction vector from hit point to light source</param>
    /// <param name="maxDistance">Shadow ray upper bound for t</param>
    public static bool CheckShadow(
        Scene scene,
        in HitRecord hit,
        Vec3 lightDirection,
        double maxDistance = double.MaxValue
    )
    {
        Vec3 origin = hit.Point + hit.Normal * MathConst.RayEpsilon; // slight nudge to avoid surface self-collision
        Ray shadowRay = new(origin, lightDirection);
        Interval rayT = new(MathConst.RayEpsilon, maxDistance);

        foreach (IHittable obj in scene.Objects)
        {
            if (!ReferenceEquals(obj, hit.Object) && obj.Hit(shadowRay, rayT, out HitRecord shadow) &&
                shadow.T > MathConst.RayEpsilon && shadow.T < maxDistance)
            {
                return true;
            }
        }

        return false;
    }

    private readonly int _width = width;
    private readonly int _height = height;
    private readonly Palette _palette = palette;
}