using PixelRay.SceneView;

namespace PixelRay.Input.Scripting;

/// <summary>
/// API to access scene camera, geometry and lights
/// </summary>
public class LuaSceneApi(Scene scene)
{
    private readonly Scene _scene = scene;

    public LuaObject GetObject(string name)
    {
        var obj = _scene.Get(name);
        return new LuaObject(obj);
    }
}