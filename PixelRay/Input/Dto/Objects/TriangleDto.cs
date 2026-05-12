using PixelRay.Input.Dto.Materials;
using PixelRay.SceneView.HitObjects;
using PixelRay.SceneView.Hittable;

namespace PixelRay.Input.Dto.Objects;

public class TriangleDto : IObjectDto
{
    public required double[] V1 { get; set; }
    public required double[] V2 { get; set; }
    public required double[] V3 { get; set; }

    public required IMaterialDto Material { get; set; }
    public TransformDto Transform { get; set; } = new();

    public IHittable Build()
    {
        return InputUtils.ToTransform(
            Transform,
            new Triangle(
                InputUtils.ToVec3(V1),
                InputUtils.ToVec3(V2),
                InputUtils.ToVec3(V3),
                Material.Build())
        );
    }
}