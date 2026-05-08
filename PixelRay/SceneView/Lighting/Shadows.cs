using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;

namespace PixelRay.SceneView.Lighting;

/// <summary>
/// Shadow-processing functions
/// </summary>
public static class Shadows
{
    /// <summary>
    /// Sample a shadow color of occluded point using pre-determined disc offsets
    /// </summary>
    public static double SampleShadowPointPreset(
        Scene scene,
        Vec3 point,
        Vec3 lightPos,
        double lightRadius,
        int shadowBands
    )
    {
        // good-looking pixel-themed soft shadows still require work, but fixed offset integration will do for now
        // values larger relative to any object size will still produce very rough/diffused shadow borders

        Vec3 toLight = lightPos - point;
        double dist = toLight.Length();
        Vec3 dir = toLight / dist;

        Ray shadowRay = new(point + dir * MathConst.RayEpsilon, dir);
        Interval t = new(MathConst.RayEpsilon, dist);

        bool centerBlocked = OcclusionCheck(scene, shadowRay, t);
        if (lightRadius <= 0.0)
            return centerBlocked ? 0.0 : 1.0;

        Utils.BuildOrthonormalBasis(dir, out Vec3 u, out Vec3 v);

        int visible = 0;
        int total = offsets.Length;

        foreach (var o in offsets)
        {
            Vec3 samplePos = lightPos + u * (o.X * lightRadius) + v * (o.Y * lightRadius);

            if (!IsOccluded(scene, point, samplePos))
                visible++;
        }

        double visibility = visible / (double)total;

        // umbra and penumbra scaling + banding

        if (centerBlocked)
            visibility *= 0.9;

        visibility = Math.Pow(visibility, 1.35);

        if (shadowBands > 1)
            visibility = Math.Round(visibility * shadowBands) / shadowBands;

        return Math.Clamp(visibility, 0.0, 1.0);
    }

    /// <summary>
    /// Sample a shadow color of occluded point from a disc
    /// </summary>
    public static double SampleShadowPointDisc(
        Scene scene,
        Vec3 point,
        Vec3 lightPos,
        double lightRadius,
        int samples
    )
    {
        /// -- Currently unused --
        /// - Sampling via Monte Carlo integration is not very effective for pixel-styled shadows: too rough/diffused
        /// shadows unless samples are increased a lot (which in turn makes rendering very slow)
        /// - Should be kept here if shadows will be expanded in the future

        Vec3 toLight = lightPos - point;
        double dist = toLight.Length();
        Vec3 dir = toLight / dist;

        Ray shadowRay = new(point + dir * MathConst.RayEpsilon, dir);
        Interval t = new(MathConst.RayEpsilon, dist);

        if (lightRadius <= 0)
            return OcclusionCheck(scene, shadowRay, t) ? 0.0 : 1.0;

        // build local coordinate system for offset sampling
        Utils.BuildOrthonormalBasis(dir, out Vec3 u, out Vec3 v);

        int visible = 0;
        int sqrtSamples = (int)Math.Sqrt(samples);
        double invSqrtN = 1.0 / sqrtSamples;

        for (int y = 0; y < sqrtSamples; y++)
        {
            for (int x = 0; x < sqrtSamples; x++)
            {
                double dx = (x + Random.Shared.NextDouble()) * invSqrtN;
                double dy = (y + Random.Shared.NextDouble()) * invSqrtN;

                double r = Math.Sqrt(dx);
                double theta = 2 * Math.PI * dy;

                Vec3 disk = new(r * Math.Cos(theta), r * Math.Sin(theta), 0);

                Vec3 samplePos = lightPos + (u * disk.X + v * disk.Y) * lightRadius;

                if (!IsOccluded(scene, point, samplePos))
                    visible++;
            }
        }

        /* // pure disc sampling (no square root or loop index)
        for (int i = 0; i < samples; i++)
        {
            double r = Math.Sqrt(Random.Shared.NextDouble());
            double theta = 2.0 * Math.PI * Random.Shared.NextDouble();

            Vec3 diskSample = new(r * Math.Cos(theta), r * Math.Sin(theta), 0);

            Vec3 samplePos = lightPos + (u * diskSample.X + v * diskSample.Y) * radius;

            if (!IsOccluded(scene, point, samplePos))
                visible++;
        }
        */

        double visibility = visible / (double)samples;

        visibility = Math.Pow(visibility, 1.35);
        visibility = Math.Round(visibility * 4.0) / 4.0;

        return Math.Clamp(visibility, 0.0, 1.0);
    }

    /// <summary>
    /// Sample a binary shadow value for directional light: 0 for no shadow , 1 for hard shadow
    /// Shadow direction is FROM hit TO light
    /// </summary>
    public static double SampleShadowDirectional(
        Scene scene,
        Vec3 point,
        Vec3 direction
    )
    {
        Vec3 origin = point + direction * MathConst.RayEpsilon; // slight nudge to avoid surface self-collision
        Ray shadowRay = new(origin, direction);
        Interval rayT = new(MathConst.RayEpsilon, double.MaxValue);

        return OcclusionCheck(scene, shadowRay, rayT) ? 0.0 : 1.0;
    }

    private static readonly Vec3[] offsets =
    [
        new(-1,  0, 0),
        new( 1,  0, 0),
        new( 0, -1, 0),
        new( 0,  1, 0),
        new(-0.7, -0.7, 0),
        new( 0.7, -0.7, 0),
        new(-0.7,  0.7, 0),
        new( 0.7,  0.7, 0),
    ];

    private static bool IsOccluded(Scene scene, Vec3 point, Vec3 lightPos)
    {
        Vec3 toLight = lightPos - point;
        double dist = toLight.Length();
        Vec3 dir = toLight / dist;

        Ray shadowRay = new(point + dir * MathConst.RayEpsilon, dir);
        Interval t = new(MathConst.RayEpsilon, dist);

        return OcclusionCheck(scene, shadowRay, t);
    }

    private static bool OcclusionCheck(Scene scene, Ray shadowRay, Interval t)
    {
        foreach (IHittable obj in scene.Objects)
        {
            if (obj.Hit(shadowRay, t, out HitRecord shadow) &&
                shadow.T > MathConst.RayEpsilon && shadow.T < t.Max - MathConst.RayEpsilon)
            {
                return true;
            }
        }

        return false;
    }
}