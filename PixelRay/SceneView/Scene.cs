using PixelRay.SceneView.Hittable;
using PixelRay.SceneView.Lighting;

namespace PixelRay.SceneView.Scene;

/// <summary>
/// Collection of all traceable objects: hittables, lights
/// </summary>
public class Scene
{
    public List<IHittable> Objects { get; } = [];
    public List<Light> Lights { get; } = [];
}