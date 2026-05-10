using PixelRay.SceneView.Materials;

namespace PixelRay.Input.Dto.Materials;

/// <summary>
/// Material dto template
/// </summary>
public interface IMaterialDto
{
    public double[] Color { get; set; }
    public double Bounce { get; set; }
    public bool LinearBounce { get; set; }

    IMaterial Build();
}
