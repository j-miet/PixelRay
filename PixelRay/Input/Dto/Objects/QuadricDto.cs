using PixelRay.Input.Dto.Materials;
using PixelRay.SceneView.HitObjects;
using PixelRay.SceneView.Hittable;

namespace PixelRay.Input.Dto.Objects;

public class QuadricDto : IObjectDto
{
    public required double[] Coefficients { get; set; }
    public required double[] MinBounds { get; set; }
    public required double[] MaxBounds { get; set; }

    public required IMaterialDto Material { get; set; }
    public TransformDto Transform { get; set; } = new();

    public IHittable Build()
    {
        if (Coefficients.Length != 10)
            throw new Exception("Quadric requires all 10 coefficients");

        double[] c = Coefficients;

        return InputUtils.ToTransform(
            Transform,
            new Quadric(
                c[0], c[1], c[2],
                c[3], c[4], c[5],
                c[6], c[7], c[8],
                c[9],
                InputUtils.ToVec3(MinBounds),
                InputUtils.ToVec3(MaxBounds),
                Material.Build())
        );
    }
}