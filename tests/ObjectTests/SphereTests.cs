using PixelRay.Objects;

namespace PixelRay.Tests;

public class SphereTests
{
    [Theory]
    [InlineData(0, 0, -1, 0, 0)]
    [InlineData(1, 1, 2, 2, 2)]
    [InlineData(-1, 0, 0, -1.5, 0)]
    public void TestSphere(double x, double y, double z, double inputRadius, double radiusActual)
    {
        Vec3 center = new(x, y, z);
        Sphere s = new(center, inputRadius);
        Assert.Equal(s.Center.Data, (x, y, z));
        Assert.Equal(s.Radius, radiusActual);
    }

    [Theory]
    [MemberData(nameof(TestHitData))]
    public void TestHit(Ray r, double rayTMin, double rayTMax, bool expected)
    {
        HitRecorder rec = new();
        // for now test with a static sphere: origin (0, 0, -1) and radius 0.5
        // if you update this, remember to change TestHitData too
        // Tests only whether ray intersects the sphere, not the point/normal vector values
        Sphere s = new(new Vec3(0, 0, -1), 0.5);
        bool result = s.Hit(r, rayTMin, rayTMax, rec);
        Assert.True(result == expected);
    }

    public static IEnumerable<object[]> TestHitData()
    {
        yield return new object[] { new Ray(new Vec3(0, 0, 0), new Vec3(0, 0, -1)), 0, 1, true };
        yield return new object[] { new Ray(new Vec3(0, 0, 0), new Vec3(0, 0.5, -1)), 0, 5, true };
        yield return new object[] { new Ray(new Vec3(0, 0, 0), new Vec3(0, 0.6, -1)), 0, 5, false };
        yield return new object[] { new Ray(new Vec3(0, 0, 0), new Vec3(0, 0, 1)), 0, -1, false };
        yield return new object[] { new Ray(new Vec3(0, 0, 0), new Vec3(10, 10, -1)), 0, 10, false };
    }
}