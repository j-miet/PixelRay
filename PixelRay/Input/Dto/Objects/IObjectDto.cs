using PixelRay.Input.Dto.Materials;
using PixelRay.SceneView.Hittable;

namespace PixelRay.Input.Dto.Objects;

/// <summary>
/// Object dto template
/// </summary>
public interface IObjectDto
{
    public IMaterialDto Material { get; set; }
    public TransformDto Transform { get; set; }

    IHittable Build();
}