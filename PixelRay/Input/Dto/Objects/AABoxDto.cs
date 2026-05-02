using PixelRay.Input.Dto.Materials;
using PixelRay.SceneView.HitObjects;
using PixelRay.SceneView.Hittable;

namespace PixelRay.Input.Dto.Objects;

public class AABoxDto : IObjectDto
{
    public required double[] MinBounds { get; set; }
    public required double[] MaxBounds { get; set; }

    public required IMaterialDto Material { get; set; }
    public TransformDto Transform { get; set; } = new();

    public IHittable Build()
    {
        return InputUtils.ToTransform(
            Transform,
            new AABox(
                InputUtils.ToVec3(MinBounds),
                InputUtils.ToVec3(MaxBounds),
                Material.Build())
        );
    }
}