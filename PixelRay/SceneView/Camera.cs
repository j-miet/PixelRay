using PixelRay.Core;
using PixelRay.Core.Mathematics;

namespace PixelRay.SceneView;

/// <summary>
/// Creates a camera i.e. a collection of rays from center to viewport pixels
/// </summary>
public class Camera
{
    public Camera(
        Vec3 position,
        Vec3 lookAt,
        Vec3 upDirection,
        double fovDegrees,
        double aspectRatio
    )
    {
        _cameraOrigin = position;

        Vec3 forward = (lookAt - position).Normalize();
        Vec3 right = Vec3.Cross(forward, upDirection).Normalize();
        Vec3 up = Vec3.Cross(right, forward).Normalize(); // ensure orthogonality of axes

        double theta = fovDegrees * Math.PI / 180.0;
        double viewportHeight = 2.0 * Math.Tan(theta / 2.0); // sensor size = theta, focal length = 1
        double viewportWidth = viewportHeight * aspectRatio;

        _horizontal = right * viewportWidth;
        _vertical = up * viewportHeight;

        _topLeft = _cameraOrigin + forward - _horizontal / 2 + _vertical / 2;
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