using PixelRay.SceneView.Hittable;
using PixelRay.SceneView.Lighting;

namespace PixelRay.SceneView;

/// <summary>
/// Collection of all traceable objects: hittables, lights
/// </summary>
public class Scene
{
    public List<IHittable> Objects { get; } = [];
    public List<ILight> Lights { get; } = [];

    public void AddObject(IHittable obj) => Objects.Add(obj);
    public void AddLight(ILight light) => Lights.Add(light);
}