using PixelRay.SceneView.Lighting;

namespace PixelRay.Input.Dto.Lights;

public class DirectionalLightDto : ILightDto
{
    public string? Name { get; set; }

    public required double[] Color { get; set; }
    public double Intensity { get; set; } = 1.0;

    public required double[] Direction { get; set; }

    public ILight Build()
    {
        return new DirectionalLight(
            InputUtils.ToVec3(Direction),
            InputUtils.ToColor(Color),
            Intensity
        )
        {
            Name = Name
        };
    }
}