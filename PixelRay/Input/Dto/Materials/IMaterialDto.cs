using PixelRay.SceneView.Materials;

namespace PixelRay.Input.Dto.Materials;

/// <summary>
/// Material dto template
/// </summary>
public interface IMaterialDto
{
    public double[] Color { get; set; }

    IMaterial Build();
}
