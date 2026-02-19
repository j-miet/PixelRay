using PixelRay.Core;
using PixelRay.Mathematics;

namespace PixelRay.Tests;

/// <summary>
/// Tests for Ray class
/// </summary>
public class RayTests
{
    [Fact]
    public void TestRay()
    {
        Vec3 origin = new(1, 2, 3);
        Vec3 direction = new(0, 0, 1);

        Ray r = new(origin, direction);
        Assert.True(r.Origin.X == origin.X);
        Assert.True(r.Origin.Y == origin.Y);
        Assert.True(r.Origin.Z == origin.Z);
        Assert.True(r.Direction.X == direction.X);
        Assert.True(r.Direction.Y == direction.Y);
        Assert.True(r.Direction.Z == direction.Z);
    }

    [Theory]
    [MemberData(nameof(TestAtData))]
    public void TestRayAt(Vec3 origin, Vec3 direction, double t, Vec3 rayEnd)
    {
        Ray r = new(origin, direction);
        Vec3 at = r.At(t);
        Assert.True(at.X == rayEnd.X);
        Assert.True(at.Y == rayEnd.Y);
        Assert.True(at.Z == rayEnd.Z);
    }

    public static IEnumerable<object[]> TestAtData()
    {
        yield return new object[] { new Vec3(0, 0, 0), new Vec3(0, 0, 0), 1, new Vec3(0, 0, 0) };
        yield return new object[] { new Vec3(0, 0, 0), new Vec3(1, 0, 0), 2.5, new Vec3(2.5, 0, 0) };
        yield return new object[] { new Vec3(1, 1, 1), new Vec3(-1, 0, 0), 1, new Vec3(0, 1, 1) };
        yield return new object[] { new Vec3(-2, 0, 10), new Vec3(0, -2, 0), -3, new Vec3(-2, 6, 10) };
    }
}