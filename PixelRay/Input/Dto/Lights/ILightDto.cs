using PixelRay.SceneView.Lighting;

namespace PixelRay.Input.Dto.Lights;

/// <summary>
/// Light dto template
/// </summary>
public interface ILightDto
{
    public string? Name { get; set; }

    public double[] Color { get; }
    public double Intensity { get; }

    ILight Build();
}