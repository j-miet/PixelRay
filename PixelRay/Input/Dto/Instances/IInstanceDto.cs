using PixelRay.Input.Dto.Materials;
using PixelRay.SceneView.InstanceObject;

namespace PixelRay.Input.Dto.Instances;

/// <summary>
/// Object dto template
/// </summary>
public interface IInstanceDto
{
    public string? Name { get; set; }

    public IMaterialDto Material { get; set; }
    public TransformDto Transform { get; set; }

    Instance Build();
}