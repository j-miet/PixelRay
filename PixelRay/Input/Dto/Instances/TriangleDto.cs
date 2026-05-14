using PixelRay.Input.Dto.Materials;
using PixelRay.SceneView.InstanceObject;
using PixelRay.SceneView.InstanceObject.Geometry;

namespace PixelRay.Input.Dto.Instances;

public class TriangleDto : IInstanceDto
{
    public string? Name { get; set; }

    public required double[] V1 { get; set; }
    public required double[] V2 { get; set; }
    public required double[] V3 { get; set; }

    public required IMaterialDto Material { get; set; }
    public TransformDto Transform { get; set; } = new();

    public Instance Build()
    {
        return new Instance(
            new Triangle(
                InputUtils.ToVec3(V1),
                InputUtils.ToVec3(V2),
                InputUtils.ToVec3(V3)
            ),
            Material.Build(),
            Transform.Build()
        )
        {
            Name = Name
        };
    }
}