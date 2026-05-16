using PixelRay.SceneView;

namespace PixelRay.Input.Scripting;

/// <summary>
/// Access scene object instances and lights
/// </summary>
public class LuaSceneApi(Scene scene)
{
    public LuaObjectApi GetObject(string name)
    {
        var obj = _scene.GetObject(name);
        return new LuaObjectApi(obj);
    }

    public LuaLightApi GetLight(string name)
    {
        var light = _scene.GetLight(name);
        return new LuaLightApi(light);
    }

    private readonly Scene _scene = scene;
}