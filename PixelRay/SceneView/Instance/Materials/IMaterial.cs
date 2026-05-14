using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Hittable;

namespace PixelRay.SceneView.Materials;

public interface IMaterial
{
    // how different lighting behaves for this material. 
    // Later if emissive materials are added e.g. ColorRGB Emit(HitRecord hit), this should be removed
    bool Scatter(Ray rayIn, HitRecord hit, out ColorRGB attenuation, out Ray scattered);

    public ColorRGB Color { get; }
    public double Bounce { get; } // amount of light bouncing off of material
    // if true, light is applied as {direct * (1-Bounce) + indirect * Bounce} instead of {direct + indirect}
    public bool LinearBounce { get; }
}