using PixelRay.Core;
using PixelRay.Geometry;
using PixelRay.Lighting;
using PixelRay.Mathematics;
using PixelRay.Output;
using PixelRay.Rendering;
using PixelRay.SceneObjects;

static class CreatePixelRay
{
    public static void Main(string[] args)
    {
        const int WIDTH = 160;
        const int HEIGHT = 120;
        const int upScaleFactor = 3;

        int w = WIDTH * upScaleFactor;
        int h = HEIGHT * upScaleFactor;

        Scene scene = new();

        scene.Objects.Add(new Sphere(
            new Vec3(-1, 0.25, -2),
            0.5,
            new ColorRGB(1, 0.2, 0.2)
        ));

        scene.Objects.Add(new Plane(
            new Vec3(0, -0.25, 0),
            new Vec3(0, 1, 0),
            new ColorRGB(0.1, 0.8, 0.8)
        ));


        scene.Objects.Add(new Triangle(
            new Vec3(0.2, -0.25, -0.5),
            new Vec3(0.8, -0.25, -1),
            new Vec3(0.4, 0.25, -0.75),
            new ColorRGB(0.3, 0.7, 0.5)
        ));


        scene.Lights.Add(new DirectionalLight(
            new Vec3(-1, -1, -1),
            new ColorRGB(1, 1, 1)
        ));




        Camera camera = new(
            new Vec3(0, 0, 0),
            WIDTH,
            HEIGHT
        );
        Renderer renderer = new(WIDTH, HEIGHT, new Palette([]), 4);
        FrameBuffer buffer = renderer.Render(scene, camera, upScaleFactor);
        ImageWriter.WritePPM("output.ppm", buffer);

        ImageDisplay display = new(w, h, buffer, 1);
        display.Display();
    }
}

