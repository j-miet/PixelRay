using PixelRay.SceneView.InstanceObject.Materials;

namespace PixelRay.Input.Dto.Materials;

public class MirrorMaterialDto : IMaterialDto
{
    public required double[] Color { get; set; }
    public double Bounce { get; set; } = 1.0;
    public bool LinearBounce { get; set; } = false;

    public IMaterial Build()
    {
        return new MirrorMaterial(
            InputUtils.ToColor(Color),
            Bounce,
            LinearBounce
        );
    }
}