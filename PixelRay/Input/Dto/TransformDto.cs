using PixelRay.Core.Mathematics;
using PixelRay.SceneView.InstanceObject;

namespace PixelRay.Input.Dto;

public class TransformDto
{
    // axis direction (first three) + angle (final index). Allows multiple rotations.
    public double[][] Rotation { get; set; } = [[0, 0, 0, 0]];
    public double[] Translate { get; set; } = [0, 0, 0];
    public double[] Scale { get; set; } = [1, 1, 1];

    public Transform Build()
    {
        Vec3 TranslateVector = new(Translate[0], Translate[1], Translate[2]);
        Matrix4x4 translate = Matrix4x4.Translate(TranslateVector);
        Matrix4x4 scale = Matrix4x4.Scale(new Vec3(Scale[0], Scale[1], Scale[2]));
        Matrix4x4 rotation = Matrix4x4.Identity();

        // applies all rotations in order. Each rotation is a 4d vector where first 3 components matches to axis
        // direction and last is the angle [0, 360] which gets converted to radians for internal use
        foreach (var r in Rotation)
        {
            double radianAngle = InputUtils.DegreesToRadians(r[3]);
            rotation *= Matrix4x4.Rotate(new(r[0], r[1], r[2]), radianAngle);
        }

        return new Transform(translate * rotation * scale)
        {
            Position = TranslateVector
        };
    }
}