using PixelRay.Core.Mathematics;

namespace PixelRay.Tests.CoreTests;

public class Vec3Tests
{
    [Fact]
    public void Addition_IsCommutative()
    {
        var a = new Vec3(1, 2, 3);
        var b = new Vec3(4, 5, 6);

        Assert.True(CoreHelper.NearlyEqual(a + b, b + a));
    }

    [Fact]
    public void Subtraction_ProducesCorrectResult()
    {
        var a = new Vec3(5, 4, 3);
        var b = new Vec3(1, 2, 3);

        Assert.True(CoreHelper.NearlyEqual(a - b, new Vec3(4, 2, 0)));
    }

    [Fact]
    public void Dot_IsCommutative()
    {
        var a = new Vec3(1, 2, 3);
        var b = new Vec3(4, 5, 6);

        Assert.True(CoreHelper.NearlyEqual(Vec3.Dot(a, b), Vec3.Dot(b, a)));
    }

    [Fact]
    public void Dot_OrthogonalVectorsAreZero()
    {
        var a = new Vec3(1, 0, 0);
        var b = new Vec3(0, 1, 0);

        Assert.True(CoreHelper.NearlyEqual(Vec3.Dot(a, b), 0));
    }

    [Fact]
    public void Cross_IsAntiCommutative()
    {
        var a = new Vec3(1, 2, 3);
        var b = new Vec3(4, 5, 6);

        var ab = Vec3.Cross(a, b);
        var ba = Vec3.Cross(b, a);

        Assert.True(CoreHelper.NearlyEqual(ab, -ba));
    }

    [Fact]
    public void Cross_IsOrthogonalToInputs()
    {
        var a = new Vec3(1, 2, 3);
        var b = new Vec3(4, 5, 6);

        var c = Vec3.Cross(a, b);

        Assert.True(CoreHelper.NearlyEqual(Vec3.Dot(c, a), 0));
        Assert.True(CoreHelper.NearlyEqual(Vec3.Dot(c, b), 0));
    }

    [Fact]
    public void Cross_RightHandRule_ZAxisExample()
    {
        var x = new Vec3(1, 0, 0);
        var y = new Vec3(0, 1, 0);

        var z = Vec3.Cross(x, y);

        Assert.True(CoreHelper.NearlyEqual(z, new Vec3(0, 0, 1)));
    }

    [Fact]
    public void Length_IsCorrect()
    {
        var v = new Vec3(3, 4, 0);

        Assert.True(CoreHelper.NearlyEqual(v.Length(), 5));
    }

    [Fact]
    public void Unit_VectorHasLengthOne()
    {
        var v = new Vec3(3, 4, 0).Unit();

        Assert.True(CoreHelper.NearlyEqual(v.Length(), 1));
    }

    [Fact]
    public void Unit_DoesNotModifyZeroVector()
    {
        var v = new Vec3(0, 0, 0).Unit();

        Assert.True(CoreHelper.NearlyEqual(v, new Vec3(0, 0, 0)));
    }

    [Fact]
    public void Projection_DecomposesVectorCorrectly()
    {
        var v = new Vec3(3, 4, 0);
        var u = new Vec3(1, 0, 0);

        var proj = Vec3.Proj(v, u);
        var ortho = Vec3.Oproj(v, u);

        var reconstructed = proj + ortho;

        Assert.True(CoreHelper.NearlyEqual(reconstructed, v));
    }

    [Fact]
    public void Reflect_OnNormalReversesDirection()
    {
        var v = new Vec3(1, -1, 0);
        var n = new Vec3(0, 1, 0);

        var r = Vec3.Reflect(v, n);

        Assert.True(CoreHelper.NearlyEqual(r, new Vec3(1, 1, 0)));
    }

    [Fact]
    public void Reflect_IsSymmetric()
    {
        var v = new Vec3(1, -1, 0);
        var n = new Vec3(0, 1, 0);

        var r = Vec3.Reflect(v, n);
        var r2 = Vec3.Reflect(r, n);

        Assert.True(CoreHelper.NearlyEqual(r2, v));
    }

    [Fact]
    public void Lerp_BoundsHold()
    {
        var a = new Vec3(0, 0, 0);
        var b = new Vec3(10, 10, 10);

        var mid = Vec3.Lerp(a, b, 0.5);

        Assert.True(CoreHelper.NearlyEqual(mid, new Vec3(5, 5, 5)));
    }

    [Fact]
    public void Cross_MagnitudeMatchesAreaProperty()
    {
        var a = new Vec3(3, 0, 0);
        var b = new Vec3(0, 4, 0);

        var cross = Vec3.Cross(a, b);

        Assert.True(CoreHelper.NearlyEqual(cross.Length(), 12));
    }
}