

using PixelRay.Core.Mathematics;

namespace PixelRay.SceneView.Lighting;

/// <summary>
/// Ambient light
/// </summary>
public class AmbientLight(ColorRGB color, double intensity = 1.0) : Light(color)
{
    public double Intensity { get; } = intensity;
}