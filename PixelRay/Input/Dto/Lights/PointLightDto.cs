using PixelRay.SceneView.Lighting;

namespace PixelRay.Input.Dto.Lights;

public class PointLightDto : ILightDto
{
    public required double[] Color { get; set; }
    public double Intensity { get; set; } = 1.0;

    public required double[] Position { get; set; }
    public double LightRadius { get; set; } = 0.0;
    public int ShadowBands { get; set; } = 0;

    public ILight Build()
    {
        return new PointLight(
            InputUtils.ToVec3(Position),
            InputUtils.ToColor(Color),
            Intensity,
            LightRadius,
            ShadowBands
        );
    }
}
