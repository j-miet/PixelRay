using PixelRay.Input.Dto.Materials;
using PixelRay.SceneView.InstanceObject;
using PixelRay.SceneView.InstanceObject.Geometry;

namespace PixelRay.Input.Dto.Instances;

public class PlaneDto : IInstanceDto
{
    public string? Name { get; set; }

    public required IMaterialDto Material { get; set; }
    public TransformDto Transform { get; set; } = new();

    public Instance Build()
    {
        return new Instance(
            new Plane(),
            Material.Build(),
            Transform.Build()
        )
        {
            Name = Name
        };
    }
}