using PixelRay.SceneView.Materials;

namespace PixelRay.Input.Dto.Materials;

public class FlatMaterialDto : IMaterialDto
{
    public required double[] Color { get; set; }
    public double Reflectivity { get; set; } = 0;
    public double Diffusion { get; set; } = 0;

    public IMaterial Build()
    {
        return new FlatMaterial(
            InputUtils.ToColor(Color),
            Reflectivity,
            Diffusion
        );
    }
}