using MoonSharp.Interpreter;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.InstanceObject;

namespace PixelRay.Input.Scripting;

/// <summary>
/// Handle geometric object manipulation
/// </summary>
public class LuaObject(Instance instance)
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

    /// <summary>
    /// Move instance object to new direction from its base position
    /// </summary>
    public void Translate(double x, double y, double z)
    {
        var t = Matrix4x4.Translate(x, y, z);

        _instance.Transform = new Transform(t * _instance.Transform.LocalToWorld);
    }

    /// <summary>
    /// Scale instance object size
    /// </summary>
    public void Scale(double x, double y, double z)
    {
        var s = Matrix4x4.Scale(x, y, z);

        _instance.Transform = new Transform(s * _instance.Transform.LocalToWorld);
    }
    public void ScaleUniform(double t) => Scale(t, t, t);

    /// <summary>
    /// Apply rotation with respect to axis direction (x, y, z) and angle w
    /// </summary>
    public void Rotate(double x, double y, double z, double w)
    {
        var axis = new Vec3(x, y, z);
        var angleRadians = InputUtils.DegreesToRadians(w);
        var r = Matrix4x4.Rotate(axis, angleRadians);

        _instance.Transform = new Transform(r * _instance.Transform.LocalToWorld);
    }

    /// <summary>
    /// Perform multiple rotation in a sequence
    /// In Lua, pass each rotation as a table e.g. RotateMultiple({x1, y1, z1, w1}, {x2, y2, z2, w2}, ...)
    /// </summary>
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