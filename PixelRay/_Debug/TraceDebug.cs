using PixelRay.Core;
using PixelRay.Lighting;
using PixelRay.Mathematics;
using PixelRay.SceneObjects;

namespace PixelRay.Debug;

/// <summary>
/// Simple tracing debugger for detecting shadow pixels
/// </summary>
public static class TraceDebug
{
    private static readonly double _epsilon = 1e-4;
    private static readonly ColorRGB _backGroundColor = new(0.1, 0.1, 0.1);

    public static ColorRGB RunTraceDebug(Ray ray, Scene scene)
    {
        double closest = double.MaxValue;
        HitRecord closestHit = default;
        bool hitAnything = false;

        foreach (IHittable obj in scene.Objects)
        {
            if (obj.Hit(ray, _epsilon, closest, out HitRecord hit))
            {
                closest = hit.T;
                hitAnything = true;
                closestHit = hit;
            }
        }

        if (!hitAnything)
            return _backGroundColor;

        foreach (Light light in scene.Lights)
        {
            if (light is DirectionalLight dirLight)
            {
                Vec3 origin = closestHit.Point + closestHit.Normal * _epsilon;
                Ray shadowRay = new(origin, -dirLight.Direction);

                bool blocked = false;
                foreach (var obj in scene.Objects)
                {
                    if (obj.Hit(shadowRay, _epsilon, double.MaxValue, out _))
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

        return new ColorRGB(0.2, 0.2, 0.2); // ambient fallback
    }
}