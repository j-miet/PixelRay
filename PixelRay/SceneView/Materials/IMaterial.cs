using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;

namespace PixelRay.SceneView.Materials;

public interface IMaterial
{
    public ColorRGB Shade(HitRecord hit, Scene scene, Renderer renderer, Ray ray, int depth);
}