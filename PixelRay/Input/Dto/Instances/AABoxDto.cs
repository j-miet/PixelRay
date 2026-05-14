using PixelRay.Input.Dto.Materials;
using PixelRay.SceneView.InstanceObject;
using PixelRay.SceneView.InstanceObject.Geometry;

namespace PixelRay.Input.Dto.Instances;

public class AABoxDto : IInstanceDto
{
    public string? Name { get; set; }

    public required double[] MinBounds { get; set; }
    public required double[] MaxBounds { get; set; }

    public required IMaterialDto Material { get; set; }
    public TransformDto Transform { get; set; } = new();

    public Instance Build()
    {
        return new Instance(
            new AABox(
                InputUtils.ToVec3(MinBounds),
                InputUtils.ToVec3(MaxBounds)
            ),
            Material.Build(),
            Transform.Build()
        )
        {
            Name = Name
        };
    }
}