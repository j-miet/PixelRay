using PixelRay.Mathematics;

namespace PixelRay.Lighting;

/// <summary>
/// Object resembling a directional light beam
/// </summary>
public class DirectionalLight(Vec3 direction, ColorRGB color) : Light(color)
{
    public Vec3 Direction { get; } = direction.Unit();
}