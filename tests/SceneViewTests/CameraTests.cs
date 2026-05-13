using PixelRay.Core.Mathematics;
using PixelRay.SceneView;

namespace PixelRay.Tests.SceneViewTests;

public class CameraTests
{
    [Fact]
    public void Camera_Basis_IsOrthogonal()
    {
        var cam = new Camera(
            new Vec3(0, 0, 0),
            new Vec3(0, 0, -1),
            new Vec3(0, 1, 0),
            90,
            1.0
        );

        double eps = 1e-10;

        // all basis vectors must be unit length
        Assert.True(Math.Abs(cam.Forward.Length() - 1.0) < eps);
        Assert.True(Math.Abs(cam.Right.Length() - 1.0) < eps);
        Assert.True(Math.Abs(cam.Up.Length() - 1.0) < eps);

        // orthogonality checks
        Assert.True(Math.Abs(Vec3.Dot(cam.Forward, cam.Right)) < eps);
        Assert.True(Math.Abs(Vec3.Dot(cam.Right, cam.Up)) < eps);
        Assert.True(Math.Abs(Vec3.Dot(cam.Up, cam.Forward)) < eps);
    }

    [Fact]
    public void Camera_CenterRay_IsForward()
    {
        var cam = new Camera(
            new Vec3(0, 0, 0),
            new Vec3(0, 0, -1),
            new Vec3(0, 1, 0),
            90,
            1.0
        );

        var ray = cam.GetRay(0, 0, 1, 1);

        // negative z-axis for right-hand coordinate system
        Assert.True(ray.Direction.Z < 0);
    }

    [Fact]
    public void Camera_RayOrigin_IsConstant()
    {
        var cam = new Camera(
            new Vec3(1, 2, 3),
            new Vec3(1, 2, 2),
            new Vec3(0, 1, 0),
            90,
            1.0
        );

        var ray = cam.GetRay(10, 10, 100, 100);

        Assert.True(CoreHelper.NearlyEqual(ray.Origin, new Vec3(1, 2, 3)));
    }

    [Fact]
    public void Camera_LeftRight_RaysDiffer()
    {
        var cam = new Camera(
            new Vec3(0, 0, 0),
            new Vec3(0, 0, -1),
            new Vec3(0, 1, 0),
            90,
            1.0
        );

        var left = cam.GetRay(0, 0, 2, 1);
        var right = cam.GetRay(1, 0, 2, 1);

        Assert.NotEqual(left.Direction.X, right.Direction.X);
    }

    [Fact]
    public void Camera_TopBottom_RaysDiffer()
    {
        var cam = new Camera(
            new Vec3(0, 0, 0),
            new Vec3(0, 0, -1),
            new Vec3(0, 1, 0),
            90,
            1.0
        );

        var top = cam.GetRay(0, 0, 1, 2);
        var bottom = cam.GetRay(0, 1, 1, 2);

        Assert.NotEqual(top.Direction.Y, bottom.Direction.Y);
    }

    [Fact]
    public void Camera_RayDirection_IsNonZero()
    {
        var cam = new Camera(
            new Vec3(0, 0, 0),
            new Vec3(0, 0, -1),
            new Vec3(0, 1, 0),
            90,
            1.0
        );

        var ray = cam.GetRay(5, 5, 10, 10);

        Assert.True(ray.Direction.Length() > 0);
    }

    [Fact]
    public void Camera_CenterSymmetry_LeftRight()
    {
        var cam = new Camera(
            new Vec3(0, 0, 0),
            new Vec3(0, 0, -1),
            new Vec3(0, 1, 0),
            90,
            1.0
        );

        var left = cam.GetRay(0, 0, 2, 1);
        var right = cam.GetRay(1, 0, 2, 1);

        // 2x1 screen, left ray should point negative, right ray to positive on x-axis
        Assert.True(left.Direction.X < 0);
        Assert.True(right.Direction.X > 0);
    }

    [Fact]
    public void Camera_CenterPixel_RayPointsForward()
    {
        var cam = new Camera(
            new Vec3(0, 0, 0),
            new Vec3(0, 0, -1),
            new Vec3(0, 1, 0),
            90,
            1.0
        );

        var ray = cam.GetRay(50, 50, 100, 100);

        // should point mostly forward
        Assert.True(ray.Direction.Z < -0.5);
    }

    [Fact]
    public void Camera_LeftRight_SymmetricAroundCenter()
    {
        var cam = new Camera(
            new Vec3(0, 0, 0),
            new Vec3(0, 0, -1),
            new Vec3(0, 1, 0),
            90,
            1.0
        );

        var left = cam.GetRay(0, 50, 100, 100);
        var right = cam.GetRay(99, 50, 100, 100);

        // symmetry means x should flip sign
        Assert.True(left.Direction.X < 0);
        Assert.True(right.Direction.X > 0);

        // x coordinates should cancel each other out
        Assert.True(Math.Abs(left.Direction.X + right.Direction.X) < 1e-4);
    }

    [Fact]
    public void Camera_TopBottom_Symmetric()
    {
        var cam = new Camera(
            new Vec3(0, 0, 0),
            new Vec3(0, 0, -1),
            new Vec3(0, 1, 0),
            90,
            1.0
        );

        var top = cam.GetRay(50, 0, 100, 100);
        var bottom = cam.GetRay(50, 99, 100, 100);

        Assert.True(Math.Abs(top.Direction.Y + bottom.Direction.Y) < 1e-4);
    }

    [Fact]
    public void Camera_AspectRatio_AffectsHorizontalSpread()
    {
        var camWide = new Camera(
            new Vec3(0, 0, 0),
            new Vec3(0, 0, -1),
            new Vec3(0, 1, 0),
            90,
            2.0 // wide screen
        );

        var camTall = new Camera(
            new Vec3(0, 0, 0),
            new Vec3(0, 0, -1),
            new Vec3(0, 1, 0),
            90,
            0.5 // tall screen
        );

        var wideLeft = camWide.GetRay(0, 50, 100, 100);
        var wideRight = camWide.GetRay(99, 50, 100, 100);

        var tallLeft = camTall.GetRay(0, 50, 100, 100);
        var tallRight = camTall.GetRay(99, 50, 100, 100);

        var wideSpan = wideRight.Direction.X - wideLeft.Direction.X;
        var tallSpan = tallRight.Direction.X - tallLeft.Direction.X;

        Assert.True(wideSpan > tallSpan);
    }

    [Fact]
    public void Camera_HorizontalSweep_IsMonotonic()
    {
        var cam = new Camera(
            new Vec3(0, 0, 0),
            new Vec3(0, 0, -1),
            new Vec3(0, 1, 0),
            90,
            1.0
        );

        double prevX = double.NegativeInfinity;

        for (int x = 0; x < 10; x++)
        {
            var ray = cam.GetRay(x, 50, 10, 10);

            Assert.True(ray.Direction.X >= prevX);

            prevX = ray.Direction.X;
        }
    }
}