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
    /// Sample an occluded point
    /// </summary>
    public static double SampleShadowPoint(
        Scene scene,
        Vec3 point,
        Vec3 lightPos,
        double radius
    )
    {
        Vec3 toLight = lightPos - point;
        double dist = toLight.Length();
        Vec3 dir = toLight / dist;

        Ray ray = new(point + dir * MathConst.RayEpsilon, dir);
        Interval t = new(MathConst.RayEpsilon, dist);

        bool blocked = IsOccluded(scene, ray, t);
        if (radius <= 0) // only hard shadows
            return blocked ? 0.0 : 1.0;

        double lightAngle = Math.Atan(radius / dist);
        double blockerScale = 0.5; // constant blocking angle approximation, could replace

        return ComputePenumbra(blocked, lightAngle, blockerScale);
    }

    /// <summary>
    /// Analytically calculate penumbra of a shadow
    /// </summary>
    public static double ComputePenumbra(bool blocked, double lightAngle, double blockerAngle)
    {
        if (!blocked)
            return 1.0;

        // overlap ratio between blocker and light
        double overlap = blockerAngle / (lightAngle + MathConst.Epsilon);
        overlap = Math.Clamp(overlap, 0.0, 1.0);

        // more overlap = darker shadow
        double shadow = 1.0 - overlap;
        shadow = Math.Round(shadow * 4) / 4.0; // TODO add parameter to adjust levels in scenes

        return 0.15 + 0.85 * shadow; //
    }

    /// <summary>
    /// Sample a binary shadow value for directional light: 0 for no shadow , 1 for hard shadow
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

        foreach (IHittable obj in scene.Objects)
        {
            if (obj.Hit(shadowRay, rayT, out HitRecord shadow) &&
                shadow.T > MathConst.RayEpsilon && shadow.T < double.MaxValue)
            {
                return 0.0;
            }
        }

        return 1.0;
    }

    /// <summary>
    /// Check if a shadow ray hits any scene objects
    /// </summary>
    public static bool IsOccluded(Scene scene, Ray shadowRay, Interval t)
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