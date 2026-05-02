namespace PixelRay.Input.Dto;

public class TransformDto
{
    public double[] Position { get; set; } = [0, 0, 0];
    // axis direction (first three) + angle (final index). Allows multiple rotations.
    public double[][] Rotation { get; set; } = [[0, 0, 0, 0]];
    public double[] Scale { get; set; } = [1, 1, 1];
}