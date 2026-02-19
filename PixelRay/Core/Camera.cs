using PixelRay.Mathematics;

namespace PixelRay.Core;

public class Camera
{
    public Camera(Vec3 position, double width, double height)
    {
        _cameraCenter = position;

        double aspectRatio = width / height;
        double viewportHeight = 2;
        double viewportWidth = viewportHeight * aspectRatio;
        double focalLength = 1;

        Vec3 horizontal = new(viewportWidth, 0, 0);
        Vec3 vertical = new(0, -viewportHeight, 0);

        _horizontalDelta = horizontal / width;
        _verticalDelta = vertical / height;

        // screen center coordinate is center - (0, 0, focalLength)
        Vec3 viewportTopLeft = _cameraCenter - new Vec3(0, 0, focalLength) - horizontal / 2 - vertical / 2;
        _topLeft = viewportTopLeft + 0.5 * (_horizontalDelta - _verticalDelta);
    }

    public Ray GetRay(int x, int y)
    {
        Vec3 pixelCenter = _topLeft + x * _horizontalDelta + y * _verticalDelta;
        Vec3 rayDirection = pixelCenter - _cameraCenter;
        return new Ray(_cameraCenter, rayDirection);
    }

    private readonly Vec3 _cameraCenter;
    private readonly Vec3 _horizontalDelta;
    private readonly Vec3 _verticalDelta;
    private readonly Vec3 _topLeft;
}