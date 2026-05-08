using PixelRay.SceneView.Lighting;

namespace PixelRay.Input.Dto.Lights;

public class SpotLightDto : ILightDto
{
    public required double[] Position { get; set; }
    public required double[] Direction { get; set; }
    public required double Angle { get; set; }
    public required double InnerAngle { get; set; }
    public required double[] Color { get; set; }
    public double Intensity { get; set; } = 1.0;
    public double LightRadius { get; set; } = 0;
    public int ShadowBands { get; set; } = 0;

    public ILight Build()
    {
        return new SpotLight(
            InputUtils.ToVec3(Position),
            InputUtils.ToVec3(Direction),
            Angle,
            InnerAngle,
            InputUtils.ToColor(Color),
            Intensity,
            LightRadius,
            ShadowBands
        );
    }
}