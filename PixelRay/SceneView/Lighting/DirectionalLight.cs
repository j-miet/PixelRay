using PixelRay.Core.Mathematics;

namespace PixelRay.SceneView.Lighting;

/// <summary>
/// A directional light ray
/// </summary>
public class DirectionalLight(Vec3 direction, ColorRGB color) : Light(color)
{
    // Light direction (so -1*Direction gives direction TO light source )
    public Vec3 Direction { get; } = direction.Unit();
}