using PixelRay.Core;
using PixelRay.Core.Mathematics;

namespace PixelRay.SceneView;

/// <summary>
/// Creates a camera i.e. a collection of rays from center to viewport pixels
/// </summary>
public class Camera
{
    // these are exposed for unit testing purposes
    public readonly Vec3 Forward;
    public readonly Vec3 Right;
    public readonly Vec3 Up;

    public Camera(
        Vec3 position,
        Vec3 lookAt,
        Vec3 upDirection,
        double fovDegrees,
        double aspectRatio
    )
    {
        _cameraOrigin = position;

        Forward = (lookAt - position).Normalize();
        Right = Vec3.Cross(Forward, upDirection).Normalize();
        Up = Vec3.Cross(Right, Forward).Normalize(); // ensure orthogonality of axes

        double theta = fovDegrees * Math.PI / 180.0;
        double viewportHeight = 2.0 * Math.Tan(theta / 2.0); // sensor size = theta, focal length = 1
        double viewportWidth = viewportHeight * aspectRatio;

        _horizontal = Right * viewportWidth;
        _vertical = Up * viewportHeight;

        _topLeft = _cameraOrigin + Forward - _horizontal / 2 + _vertical / 2;
    }

    public Ray GetRay(int x, int y, int width, int height)
    {
        double w = (x + 0.5) / width;
        double h = (y + 0.5) / height;

        Vec3 pixelCenter = _topLeft + w * _horizontal - h * _vertical;
        Vec3 rayDirection = pixelCenter - _cameraOrigin;
        return new Ray(_cameraOrigin, rayDirection);
    }

    private readonly Vec3 _cameraOrigin;
    private readonly Vec3 _horizontal;
    private readonly Vec3 _vertical;
    private readonly Vec3 _topLeft;
}