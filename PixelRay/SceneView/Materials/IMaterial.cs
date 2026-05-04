using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;

namespace PixelRay.SceneView.Materials;

public interface IMaterial
{
    // how different lighting behaves for this material. 
    // Later if emissive materials are added e.g. ColorRGB Emit(HitRecord hit), this should be removed
    public ColorRGB Shade(HitRecord hit, Scene scene, Renderer renderer, Ray ray, int depth);
    bool Scatter(Ray rayIn, HitRecord hit, out ColorRGB attenuation, out Ray scattered);

    double Bounce { get; } // amount of light bouncing off of material
}