using PixelRay.Core.Mathematics;
using PixelRay.Input.Dto;
using PixelRay.SceneView.Hittable;

namespace PixelRay.Input;

/// <summary>
/// Input-parsing utilities
/// </summary>
public static class InputUtils
{
    public static Vec3 ToVec3(double[] arrayVector) => new(arrayVector[0], arrayVector[1], arrayVector[2]);
    public static ColorRGB ToColor(double[] colorArray) => new(colorArray[0], colorArray[1], colorArray[2]);
    public static double DegreesToRadians(double rad) => rad * Math.PI / 180;
}