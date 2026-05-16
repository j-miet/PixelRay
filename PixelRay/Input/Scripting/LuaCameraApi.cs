using PixelRay.SceneView;

namespace PixelRay.Input.Scripting;

/// <summary>
/// Scene camera API
/// </summary>
public class LuaCameraApi(Camera cam)
{
    public void Position(double x, double y, double z) => _cam.Position = new(x, y, z);
    public void LookAt(double x, double y, double z) => _cam.LookAt = new(x, y, z);
    public void Up(double x, double y, double z) => _cam.UpDirection = new(x, y, z);
    public void Fov(double value) => _cam.Fov = value;
    public void AspectRatio(double value) => _cam.AspectRatio = value; // no automatic calculations here

    private readonly Camera _cam = cam;
}