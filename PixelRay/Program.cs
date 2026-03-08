using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Scene;
using PixelRay.SceneView.Camera;
using PixelRay.SceneView.HitObjects;
using PixelRay.SceneView.Lighting;
using PixelRay.Output;
using PixelRay.Rendering;
using PixelRay.SceneView.Hittable;
using PixelRay.SceneView.HitObjects.Transformed;

using Debug = PixelRay.Core.Mathematics.Const.DebugMode;
using M4 = PixelRay.Core.Mathematics.Matrix4x4;

static class CreatePixelRay
{
    public static void Main(string[] args)
    {
        const int WIDTH = 240;
        const int HEIGHT = 140;
        const int upScaleFactor = 3;

        int w = WIDTH * upScaleFactor;
        int h = HEIGHT * upScaleFactor;

        Scene scene = new();

        scene.Objects.Add(new SphereT(
            new Vec3(-0.25, -0.15, -0.25),
            0.05,
            new ColorRGB(0.7, 0.2, 0.2)
        ));

        scene.Objects.Add(new Triangle(
            new Vec3(0.2, -0.25, -0.3),
            new Vec3(1.1, -0.25, -0.8),
            new Vec3(0.9, 0.25, -0.65),
            new ColorRGB(0.8, 0.9, 0.1)
        ));

        scene.Objects.Add(new CylinderT(
            new Vec3(-0.6, -0.25, -0.75),
            new Vec3(0, 1, 0),
            0.25,
            0.35,
            new ColorRGB(0.1, 0.3, 0.9)
        ));

        scene.Objects.Add(new ConeT(
            new Vec3(-0.6, 0.45, -0.75),
            new Vec3(0, -1, 0),
            0.25,
            0.35,
            new ColorRGB(0.8, 0.1, 0.1)
        ));

        scene.Objects.Add(new DiscT(
            new Vec3(-1.3, 0.65, -1.2),
            new Vec3(0.5, 0, 1),
            0.25,
            new ColorRGB(0.8, 0.2, 0.8)
        ));

        scene.Objects.Add(new TorusT(
            new Vec3(0, 0.1, -0.75),
            new Vec3(0, 0, -1),
            0.2,
            0.1,
            new ColorRGB(0.5, 1.0, 0.5)
        ));

        scene.Objects.Add(new PlaneT(
            new Vec3(0, -0.25, 0),
            new Vec3(0, 1, 0),
            new ColorRGB(0.1, 0.8, 0.8)
        ));


        scene.Objects.Add(new Transform(
            new Quadric(
                new ColorRGB(0.5, 0.5, 0.5),
                1.0, -1.0, 1.0,
                0, 0, 0,
                0, 0, 0,
                -0.2,
                xmin: -2,
                xmax: 2,
                ymin: -2,
                ymax: 2,
                zmin: -2,
                zmax: 2
            ),
            M4.Translate(new Vec3(2, 0.5, -3))
        ));

        scene.Lights.Add(new DirectionalLight(
            new Vec3(0, -0.3, -1),
            new ColorRGB(1, 1, 1)
        ));

        Camera camera = new(
            new Vec3(0, 0, 0),
            WIDTH,
            HEIGHT,
            2,
            1
        );
        Renderer renderer = new(WIDTH, HEIGHT, new Palette([]), 32, 0);
        FrameBuffer buffer = renderer.Render(scene, camera, upScaleFactor);
        ImageWriter.WritePPM("output.ppm", buffer);

        ImageDisplay display = new(w, h, buffer, 0);
        display.Display();
    }
}

