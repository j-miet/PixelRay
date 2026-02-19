using PixelRay.Mathematics;

namespace PixelRay.Tests;

/// <summary>
/// Tests for Vec3 class
/// </summary>
public class Vec3Tests
{
    [Fact]
    public void TestVec3Default()
    {
        Vec3 v = new();

        Assert.Equal(0, v.X);
        Assert.Equal(0, v.Y);
        Assert.Equal(0, v.Z);
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1, 2, 3)]
    [InlineData(2.0, 1.0, -2)]
    public void TestVec3(double x, double y, double z)
    {
        Vec3 v = new(x, y, z);

        Assert.Equal(x, v.X);
        Assert.Equal(y, v.Y);
        Assert.Equal(z, v.Z);
    }

    [Fact]
    public void TestToString()
    {
        Vec3 v = new(-1, 2, 1.2);

        Assert.True(v.ToString() == "(" + v.X + ", " + v.Y + ", " + v.Z + ")");
    }

    [Theory]
    [MemberData(nameof(TestLengthData))]
    public void TestLengthSquared(double x, double y, double z, double value)
    {
        double error = 10e-14;
        Vec3 v = new(x, y, z);

        Assert.True(v.LengthSquared() >= 0);
        Assert.True(Math.Abs(v.LengthSquared() - value * value) < error);
    }

    [Theory]
    [MemberData(nameof(TestLengthData))]
    public void TestLength(double x, double y, double z, double value)
    {
        Vec3 v = new(x, y, z);

        Assert.True(v.Norm() >= 0);
        Assert.Equal(value, v.Norm());
    }

    [Fact]
    public void TestVectorIndex()
    {
        Vec3 v = new(1, 2, 3);

        Assert.Equal(v.X, v[0]);
        Assert.Equal(v.Y, v[1]);
        Assert.Equal(v.Z, v[2]);
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1, 0, 2)]
    [InlineData(-1, -2, -1)]
    [InlineData(-1, 0.5, 0)]
    public void TestVectorInverse(double x, double y, double z)
    {
        Vec3 v = new(x, y, z);
        Vec3 w = -v;
        Assert.Equal(-x, w.X);
        Assert.Equal(-y, w.Y);
        Assert.Equal(-z, w.Z);
    }

    [Theory]
    [MemberData(nameof(TestVectorAndVectorOperationsData))]
    public void TestVectorSubtract(Vec3 v, Vec3 w)
    {
        Vec3 sub = v - w;
        Assert.Equal(v.X - w.X, sub.X);
        Assert.Equal(v.Y - w.Y, sub.Y);
        Assert.Equal(v.Z - w.Z, sub.Z);
    }

    [Theory]
    [MemberData(nameof(TestVectorAndVectorOperationsData))]
    public void TestVectorSum(Vec3 v, Vec3 w)
    {
        Vec3 sum = v + w;
        Assert.Equal(v.X + w.X, sum.X);
        Assert.Equal(v.Y + w.Y, sum.Y);
        Assert.Equal(v.Z + w.Z, sum.Z);
    }

    [Theory]
    [MemberData(nameof(TestScalarAndVectorOperationsData))]
    public void TestVectorLeftScalarProduct(double t, Vec3 v)
    {
        Vec3 leftProd = t * v;
        Assert.Equal(t * v.X, leftProd.X);
        Assert.Equal(t * v.Y, leftProd.Y);
        Assert.Equal(t * v.Z, leftProd.Z);
    }

    [Theory]
    [MemberData(nameof(TestScalarAndVectorOperationsData))]
    public void TestVectorRightScalarProduct(double t, Vec3 v)
    {
        Vec3 rightProd = v * t;
        Assert.Equal(t * v.X, rightProd.X);
        Assert.Equal(t * v.Y, rightProd.Y);
        Assert.Equal(t * v.Z, rightProd.Z);
    }

    [Theory]
    [MemberData(nameof(TestScalarAndVectorOperationsData))]
    public void TestVectorScalarDivision(double t, Vec3 v)
    {
        Vec3 div = v / t;
        if (t == 0)
        {
            Assert.Equal(0, div.X);
            Assert.Equal(0, div.Y);
            Assert.Equal(0, div.Z);
        }
        else
        {
            Assert.Equal(1 / t * v.X, div.X);
            Assert.Equal(1 / t * v.Y, div.Y);
            Assert.Equal(1 / t * v.Z, div.Z);
        }
    }

    [Theory]
    [MemberData(nameof(TestVectorAndVectorOperationsData))]
    public void TestVectorProduct(Vec3 v, Vec3 w)
    {
        Vec3 u = v * w;
        Assert.Equal(v.X * w.X, u.X);
        Assert.Equal(v.Y * w.Y, u.Y);
        Assert.Equal(v.Z * w.Z, u.Z);
    }

    [Theory]
    [MemberData(nameof(TestVectorAndVectorOperationsData))]
    public void TestVectorDotProduct(Vec3 v, Vec3 w)
    {
        double dot = Vec3.Dot(v, w);
        Assert.Equal(v.X * w.X + v.Y * w.Y + v.Z * w.Z, dot);
    }

    [Theory]
    [MemberData(nameof(TestVectorAndVectorOperationsData))]
    public void TestVectorCrossProduct(Vec3 v, Vec3 w)
    {
        Vec3 u = Vec3.Cross(v, w);
        Assert.Equal(v.Y * w.Z - v.Z * w.Y, u.X);
        Assert.Equal(v.Z * w.X - v.X * w.Z, u.Y);
        Assert.Equal(v.X * w.Y - v.Y * w.X, u.Z);
    }

    [Theory]
    [MemberData(nameof(TestVectorUnitData))]
    public void TestVectorUnit(Vec3 v, Vec3 result)
    {
        double len = v.Norm();
        Vec3 unit = v / len;
        Assert.Equal(result.X, unit.X);
        Assert.Equal(result.Y, unit.Y);
        Assert.Equal(result.Z, unit.Z);
    }

    // MemberData iterators
    public static IEnumerable<object[]> TestLengthData() // data for TestLength and TestLengthSquared
    {
        yield return new object[] { 0, 0, 0, 0 };
        yield return new object[] { 3, 0, 0, 3 };
        yield return new object[] { 0, 3, 0, 3 };
        yield return new object[] { 0, 0, -3, 3 };
        yield return new object[] { -2, 2, -2, Math.Sqrt(12) };
        yield return new object[] { 1, 1, 1, Math.Sqrt(3) };
        yield return new object[] { -1, -1, -1, Math.Sqrt(3) };
    }

    public static IEnumerable<object[]> TestVectorAndVectorOperationsData() // data for (vector, vector) operations
    {
        yield return new object[] { new Vec3(0, 0, 0), new Vec3(1, 2, 3) };
        yield return new object[] { new Vec3(0, -1, 0), new Vec3(0, 1, 0) };
        yield return new object[] { new Vec3(0, 0, 0), new Vec3(0, 0, 0) };
        yield return new object[] { new Vec3(2, 2, 2), new Vec3(1, 1, 1) };
    }

    public static IEnumerable<object[]> TestScalarAndVectorOperationsData() // data for (scalar, vector) operations
    {
        yield return new object[] { 1, new Vec3(0, 0, 0) };
        yield return new object[] { 0, new Vec3(1, -1, 1) };
        yield return new object[] { 0.5, new Vec3(1, 0, 2) };
        yield return new object[] { -0.1, new Vec3(0, 0, -1) };
        yield return new object[] { -5, new Vec3(-2, -1, -0.5) };
    }

    public static IEnumerable<object[]> TestVectorUnitData() // data for vector normalization
    {
        yield return new object[] { new Vec3(0, 0, 0), new Vec3(0, 0, 0) };
        yield return new object[] { new Vec3(0, -3, 0), new Vec3(0, -1, 0) };
        yield return new object[] { new Vec3(1, 1, 1), 1 / Math.Sqrt(3) * new Vec3(1, 1, 1) };
        yield return new object[] { new Vec3(-1, 0, 0), new Vec3(-1, 0, 0) };
        yield return new object[] { new Vec3(-1, -1, -1), -1 / Math.Sqrt(3) * new Vec3(1, 1, 1) };
    }
}
