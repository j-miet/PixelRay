using PixelRay.Core;
using PixelRay.Input;
using PixelRay.Output;

using DebugMode = PixelRay.Const.DebugMode;

static class CreatePixelRay
{
    public static void Main(string[] args)
    {
        if (args.Length >= 3 &&
            (args[0] == "-i" || args[0] == "--image") &&
            args[1] != null && args[2] != null)
        {
            var dto = SceneLoader.Load((args[1] + ".json").ToString());
            var (scene, camera, settings) = SceneBuilder.Build(dto);

            int width = settings.Width;
            int height = settings.Height;

            // if more options are added, consider passing settings itself and let renderer handle rest
            Renderer renderer = new(
                width,
                height,
                settings.Palette,
                settings.LightingBands,
                settings.MaxBounces,
                settings.Dithering,
                settings.DitherLevels,
                settings.DitherDimension
            );

            int upScaleFactor = 3;

            FrameBuffer buffer = renderer.Render(scene, camera, upScaleFactor, DebugMode.None);
            ImageWriter.WritePPM(args[2].ToString() + "ppm", buffer);

            if (args.Length == 4 && args[3] == "-p") // preview image
            {
                // ensure upscaled resolution has even coordinates, otherwise image display crashes
                int w = (width % 2 == 0) ? width * upScaleFactor : (width - 1) * upScaleFactor;
                int h = (height % 2 == 0) ? height * upScaleFactor : (height - 1) * upScaleFactor;

                ImageDisplay display = new(w, h, buffer, 0);
                display.Display();
            }
        }
    }
}

