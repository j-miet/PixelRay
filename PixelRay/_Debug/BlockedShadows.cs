using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;
using PixelRay.SceneView.Lighting;
using PixelRay.SceneView.Scene;

namespace PixelRay.Debug;

/// <summary>
/// Simple tracing debugger for detecting blocked shadow pixels i.e. pixels which are both hit by a light ray AND 
/// blocked by another object.
/// Pixels blocked by some object show as red, anything else as green.
/// </summary>
public static class BlockedShadows
{
    private static readonly ColorRGB _backGroundColor = new(0.1, 0.1, 0.1);

    public static ColorRGB TraceDebug(Ray ray, Scene scene)
    {
        double closestT = double.MaxValue;
        HitRecord closestHit = default;
        bool hitAnything = false;

        foreach (IHittable obj in scene.Objects)
        {
            if (obj.Hit(ray, MathConst.RayEpsilon, closestT, out HitRecord hit))
            {
                if (hit.T + MathConst.RayEpsilon < closestT)
                {
                    hitAnything = true;
                    closestT = hit.T;
                    closestHit = hit;
                }
            }
        }

        if (!hitAnything)
            return _backGroundColor;

        foreach (Light light in scene.Lights)
        {
            if (light is DirectionalLight dirLight)
            {
                Vec3 origin = closestHit.Point + closestHit.Normal * MathConst.RayEpsilon;
                Ray shadowRay = new(origin, -dirLight.Direction);

                bool blocked = false;
                foreach (var obj in scene.Objects)
                {
                    if (obj.Hit(shadowRay, MathConst.RayEpsilon, double.MaxValue, out _))
                    {
                        blocked = true;
                    }
                }

                if (blocked)
                {
                    return new ColorRGB(1, 0, 0);
                }
                else
                {
                    return new ColorRGB(0, 1, 0);
                }
            }
        }

        return new ColorRGB(0.2, 0.2, 0.2); // default ambient color
    }
}