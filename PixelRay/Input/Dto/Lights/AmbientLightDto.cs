using PixelRay.SceneView.Lighting;

namespace PixelRay.Input.Dto.Lights;

public class AmbientLightDto : ILightDto
{
    public string? Name { get; set; }

    public required double[] Color { get; set; }
    public double Intensity { get; set; } = 1.0;

    public ILight Build()
    {
        return new AmbientLight(
            InputUtils.ToColor(Color),
            Intensity
        )
        {
            Name = Name
        };
    }
}