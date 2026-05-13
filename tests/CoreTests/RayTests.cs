using PixelRay.Core;
using PixelRay.Core.Mathematics;

namespace PixelRay.Tests.CoreTests;

public class RayTests
{
    [Fact]
    public void Constructor_NormalizesDirection()
    {
        var origin = new Vec3(0, 0, 0);
        var direction = new Vec3(10, 0, 0); // not normalized

        var ray = new Ray(origin, direction); // ray handles direction normalization
        var expected = new Vec3(1, 0, 0);

        Assert.True(CoreHelper.NearlyEqual(ray.Direction, expected));
    }

    [Fact]
    public void At_Zero_ReturnsOrigin()
    {
        var ray = new Ray(
            new Vec3(1, 2, 3),
            new Vec3(0, 1, 0)
        );

        var result = ray.At(0);

        Assert.True(CoreHelper.NearlyEqual(result, ray.Origin));
    }

    [Fact]
    public void At_PositiveT_MovesAlongDirection()
    {
        var ray = new Ray(
            new Vec3(0, 0, 0),
            new Vec3(0, 0, 10)
        );

        var result = ray.At(2);
        var expected = new Vec3(0, 0, 2);

        Assert.True(CoreHelper.NearlyEqual(result, expected));
    }

    [Fact]
    public void At_NegativeT_MovesOppositeDirection()
    {
        var ray = new Ray(
            new Vec3(0, 0, 0),
            new Vec3(1, 0, 0)
        );

        var result = ray.At(-3);
        var expected = new Vec3(-3, 0, 0);

        Assert.True(CoreHelper.NearlyEqual(result, expected));
    }

    [Fact]
    public void Direction_IsAlwaysUnitLength()
    {
        var ray = new Ray(
            new Vec3(0, 0, 0),
            new Vec3(3, 4, 0)
        );

        var len = ray.Direction.Length();

        Assert.True(Math.Abs(len - 1.0) < 1e-10);
    }
}