using PixelRay.Core;
using PixelRay.SceneView;
using PixelRay.SceneView.HitObjects;
using PixelRay.SceneView.Lighting;
using PixelRay.Output;
using PixelRay.Rendering;
using PixelRay.SceneView.Hittable;

using DebugMode = PixelRay.Const.DebugMode;
using M4 = PixelRay.Core.Mathematics.Matrix4x4;
using PixelRay.SceneView.Materials;

static class CreatePixelRay
{
    public static void Main(string[] args)
    {
        const int WIDTH = 249;
        const int HEIGHT = 140;
        const string outputPPM = "output.ppm";

        // renderer init
        Palette colorPalette = new([]);
        const int lightingBands = 32;
        const int ambientFactor = 0;
        const int maxDepth = 1; // reflection bounces; keep at 1

        // render settings
        const DebugMode debugMode = DebugMode.None;
        const int upScaleFactor = 3;

        // scene: camera and scene objects
        Camera camera = new(
            new(0, 0, 0),
            WIDTH,
            HEIGHT,
            2,
            1
        );
        Scene scene = new();

        // objects
        scene.AddObject(new Transform(
            new Sphere(
                new FlatMaterial(
                    new(0.7, 0.2, 0.2),
                    0
                )
            ),
            M4.Translate(-0.25, -0.15, -0.25) *
            M4.Scale(0.05)
        ));

        scene.AddObject(
            new Triangle(
                new(0.2, -0.25, -0.3),
                new(1.1, -0.25, -0.8),
                new(0.9, 0.25, -0.65),
                new FlatMaterial(
                    new(0.8, 0.9, 0.1),
                    0
                )
            )
        );

        scene.AddObject(new Transform(
            new Cylinder(
                new FlatMaterial(
                    new(0.1, 0.3, 0.9),
                    0.5
                )
            ),
            M4.Translate(-0.6, -0.25, -0.75) *
            M4.Scale(0.25, 0.35, 0.25)
        ));

        scene.AddObject(new Transform(
            new Cone(
                new FlatMaterial(
                    new(0.8, 0.1, 0.1),
                    0
                )
            ),
            M4.Translate(-0.6, 0.45, -0.75) *
            M4.Rotate(new(1, 0, 0), Math.PI) *
            M4.Scale(0.25, 0.35, 0.25)
        ));

        scene.AddObject(new Transform(
            new Disc(
                new FlatMaterial(
                    new(0.8, 0.2, 0.8),
                    0
                )
            ),
            M4.Translate(-1.55, 0.7, -1.2) *
            M4.Rotate(new(0, 1, 0), -Math.PI / 32) *
            M4.Rotate(new(1, 0, 0), Math.PI / 2) *
            M4.Scale(0.25, 1.0, 0.25)
        ));

        scene.AddObject(new Transform(
            new Torus(
                0.2,
                0.1,
                new FlatMaterial(
                    new(0.5, 1.0, 0.5),
                    0
                )
            ),
            M4.Translate(0, 0.1, -0.75) *
            M4.Rotate(new(1, 0, 0), Math.PI / 2)
        ));

        scene.AddObject(new Transform(
            new Plane(
                new FlatMaterial(
                    new(0.1, 0.8, 0.8),
                    0
                )
            ),
            M4.Translate(0, -0.25, 0)
        ));

        scene.AddObject(new Transform(
            new Plane(
                new FlatMaterial(
                    new(1, 1, 1),
                    0
                )
            ),
            M4.Translate(0, 0, -20) *
            M4.Rotate(new(1, 0, 0), Math.PI / 2)
        ));

        scene.AddObject(new Transform(
            new Quadric(
                1.0, -1.0, 1.0,
                0, 0, 0,
                0, 0, 0,
                -0.2,
                new(-3, -3, -3),
                new(3, 3, 3),
                new FlatMaterial(
                    new(0.5, 0.5, 0.5),
                    0
                )
            ),
            M4.Translate(new(2, 0.5, -3))
        ));

        scene.AddObject(new Transform(
            new AABox(
                new(-0.05, -0.05, -0.05),
                new(0.05, 0.05, 0.05),
                new FlatMaterial(
                    new(1, 0.65, 0),
                    0
                )
            ),
            M4.Translate(new(0.14, -0.1, -0.30))
        ));

        // lights
        scene.AddLight(new DirectionalLight(
            new(0.2, -0.3, -1),
            new(1, 1, 1)
        ));

        // render, save output image and display it
        Renderer renderer = new(WIDTH, HEIGHT, colorPalette, lightingBands, ambientFactor, maxDepth);
        FrameBuffer buffer = renderer.Render(scene, camera, upScaleFactor, debugMode);
        ImageWriter.WritePPM(outputPPM, buffer);

        // ensure upscaled resolution has even coordinates
        int w = (WIDTH % 2 == 0) ? WIDTH * upScaleFactor : (WIDTH - 1) * upScaleFactor;
        int h = (HEIGHT % 2 == 0) ? HEIGHT * upScaleFactor : (HEIGHT - 1) * upScaleFactor;
        ImageDisplay display = new(w, h, buffer, 0);
        display.Display();
    }
}

