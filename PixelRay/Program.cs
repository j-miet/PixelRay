using PixelRay.Core;
using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Scene;
using PixelRay.SceneView.Camera;
using PixelRay.SceneView.HitObjects;
using PixelRay.SceneView.Lighting;
using PixelRay.Output;
using PixelRay.Rendering;
using PixelRay.SceneView.Hittable;

using DebugMode = PixelRay.Const.DebugMode;
using M4 = PixelRay.Core.Mathematics.Matrix4x4;

static class CreatePixelRay
{
    public static void Main(string[] args)
    {
        const int WIDTH = 249;
        const int HEIGHT = 140;
        const string outputPPM = "output.ppm";

        // renderer
        Palette colorPalette = new([]);
        const int lightingBands = 32;
        const int ambientFactor = 0;

        const DebugMode debugMode = DebugMode.None;
        const int upScaleFactor = 3;

        // scene: camera and scene objects
        Camera camera = new(
            new Vec3(0, 0, 0),
            WIDTH,
            HEIGHT,
            2,
            1
        );
        Scene scene = new();

        scene.AddObject(new Transform(
            new Sphere(new ColorRGB(0.7, 0.2, 0.2)),
            M4.Translate(-0.25, -0.15, -0.25) *
            M4.Scale(0.05)
        ));

        scene.AddObject(new Triangle(
            new Vec3(0.2, -0.25, -0.3),
            new Vec3(1.1, -0.25, -0.8),
            new Vec3(0.9, 0.25, -0.65),
            new ColorRGB(0.8, 0.9, 0.1)
        ));

        scene.AddObject(new Transform(
            new Cylinder(new ColorRGB(0.1, 0.3, 0.9)),
            M4.Translate(-0.6, -0.25, -0.75) *
            M4.Scale(0.25, 0.35, 0.25)
        ));

        scene.AddObject(new Transform(
            new Cone(new ColorRGB(0.8, 0.1, 0.1)),
            M4.Translate(-0.6, 0.45, -0.75) *
            M4.Rotate(new Vec3(1, 0, 0), Math.PI) *
            M4.Scale(0.25, 0.35, 0.25)
        ));

        scene.AddObject(new Transform(
            new Disc(new ColorRGB(0.8, 0.2, 0.8)),
            M4.Translate(-1.5, 0.65, -1.2) *
            M4.Rotate(new Vec3(0, 1, 0), -Math.PI / 32) *
            M4.Rotate(new Vec3(1, 0, 0), Math.PI / 2) *
            M4.Scale(0.25, 1.0, 0.25)
        ));

        scene.AddObject(new Transform(
            new Torus(new ColorRGB(0.5, 1.0, 0.5), 0.2, 0.1),
            M4.Translate(0, 0.1, -0.75) *
            M4.Rotate(new Vec3(1, 0, 0), Math.PI / 2)
        ));

        scene.AddObject(new Transform(
            new Plane(new ColorRGB(0.1, 0.8, 0.8)),
            M4.Translate(0, -0.25, 0)
        ));

        scene.AddObject(new Transform(
            new Quadric(
                new ColorRGB(0.5, 0.5, 0.5),
                1.0, -1.0, 1.0,
                0, 0, 0,
                0, 0, 0,
                -0.2,
                xmin: -3,
                xmax: 3,
                ymin: -3,
                ymax: 3,
                zmin: -3,
                zmax: 3
            ),
            M4.Translate(new Vec3(2, 0.5, -3))
        ));

        scene.AddLight(new DirectionalLight(
            new Vec3(0, -0.3, -1),
            new ColorRGB(1, 1, 1)
        ));

        // render, save output image and display it
        Renderer renderer = new(WIDTH, HEIGHT, colorPalette, lightingBands, ambientFactor);
        FrameBuffer buffer = renderer.Render(scene, camera, upScaleFactor, debugMode);
        ImageWriter.WritePPM(outputPPM, buffer);

        // ensure upscaled resolution has even coordinates
        int w = (WIDTH % 2 == 0) ? WIDTH * upScaleFactor : (WIDTH - 1) * upScaleFactor;
        int h = (HEIGHT % 2 == 0) ? HEIGHT * upScaleFactor : (HEIGHT - 1) * upScaleFactor;
        ImageDisplay display = new(w, h, buffer, 0);
        display.Display();
    }
}

