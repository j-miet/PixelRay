using PixelRay.Lighting;

namespace PixelRay.SceneObjects;

public class Scene
{
    public List<IHittable> Objects { get; } = [];
    public List<Light> Lights { get; } = [];
}