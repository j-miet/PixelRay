namespace PixelRay.Input.Dto;

public class CameraDto
{
    public required double[] Position { get; set; }
    public required double[] LookAt { get; set; }
    public required double[] UpDirection { get; set; }
    public double Fov { get; set; }
}