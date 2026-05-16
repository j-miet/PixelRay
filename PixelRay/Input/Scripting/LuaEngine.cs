using MoonSharp.Interpreter;

using PixelRay.SceneView;

namespace PixelRay.Input.Scripting;

public class LuaEngine
{
    public LuaEngine()
    {
        UserData.RegisterType<LuaSceneApi>();
        UserData.RegisterType<LuaCameraApi>();
        UserData.RegisterType<LuaObjectApi>();
        UserData.RegisterType<LuaLightApi>();
    }

    public void AttachScene(Scene scene, Camera camera)
    {
        _camera = camera;

        _lua.Globals["scene"] = UserData.Create(new LuaSceneApi(scene));
        _lua.Globals["camera"] = UserData.Create(new LuaCameraApi(camera));
    }

    public void Load(string file)
    {
        _lua.DoFile(file);
    }

    public void Update(double t)
    {
        _lua.Globals["t"] = t;

        var fn = _lua.Globals.Get("update");

        if (!fn.IsNil())
            _lua.Call(fn, t);

        _camera?.Rebuild();
    }

    private readonly Script _lua = new();
    private Camera? _camera;
}