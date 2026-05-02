using PixelRay.SceneView.Lighting;

namespace PixelRay.Input.Dto.Lights;

public class PointLightDto : ILightDto
{
    public required double[] Position { get; set; }
    public required double[] Color { get; set; }
    public double Intensity { get; set; }

    public Light Build()
    {
        return new PointLight(
            InputUtils.ToVec3(Position),
            InputUtils.ToColor(Color),
            Intensity
        );
    }
}
