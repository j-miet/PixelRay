using PixelRay.Input.Dto.Materials;
using PixelRay.SceneView.HitObjects;
using PixelRay.SceneView.Hittable;

namespace PixelRay.Input.Dto.Objects;

public class DiscDto : IObjectDto
{
    public required IMaterialDto Material { get; set; }
    public TransformDto Transform { get; set; } = new();

    public IHittable Build()
    {
        return new Instance(
            new Disc(),
            Material.Build(),
            Transform.Build()
        );
    }
}