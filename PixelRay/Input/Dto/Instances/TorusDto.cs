using PixelRay.Input.Dto.Materials;
using PixelRay.SceneView.HitObjects;
using PixelRay.SceneView.InstanceObject;

namespace PixelRay.Input.Dto.Instances;

public class TorusDto : IInstanceDto
{
    public string? Name { get; set; }

    public double MinorRadius { get; set; }
    public double MajorRadius { get; set; }

    public required IMaterialDto Material { get; set; }
    public TransformDto Transform { get; set; } = new();

    public Instance Build()
    {
        return new Instance(
            new Torus(
                MinorRadius,
                MajorRadius
            ),
            Material.Build(),
            Transform.Build()
        )
        {
            Name = Name
        };
    }
}