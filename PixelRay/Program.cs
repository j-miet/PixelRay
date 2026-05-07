using PixelRay.Core;
using PixelRay.Input;
using PixelRay.Output;

using DebugMode = PixelRay.Const.DebugMode;

static class CreatePixelRay
{
    public static void Main(string[] args)
    {
        Dictionary<string, string> values = [];
        values.Add("input", "");
        values.Add("output", "");
        values.Add("outputFormat", "");
        values.Add("preview", "");
        values.Add("previewSpeed", "0");
        values.Add("debug", "");

        // parse CLI args and update config dictionary values
        try
        {
            string first = args[0];
            string second = args[1];

            if (args[0] != "-i" || args[0] != "--image" || args[1] is not null)
            {
                string input = args[1];
                string format = input.Split(".")[1];
                if (format == "json")
                    values["input"] = input;
                else
                {
                    Console.WriteLine("Scene file must be in json format");
                    return;
                }
            }
        }
        catch (IndexOutOfRangeException)
        {
            Console.WriteLine("Scene file path must be passed first with -i <inputPath>. For example: -i scene.json");
            return;
        }

        for (int i = 2; i < args.Length; i++)
        {
            string val = args[i];

            switch (val)
            {
                case "-o" or "--output":
                    try
                    {
                        if (args[i + 1] is not null)
                        {
                            string output = args[i + 1];
                            string format = output.Split(".")[1];
                            if (format == "png" || format == "ppm")
                            {
                                values["output"] = output;
                                values["outputFormat"] = format;
                                i++;
                            }
                            else
                            {
                                Console.WriteLine("Output file must be in json/ppm format");
                                return;
                            }
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Console.WriteLine("Usage: -o <outputPath>");
                        return;
                    }
                    break;

                case "-p" or "--preview":
                    try
                    {
                        values["preview"] = "enabled";
                        string speedVal = args[i + 1];

                        if (speedVal is not null)
                        {
                            if (int.TryParse(speedVal, out int intValue))
                            {
                                string intToStr = intValue.ToString();
                                values["previewSpeed"] = intValue >= 0 ? intToStr : "0";
                                i++;
                                break;
                            }
                            {
                                Console.WriteLine($"Invalid speed value: {speedVal}");
                                return;
                            }
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Console.WriteLine("Usage: -p <value>");
                        return;
                    }
                    break;

                case "--debug":
                    try
                    {
                        if (args[i + 1] is not null)
                        {
                            string mode = args[i + 1].ToLower();
                            ReadOnlySpan<string> modes = ["normals", "distance", "id"];
                            if (modes.Contains(mode))
                            {
                                values["debug"] = mode;
                                i++;
                            }
                            else
                            {
                                Console.WriteLine("Invalid debug mode flag: use 'normals', 'distance' or 'id'");
                                return;
                            }
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Console.WriteLine("Usage: --debug <mode>");
                        return;
                    }
                    break;

                default:
                    Console.WriteLine($"Unsupported arg: {val}");
                    break;
            }
        }

        if (values["output"] == "" && values["preview"] == "")
        {
            Console.WriteLine("Input cannot be called without output/display command, use -o and/or -p flags.");
            return;
        }

        // setup
        var dto = SceneLoader.Load(values["input"]);
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

        DebugMode debug = values["debug"] switch
        {
            "normals" => DebugMode.Normals,
            "distance" => DebugMode.DepthHeat,
            "id" => DebugMode.ObjectID,
            _ => DebugMode.None
        };

        FrameBuffer buffer = renderer.Render(scene, camera, settings.Threading, upScaleFactor, debug);

        string outputFile = values["output"];
        string imageFormat = values["outputFormat"];

        if (values["output"] != "")
        {
            if (imageFormat == "ppm")
                ImageWriter.WritePPM(outputFile, buffer);
            else if (imageFormat == "png")
                ImageWriter.WritePNG(outputFile, buffer);
            else
                throw new Exception($"Unsupported image format {values["output"]}");
        }

        if (values["preview"] == "enabled")
        {
            int speed = 0;
            // parser already does the user input -> int -> str conversion. This is only to convert string -> int again
            if (int.TryParse(values["previewSpeed"], out int speedValue))
                speed = speedValue;

            // ensure upscaled resolution has even coordinates, otherwise image preview crashes
            int w = (width % 2 == 0) ? width * upScaleFactor : (width - 1) * upScaleFactor;
            int h = (height % 2 == 0) ? height * upScaleFactor : (height - 1) * upScaleFactor;

            ImageDisplay display = new(w, h, buffer, speed);
            display.Display();
        }
    }
}