using PixelRay.SceneView.Lighting;

namespace PixelRay.Input.Dto.Lights;

public class DirectionalLightDto : ILightDto
{
    public required double[] Direction { get; set; }
    public required double[] Color { get; set; }

    public Light Build()
    {
        return new DirectionalLight(
            InputUtils.ToVec3(Direction),
            InputUtils.ToColor(Color)
        );
    }
}