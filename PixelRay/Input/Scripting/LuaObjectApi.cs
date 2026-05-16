using MoonSharp.Interpreter;

using PixelRay.Core.Mathematics;
using PixelRay.SceneView.InstanceObject;

namespace PixelRay.Input.Scripting;

/// <summary>
/// Handle instance object geometry i.e. updating transforms
/// </summary>
public class LuaObjectApi(Instance instance)
{
    /// <summary>
    /// Reset back to base transform matrix.
    /// This way transforms are always applied to base which both makes scripting easier AND removes the error
    /// accumulation after multiple matrix multiplications.
    /// -- not internally called, because otherwise this would cancel sequential transforms. Instead it should be
    /// called manually by user e.g. at the start of loop to prevent mentioned numerical errors --
    /// </summary>
    public void ResetTransform()
    {
        _instance.Transform = _instance.BaseTransform;
    }

    public void Translate(double x, double y, double z)
    {
        var t = Matrix4x4.Translate(x, y, z);
        Vec3 newPos = _instance.Transform.Position + new Vec3(x, y, z);

        _instance.Transform = new Transform(t * _instance.Transform.LocalToWorld)
        {
            Position = newPos
        };
    }

    public void Scale(double x, double y, double z)
    {
        Vec3 pos = _instance.Transform.Position;

        var posT = Matrix4x4.Translate(pos);
        var invPosT = Matrix4x4.Translate(-pos);

        var s = Matrix4x4.Scale(x, y, z);

        _instance.Transform = new Transform(posT * s * invPosT * _instance.Transform.LocalToWorld)
        {
            Position = pos
        };
    }

    public void ScaleUniform(double t) => Scale(t, t, t);

    public void Rotate(double x, double y, double z, double w)
    {
        Vec3 pos = _instance.Transform.Position;

        var posT = Matrix4x4.Translate(pos);
        var invPosT = Matrix4x4.Translate(-pos);

        var axis = new Vec3(x, y, z);
        var angleRadians = InputUtils.DegreesToRadians(w);
        var r = Matrix4x4.Rotate(axis, angleRadians);

        _instance.Transform = new Transform(posT * r * invPosT * _instance.Transform.LocalToWorld)
        {
            Position = pos
        };
    }

    public void RotateMultiple(DynValue rotations)
    {
        var list = rotations.Table;
        Matrix4x4 finalRotation = new();

        foreach (var pair in list.Pairs)
        {
            var item = pair.Value.Table;

            var axis = new Vec3(
                (double)item.Get(1).Number,
                (double)item.Get(2).Number,
                (double)item.Get(3).Number
            );
            var angleRadians = InputUtils.DegreesToRadians((double)item.Get(4).Number);

            var r = Matrix4x4.Rotate(axis, angleRadians);

            finalRotation = r * finalRotation;
        }

        _instance.Transform = new Transform(finalRotation * _instance.Transform.LocalToWorld);
    }

    private readonly Instance _instance = instance;
}