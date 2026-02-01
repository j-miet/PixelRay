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

        Assert.True(v.Data == (0, 0, 0));
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1, 2, 3)]
    [InlineData(2.0, 1.0, -2)]
    public void TestVec3(double x, double y, double z)
    {
        Vec3 v = new(x, y, z);

        Assert.True(v.Data == (x, y, z));
    }

    [Fact]
    public void TestVec3Get()
    {
        Vec3 v = new(1, 2, 3);
        Assert.Equal(1, v.X);
        Assert.Equal(2, v.Y);
        Assert.Equal(3, v.Z);
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
        Vec3 v = new(x, y, z);

        Assert.True(v.LengthSquared() >= 0);
        Assert.True(v.LengthSquared() >= value * value);
    }

    [Theory]
    [MemberData(nameof(TestLengthData))]
    public void TestLength(double x, double y, double z, double value)
    {
        Vec3 v = new(x, y, z);

        Assert.True(v.Length() >= 0);
        Assert.True(v.Length() == value);
    }

    [Fact]
    public void TestVectorIndex()
    {
        Vec3 v = new(1, 2, 3);

        Assert.True(v[0] == v.X);
        Assert.True(v[1] == v.Y);
        Assert.True(v[2] == v.Z);
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
        Assert.True(w.X == -x);
        Assert.True(w.Y == -y);
        Assert.True(w.Z == -z);
    }

    [Theory]
    [MemberData(nameof(TestVectorAndVectorOperationsData))]
    public void TestVectorSubtract(Vec3 v, Vec3 w)
    {
        Vec3 sub = v - w;
        Assert.True(sub.X == v.X - w.X);
        Assert.True(sub.Y == v.Y - w.Y);
        Assert.True(sub.Z == v.Z - w.Z);
    }

    [Theory]
    [MemberData(nameof(TestVectorAndVectorOperationsData))]
    public void TestVectorSum(Vec3 v, Vec3 w)
    {
        Vec3 sum = v + w;
        Assert.True(sum.X == v.X + w.X);
        Assert.True(sum.Y == v.Y + w.Y);
        Assert.True(sum.Z == v.Z + w.Z);
    }

    [Theory]
    [MemberData(nameof(TestScalarAndVectorOperationsData))]
    public void TestVectorLeftScalarProduct(double t, Vec3 v)
    {
        Vec3 leftProd = t * v;
        Assert.True(leftProd.X == t * v.X);
        Assert.True(leftProd.Y == t * v.Y);
        Assert.True(leftProd.Z == t * v.Z);
    }

    [Theory]
    [MemberData(nameof(TestScalarAndVectorOperationsData))]
    public void TestVectorRightScalarProduct(double t, Vec3 v)
    {
        Vec3 rightProd = v * t;
        Assert.True(rightProd.X == t * v.X);
        Assert.True(rightProd.Y == t * v.Y);
        Assert.True(rightProd.Z == t * v.Z);
    }

    [Theory]
    [MemberData(nameof(TestScalarAndVectorOperationsData))]
    public void TestVectorScalarDivision(double t, Vec3 v)
    {
        if (t == 0)
        {
            Assert.Throws<DivideByZeroException>(() => v / t);
        }
        else
        {
            Vec3 div = v / t;
            Assert.True(div.X == 1 / t * v.X);
            Assert.True(div.Y == 1 / t * v.Y);
            Assert.True(div.Z == 1 / t * v.Z);
        }
    }

    [Theory]
    [MemberData(nameof(TestVectorAndVectorOperationsData))]
    public void TestVectorProduct(Vec3 v, Vec3 w)
    {
        Vec3 u = v * w;
        Assert.True(u.X == v.X * w.X);
        Assert.True(u.Y == v.Y * w.Y);
        Assert.True(u.Z == v.Z * w.Z);
    }

    [Theory]
    [MemberData(nameof(TestVectorAndVectorOperationsData))]
    public void TestVectorDotProduct(Vec3 v, Vec3 w)
    {
        double dot = Vec3.Dot(v, w);
        Assert.True(dot == v.X * w.X + v.Y * w.Y + v.Z * w.Z);
    }

    [Theory]
    [MemberData(nameof(TestVectorAndVectorOperationsData))]
    public void TestVectorCrossProduct(Vec3 v, Vec3 w)
    {
        Vec3 u = Vec3.Cross(v, w);
        Assert.True(u.X == v.Y * w.Z - v.Z * w.Y);
        Assert.True(u.Y == v.Z * w.X - v.X * w.Z);
        Assert.True(u.Z == v.X * w.Y - v.Y * w.X);
    }

    [Theory]
    [MemberData(nameof(TestVectorUnitData))]
    public void TestVectorUnit(Vec3 v, Vec3 result)
    {
        double len = v.Length();
        if (len == 0)
        {
            Assert.Throws<DivideByZeroException>(() => v / len);
        }
        else
        {
            Vec3 unit = v / len;
            Assert.True(unit.Data == result.Data);
        }
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
