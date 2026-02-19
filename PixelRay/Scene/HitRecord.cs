using PixelRay.Mathematics;

namespace PixelRay.SceneObjects;

/// <summary>
/// Record data of ray intersecting a hittable object.
/// </summary>
public struct HitRecord
{
    public Vec3 Point;
    public Vec3 Normal;
    public double T;
    public ColorRGB Color;
}