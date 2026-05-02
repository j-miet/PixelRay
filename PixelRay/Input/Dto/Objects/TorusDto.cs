using PixelRay.Input.Dto.Materials;
using PixelRay.SceneView.HitObjects;
using PixelRay.SceneView.Hittable;

namespace PixelRay.Input.Dto.Objects;

public class TorusDto : IObjectDto
{
    public double MinorRadius { get; set; }
    public double MajorRadius { get; set; }

    public required IMaterialDto Material { get; set; }
    public TransformDto Transform { get; set; } = new();

    public IHittable Build()
    {
        return InputUtils.ToTransform(
            Transform,
            new Torus(
                MinorRadius,
                MajorRadius,
                Material.Build())
        );
    }
}