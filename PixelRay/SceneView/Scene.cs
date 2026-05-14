using PixelRay.SceneView.Lighting;
using PixelRay.SceneView.InstanceObject;

namespace PixelRay.SceneView;

/// <summary>
/// Collection of all traceable objects i.e. hittable instances and lights
/// </summary>
public class Scene
{
    public required Camera Camera { get; set; }
    public List<Instance> Objects { get; } = [];
    public List<ILight> Lights { get; } = [];

    public Dictionary<string, int> NameLookup = [];

    public Instance Get(string name)
    {
        return Objects[NameLookup[name]];
    }

    public void AddObject(Instance obj)
    {
        Objects.Add(obj);
    }

    public void AddLight(ILight light) => Lights.Add(light);
}