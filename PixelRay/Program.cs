using PixelRay.Core;
using PixelRay.Geometry;
using PixelRay.Mathematics;
using PixelRay.Ouput;
using PixelRay.SceneObjects;

static class CreatePixelRay
{
    public static void Main(string[] args)
    {
        const int WIDTH = 160;
        const int HEIGHT = 120;

        Scene scene = new();

        scene.Objects.Add(new Sphere(
            new Vec3(0, 0, -2),
            1,
            new ColorRGB(1, 0.2, 0.2)
        ));

        Camera camera = new(
            new Vec3(0, 0, 0),
            WIDTH,
            HEIGHT
        );
        Renderer renderer = new(WIDTH, HEIGHT, 4, new Vec3(-1, -1, -1));
        FrameBuffer buffer = renderer.Render(scene, camera);
        ImageWriter.WritePPM("output.ppm", buffer);
    }
}

