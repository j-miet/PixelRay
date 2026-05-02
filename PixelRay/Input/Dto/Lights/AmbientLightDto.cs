using PixelRay.SceneView.Lighting;

namespace PixelRay.Input.Dto.Lights;

public class AmbientLightDto : ILightDto
{
    public required double[] Color { get; set; }
    public double Intensity { get; set; }

    public Light Build()
    {
        return new AmbientLight(
            InputUtils.ToColor(Color),
            Intensity
        );
    }
}