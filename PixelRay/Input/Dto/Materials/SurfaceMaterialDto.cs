using PixelRay.SceneView.Materials;

namespace PixelRay.Input.Dto.Materials;

public class SurfaceMaterialDto : IMaterialDto
{
    public required double[] Color { get; set; }
    public double Bounce { get; set; } = 0.0;
    public bool LinearBounce { get; set; } = false;

    public double Reflectivity { get; set; } = 0.0;
    public double Roughness { get; set; } = 0.0;

    public IMaterial Build()
    {
        return new SurfaceMaterial(
            InputUtils.ToColor(Color),
            Reflectivity,
            Roughness,
            Bounce,
            LinearBounce
        );
    }
}