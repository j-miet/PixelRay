using PixelRay.Core;
using PixelRay.Core.Mathematics;

namespace PixelRay.SceneView;

/// <summary>
/// Creates a camera i.e. a collection of rays from center to viewport pixels
/// </summary>
public class Camera
{
    public Vec3 Position;
    public Vec3 LookAt;
    public Vec3 UpDirection;
    public double Fov;
    public double AspectRatio;

    public Vec3 Forward;
    public Vec3 Right;
    public Vec3 Up;

    public Camera(
        Vec3 position,
        Vec3 lookAt,
        Vec3 upDirection,
        double fovDegrees,
        double aspectRatio
    )
    {
        Position = position;
        LookAt = lookAt;
        UpDirection = upDirection;
        Fov = fovDegrees;
        AspectRatio = aspectRatio;

        Rebuild();
    }

    /// <summary>
    /// Return ray pointing at pixel location (x, y)
    /// </summary>
    public Ray GetRay(int x, int y, int width, int height)
    {
        double w = (x + 0.5) / width;
        double h = (y + 0.5) / height;

        Vec3 pixelCenter = _topLeft + w * _horizontal - h * _vertical;
        Vec3 rayDirection = pixelCenter - _cameraOrigin;
        return new Ray(_cameraOrigin, rayDirection);
    }

    /// <summary>
    /// Build camera, useful for scripts
    /// </summary>
    public void Rebuild()
    {
        _cameraOrigin = Position;

        Forward = (LookAt - Position).Normalize();
        Right = Vec3.Cross(Forward, UpDirection).Normalize();
        Up = Vec3.Cross(Right, Forward).Normalize(); // ensure orthogonality of axes

        double theta = Fov * Math.PI / 180.0;
        double viewportHeight = 2.0 * Math.Tan(theta / 2.0); // sensor size = theta, focal length = 1
        double viewportWidth = viewportHeight * AspectRatio;

        _horizontal = Right * viewportWidth;
        _vertical = Up * viewportHeight;

        _topLeft = _cameraOrigin + Forward - _horizontal / 2 + _vertical / 2;
    }

    private Vec3 _cameraOrigin;
    private Vec3 _horizontal;
    private Vec3 _vertical;
    private Vec3 _topLeft;
}