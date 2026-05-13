using PixelRay.Core;
using PixelRay.Core.Mathematics;

namespace PixelRay.Tests.CoreTests;

public class Matrix4x4Tests
{
    [Fact]
    public void Identity_DoesNotChangePoint()
    {
        var p = new Vec3(1, 2, 3);
        var id = Matrix4x4.Identity();

        var result = id.TransformPoint(p);

        Assert.True(CoreHelper.NearlyEqual(p, result));
    }

    [Fact]
    public void Identity_DoesNotChangeVector()
    {
        var v = new Vec3(1, 2, 3);
        var id = Matrix4x4.Identity();

        var result = id.TransformVector(v);

        Assert.True(CoreHelper.NearlyEqual(v, result));
    }

    [Fact]
    public void Translate_OnlyMovesPoints()
    {
        var p = new Vec3(1, 1, 1);
        var t = Matrix4x4.Translate(10, 0, 0);

        var result = t.TransformPoint(p);

        Assert.True(CoreHelper.NearlyEqual(new Vec3(11, 1, 1), result));
    }

    [Fact]
    public void Translate_DoesNotAffectVectors()
    {
        var v = new Vec3(1, 0, 0);
        var t = Matrix4x4.Translate(10, 0, 0);

        var result = t.TransformVector(v);

        Assert.True(CoreHelper.NearlyEqual(v, result));
    }

    [Fact]
    public void Scale_TransformsPointCorrectly()
    {
        var p = new Vec3(1, 2, 3);
        var s = Matrix4x4.Scale(2, 3, 4);

        var result = s.TransformPoint(p);

        Assert.True(CoreHelper.NearlyEqual(new Vec3(2, 6, 12), result));
    }

    [Fact]
    public void Scale_TransformsVectorCorrectly()
    {
        var v = new Vec3(1, 2, 3);
        var s = Matrix4x4.Scale(2, 3, 4);

        var result = s.TransformVector(v);

        Assert.True(CoreHelper.NearlyEqual(new Vec3(2, 6, 12), result));
    }

    [Fact]
    public void TransformRay_TransformsOriginAndDirectionCorrectly()
    {
        var ray = new Ray(
            new Vec3(1, 1, 1),
            new Vec3(1, 0, 0)
        );

        var t = Matrix4x4.Translate(10, 0, 0);

        var result = t.TransformRay(ray);

        Assert.True(CoreHelper.NearlyEqual(new Vec3(11, 1, 1), result.Origin));
        Assert.True(CoreHelper.NearlyEqual(new Vec3(1, 0, 0), result.Direction));
    }

    [Fact]
    public void InverseAffine_RoundTrip_ReturnsIdentity()
    {
        var m = Matrix4x4.Translate(3, -2, 5) *
                Matrix4x4.Scale(2, 2, 2);

        var inv = m.InverseAffine();

        var identity = m * inv;

        var id = Matrix4x4.Identity();

        Assert.True(CoreHelper.NearlyEqual(identity, id));
    }

    [Fact]
    public void InverseAffine_RestoresOriginalPoint()
    {
        var p = new Vec3(1, 2, 3);

        var m = Matrix4x4.Translate(5, -2, 1) *
                Matrix4x4.Scale(2, 3, 4);

        var inv = m.InverseAffine();

        var transformed = m.TransformPoint(p);
        var restored = inv.TransformPoint(transformed);

        Assert.True(CoreHelper.NearlyEqual(p, restored));
    }

    [Fact]
    public void MatrixMultiplication_OrderMatters()
    {
        var p = new Vec3(1, 0, 0);

        var translate = Matrix4x4.Translate(10, 0, 0);
        var scale = Matrix4x4.Scale(2);

        var a = translate * scale;
        var b = scale * translate;

        var ra = a.TransformPoint(p);
        var rb = b.TransformPoint(p);

        Assert.False(CoreHelper.NearlyEqual(ra, rb));
    }

    [Fact]
    public void Rotate_90Degrees_ZAxis()
    {
        var p = new Vec3(1, 0, 0);

        var rot = Matrix4x4.Rotate(new Vec3(0, 0, 1), Math.PI / 2);

        var result = rot.TransformPoint(p);

        Assert.True(CoreHelper.NearlyEqual(new Vec3(0, 1, 0), result, 1e-8));
    }
}