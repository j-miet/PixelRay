using PixelRay.Lighting;

namespace PixelRay.SceneObjects;

/// <summary>
/// Collection of all traceable objects: hittables, lights
/// </summary>
public class Scene
{
    public List<IHittable> Objects { get; } = [];
    public List<Light> Lights { get; } = [];
}