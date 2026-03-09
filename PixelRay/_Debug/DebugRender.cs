using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;
using PixelRay.SceneView.Scene;
using static PixelRay.Core.Mathematics.Const;

namespace PixelRay.Debug;

/// <summary>
/// Render debugging
/// </summary>
public static class DebugRender
{
    /// <summary>
    /// Debugging rendering via ray hits. Supports different debugging modes.<br/>
    /// Normals: visualize ray hits via normals by weighting coordinate colors: red for x, green for y, blue for z. 
    /// Missed rays generate black color.<br/>
    /// DepthHeat: visualize hits based on distance to camera. Uses e^(-x) for decay to clearly contrast change. Red 
    /// for missed rays, light colors for close objects, dark for distant objects<br/>
    /// ObjectID: visualize hits based on which object the ray hits. Thus same objects have unique coloring, missed rays
    /// generate always black.
    /// </summary>
    static public ColorRGB Debug(Ray ray, Scene scene, DebugMode mode)
    {
        double closestT = double.MaxValue;
        bool hitAnything = false;
        HitRecord closestHit = default;

        foreach (IHittable obj in scene.Objects)
        {
            if (obj.Hit(ray, HitMin, closestT, out HitRecord hit))
            {
                if (hit.T + ClosestHitEpsilon < closestT)
                {
                    hitAnything = true;
                    closestT = hit.T;
                    closestHit = hit;
                }
            }
        }

        if (!hitAnything)
        {
            return mode switch
            {
                DebugMode.Normals => new ColorRGB(0, 0, 0),
                DebugMode.DepthHeat => new ColorRGB(1, 0, 0),
                DebugMode.ObjectID => new ColorRGB(0, 0, 0),
                _ => new ColorRGB(0, 0, 0)
            };
        }

        switch (mode)
        {
            case DebugMode.Normals:
                Vec3 n = closestHit.Normal;
                // remap range [-1,1] to [0,1] for color
                return new ColorRGB(0.5 * (n.X + 1), 0.5 * (n.Y + 1), 0.5 * (n.Z + 1));

            case DebugMode.DepthHeat:
                double shade = Math.Exp(-0.5 * closestHit.T);
                return new ColorRGB(shade, shade, shade);

            case DebugMode.ObjectID:
                if (closestHit.Object != null)
                {
                    int id = closestHit.Object.GetHashCode();
                    double r = ((id >> 0) & 255) / 255.0;
                    double g = ((id >> 8) & 255) / 255.0;
                    double b = ((id >> 16) & 255) / 255.0;
                    return new ColorRGB(r, g, b);
                }
                return closestHit.Color;

            default:
                return closestHit.Color;
        }
    }
}