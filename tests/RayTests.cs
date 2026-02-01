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
        Assert.True(r.Origin == origin);
        Assert.True(r.Direction == direction);
    }

    [Theory]
    [MemberData(nameof(TestAtData))]
    public void TestRayAt(Vec3 origin, Vec3 direction, double t, Vec3 rayEnd)
    {
        Ray r = new(origin, direction);
        Assert.True(r.At(t).Data == rayEnd.Data);
    }

    public static IEnumerable<object[]> TestAtData()
    {
        yield return new object[] { new Vec3(0, 0, 0), new Vec3(0, 0, 0), 1, new Vec3(0, 0, 0) };
        yield return new object[] { new Vec3(0, 0, 0), new Vec3(1, 0, 0), 2.5, new Vec3(2.5, 0, 0) };
        yield return new object[] { new Vec3(1, 1, 1), new Vec3(-1, 0, 0), 1, new Vec3(0, 1, 1) };
        yield return new object[] { new Vec3(-2, 0, 10), new Vec3(0, -2, 0), -3, new Vec3(-2, 6, 10) };
    }
}