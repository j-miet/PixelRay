using PixelRay.Core.Mathematics;
using PixelRay.Input.Dto;
using PixelRay.SceneView.Hittable;

namespace PixelRay.Input;

/// <summary>
/// Input-parsing utilities
/// </summary>
public static class InputUtils
{
    // TODO add number parser for input strings to allow math constants/values and basic arithmetic operations in 
    // rotation angles e.g. "2*PI / 3" -> (double)(2*Math.PI / 3)

    /// <summary>
    /// Create a transform object from transform and object data
    /// </summary>
    public static Transform ToTransform(TransformDto dto, IHittable obj)
    {
        Matrix4x4 translate = Matrix4x4.Translate(new(dto.Position[0], dto.Position[1], dto.Position[2]));
        Matrix4x4 scale = Matrix4x4.Scale(new Vec3(dto.Scale[0], dto.Scale[1], dto.Scale[2]));
        Matrix4x4 rotation = Matrix4x4.Identity();

        // applies all rotations in order. Each rotation is a 4d vector where first 3 components matches to axis
        // direction and last is the angle [0, 2*PI]
        foreach (var r in dto.Rotation)
        {
            rotation *= Matrix4x4.Rotate(new(r[0], r[1], r[2]), r[3]);
        }

        return new Transform(obj, translate * rotation * scale);
    }

    public static Vec3 ToVec3(double[] arrayVector) => new(arrayVector[0], arrayVector[1], arrayVector[2]);
    public static ColorRGB ToColor(double[] colorArray) => new(colorArray[0], colorArray[1], colorArray[2]);
}