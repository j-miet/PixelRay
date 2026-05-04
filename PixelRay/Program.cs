using PixelRay.Core;
using PixelRay.Input;
using PixelRay.Output;

using DebugMode = PixelRay.Const.DebugMode;

static class CreatePixelRay
{
    public static void Main(string[] args)
    {
        // TODO implement better parser which doesn't depend on arg writing order
        // should separate input and output (-i / -o) so -p flag can be used without needing to save the image
        if (args.Length >= 3 &&
            (args[0] == "-i" || args[0] == "--image") &&
            args[1] != null && args[2] != null)
        {
            var dto = SceneLoader.Load(args[1].ToString());
            var (scene, camera, settings) = SceneBuilder.Build(dto);

            int width = settings.Width;
            int height = settings.Height;
            int upScaleFactor = settings.UpScaleFactor;

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

            // debug 
            DebugMode debug = DebugMode.None;
            if (args.Length >= 5 && args[4] == "--debug" && args[5] != null)
            {
                string mode = args[5].ToString();
                debug = mode switch
                {
                    "normals" => DebugMode.Normals,
                    "distance" => DebugMode.DepthHeat,
                    "id" => DebugMode.ObjectID,
                    _ => DebugMode.None
                };
            }

            FrameBuffer buffer = renderer.Render(scene, camera, upScaleFactor, debug);

            // output file
            string outputFile = args[2].ToString();
            string imageFormat = outputFile.Split(".")[1];

            if (imageFormat == "ppm")
            {
                ImageWriter.WritePPM(outputFile, buffer);
            }
            else if (imageFormat == "png")
            {
                ImageWriter.WritePNG(outputFile, buffer);
            }
            else
            {
                throw new Exception("Unsupported image format: use .ppm or .png");
            }

            // preview image
            if (args.Length >= 4 && args[3] == "-p")
            {
                int speed = 0;
                if (args.Length == 5 && int.TryParse(args[4], out int value))
                    speed = value >= 0 ? value : 0;

                // ensure upscaled resolution has even coordinates, otherwise image display crashes
                int w = (width % 2 == 0) ? width * upScaleFactor : (width - 1) * upScaleFactor;
                int h = (height % 2 == 0) ? height * upScaleFactor : (height - 1) * upScaleFactor;

                ImageDisplay display = new(w, h, buffer, speed);
                display.Display();
            }
        }
    }
}

