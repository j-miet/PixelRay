using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;

namespace PixelRay.SceneView.Materials;

public abstract class Material
{
    public abstract ColorRGB Shade(HitRecord hit, Scene scene, Renderer renderer, Ray ray, int depth);
}