

using PixelRay.Core.Mathematics;

namespace PixelRay.SceneView.Lighting;

/// <summary>
/// Light source which emits light evenly to all directions with specified intensity
/// </summary>
/// <param name="intensity">Use values >= 0. Default is 1</param>
public class PointLight(Vec3 position, ColorRGB color, double intensity = 1.0) : Light(color)
{
    public Vec3 Position { get; } = position;
    public double Intensity { get; } = intensity;
}