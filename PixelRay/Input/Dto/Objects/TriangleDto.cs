using PixelRay.Input.Dto.Materials;
using PixelRay.SceneView.HitObjects;
using PixelRay.SceneView.Hittable;

namespace PixelRay.Input.Dto.Objects;

public class TriangleDto : IObjectDto
{
    public required double[] Vertex1 { get; set; }
    public required double[] Vertex2 { get; set; }
    public required double[] Vertex3 { get; set; }

    public required IMaterialDto Material { get; set; }
    public TransformDto Transform { get; set; } = new();

    public IHittable Build()
    {
        return InputUtils.ToTransform(
            Transform,
            new Triangle(
                InputUtils.ToVec3(Vertex1),
                InputUtils.ToVec3(Vertex2),
                InputUtils.ToVec3(Vertex3),
                Material.Build())
        );
    }
}