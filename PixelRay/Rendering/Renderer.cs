using PixelRay.Core.Mathematics;
using PixelRay.Debug;
using PixelRay.Rendering;
using PixelRay.SceneView;
using PixelRay.SceneView.Hittable;
using static PixelRay.Const;

namespace PixelRay.Core;

/// <summary>
/// Image rendering class
/// </summary>
/// <param name="width">Resolution width</param>
/// <param name="height">Resolution height></param>
/// <param name="palette">Used color palette. If you wish to use no palette, pass 'new Palette([])' instead</param>
/// <param name="lightingBands">Amount of lighting quantization levels. Default is 1</param>
/// <param name="maxBounces">Max amount of ray bounces. Default is 1 and should be kept at low value for pixel look
/// </param>
/// <param name="useDithering">If ordered dithering is used or not</param>
/// <param name="ditherStrength">Only if dithering == true: how aggressive the dithering thresholds are, ranges from 0 
/// to 1</param>
/// <param name="ditherDimension">Threshold Bayer matrix dimension. Only 4 and 8 are supported, default is 4.</param>
public class Renderer(
    int width,
    int height,
    Palette palette,
    int lightingBands = 4,
    int maxBounces = 1, // keep this very low, preferably at 1, for pixelated look
    bool useDithering = false,
    int ditherLevels = 16,
    int ditherDimension = 4
)
{
    public ColorRGB BackGroundColor = new(0, 0, 0);
    public readonly int LightingBands = lightingBands;
    public readonly int MaxBounces = maxBounces;

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

                Ray ray = camera.GetRay(baseX, baseY, _width, _height);
                ColorRGB color = mode switch
                {
                    DebugMode.None => Trace(ray, scene),
                    DebugMode.BlockedShadows => BlockedShadows.TraceDebug(ray, scene),
                    _ => DebugRender.Debug(ray, scene, mode),
                };

                if (_useDithering)
                    color = Palette.MapDithered(color, x, y, _ditherDimension, _ditherLevels);

                if (_palette.Colors.Length > 0) // always apply palette color last
                    color = _palette.Map(color);

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
        if (depth > MaxBounces)
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

        return shaded;
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
    private readonly bool _useDithering = useDithering;
    private readonly int _ditherLevels = ditherLevels;
    private readonly int _ditherDimension = ditherDimension;
}