using PixelRay.Core.Mathematics;
using PixelRay.Debug;
using PixelRay.Output.Progress;
using PixelRay.Rendering;
using PixelRay.SceneView;
using PixelRay.SceneView.Hittable;
using PixelRay.SceneView.Lighting;

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
/// <param name="ditherLevels">Amount of quantization levels in dithering</param>
/// <param name="ditherDimension">Bayer matrix dimension. Only 4 and 8 are supported, default is 4.</param>
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
    /// <param name="mode">What debug mode is used. Pass value as any DebugMode enum, which are: <br/>
    /// None, Normals, DepthHeat, ObjectId 
    /// </param>
    public FrameBuffer Render(
        Scene scene,
        Camera camera,
        bool threading,
        int upScale = 1,
        DebugMode mode = DebugMode.None
    )
    {
        int scaledW = upScale * _width;
        int scaledH = upScale * _height;
        FrameBuffer buffer = new(scaledW, scaledH);

        var consoleBar = new ProgressBar(30);
        var reporter = new ProgressReporter(
            _width * _height,
            consoleBar.Update
        );

        for (int y = 0; y < _height; y++)
        {
            if (threading)
            {
                Parallel.For(0, _width, x =>
                {
                    Ray ray = camera.GetRay(x, y, _width, _height);
                    ColorRGB color = mode switch
                    {
                        DebugMode.None => Trace(ray, scene),
                        _ => DebugRender.Debug(ray, scene, mode),
                    };

                    if (_useDithering)
                        color = Palette.MapDithered(color, x, y, _ditherDimension, _ditherLevels);

                    if (_palette.Colors.Length > 0) // always apply palette color last
                        color = _palette.Map(color);

                    // use nearest neighbor scaling: copy current pixel, no need to trace same pixel multiple times
                    color = color.Clamp();
                    for (int j = 0; j < upScale; j++)
                    {
                        for (int i = 0; i < upScale; i++)
                            buffer.SetPixel(upScale * x + i, upScale * y + j, color);
                    }

                    reporter.Increment();
                });
            }
            else
            {
                for (int x = 0; x < _width; x++)
                {
                    Ray ray = camera.GetRay(x, y, _width, _height);
                    ColorRGB color = mode switch
                    {
                        DebugMode.None => Trace(ray, scene),
                        _ => DebugRender.Debug(ray, scene, mode),
                    };

                    if (_useDithering)
                        color = Palette.MapDithered(color, x, y, _ditherDimension, _ditherLevels);

                    if (_palette.Colors.Length > 0)
                        color = _palette.Map(color);

                    color = color.Clamp();
                    for (int j = 0; j < upScale; j++)
                    {
                        for (int i = 0; i < upScale; i++)
                            buffer.SetPixel(upScale * x + i, upScale * y + j, color);
                    }

                    reporter.Increment();
                }
            }
        }

        return buffer;
    }

    /// <summary>
    /// Trace a single ray, returning the final pixel color
    /// </summary>
    public ColorRGB Trace(Ray ray, Scene scene, int depth = 0)
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

        if (!hitAnything || closestHit.Material is null)
            return BackGroundColor;

        ColorRGB direct = new(0, 0, 0);

        foreach (ILight light in scene.Lights)
        {
            LightContribution sample = light.Shade(scene, in closestHit);
            if (sample.Shading <= 0)
                continue;

            ColorRGB lightColor = closestHit.Material.Color * sample.Shading;
            lightColor = lightColor.Quantize(LightingBands); // quantization step before attenuation!
            direct += lightColor * light.Color * light.Intensity * sample.Attenuation;

        }

        ColorRGB indirect = new(0, 0, 0);

        if (depth < MaxBounces)
        {
            var mat = closestHit.Material;

            if (mat.Scatter(ray, closestHit, out ColorRGB attenuation, out Ray scattered))
            {
                indirect = Trace(scattered, scene, depth + 1) * attenuation;
            }

            if (mat.LinearBounce)
                return direct * (1 - mat.Bounce) + indirect * mat.Bounce;

            return direct + indirect * mat.Bounce;
        }

        return direct;
    }

    private readonly int _width = width;
    private readonly int _height = height;
    private readonly Palette _palette = palette;
    private readonly bool _useDithering = useDithering;
    private readonly int _ditherLevels = ditherLevels;
    private readonly int _ditherDimension = ditherDimension;
}