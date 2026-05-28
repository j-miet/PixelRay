using System.Diagnostics;

using PixelRay.Core;
using PixelRay.Debug;
using PixelRay.Input;
using PixelRay.Input.Scripting;
using PixelRay.Output;

static class CreatePixelRay
{
    public static void Main(string[] args)
    {
        Dictionary<string, string> values = [];
        values.Add("input", "");
        values.Add("output", "");
        values.Add("outputFormat", "");
        values.Add("script", "");
        values.Add("frameCount", "");
        values.Add("produceGif", "");
        values.Add("preview", "");
        values.Add("debug", "");

        // parse CLI args and update config dictionary values
        try
        {
            string first = args[0];
            string second = args[1];

            if (args[0] != "-i" || args[0] != "--image" || args[1] is not null)
            {
                string input = args[1];
                string format = Path.GetExtension(input).TrimStart('.');

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
                            string format = Path.GetExtension(output).TrimStart('.');

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

                case "-s" or "--script":
                    try
                    {
                        if (args[i + 1] is not null)
                        {
                            string scriptFile = args[i + 1];
                            string format = Path.GetExtension(scriptFile).TrimStart('.');

                            if (format == "lua")
                            {
                                values["script"] = args[i + 1];
                                i++;
                            }
                            else
                            {
                                Console.WriteLine("Script must use lua format");
                                return;
                            }
                        }

                        string flag;
                        if (args[i + 1] is not null)
                        {
                            flag = args[i + 1];
                            values["frameCount"] = flag;
                            i++;
                        }

                        if (i + 1 < args.Length && args[i + 1] is not null)
                        {
                            flag = args[i + 1];
                            if (flag == "-g" || flag == "--gif")
                            {
                                values["produceGif"] = "enabled";
                                i++;
                            }
                            else
                            {
                                Console.WriteLine("Use -g or --gif");
                                return;
                            }
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Console.WriteLine("Usage: -s <scriptLua> <frames> {-g}");
                        return;
                    }
                    break;

                case "-p" or "--preview":
                    values["preview"] = "enabled";
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
                    return;
            }
        }

        if (values["output"] == "" && values["script"] == "")
        {
            Console.WriteLine("Input cannot be called without output or script command.\n" +
                "Use -o <outputPath> or -s <luaScriptFilePath>."
            );
            return;
        }

        // setup
        var dto = SceneLoader.Load(values["input"]);
        var (scene, settings) = SceneBuilder.Build(dto);

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

        FrameBuffer buffer;

        // Lua script animations
        if (values["script"] != "")
        {
            string scriptRoot = Path.GetDirectoryName(values["script"])!;

            string gifOutputName = "outputGIF.gif";
            string frameOutputDir;
            string gifOutputPath;

            if (scriptRoot == "")
            {
                frameOutputDir = "frames";
                gifOutputPath = gifOutputName;
            }
            else
            {
                frameOutputDir = scriptRoot + Path.DirectorySeparatorChar + "frames";
                gifOutputPath = scriptRoot + Path.DirectorySeparatorChar + gifOutputName;
            }

            // create output frame dir if it doesn't already exist
            Directory.CreateDirectory(frameOutputDir);

            int totalFrames = 60;

            // flush frame directory
            Directory.CreateDirectory(frameOutputDir);
            foreach (var file in Directory.GetFiles(frameOutputDir, "*.png"))
                File.Delete(file);

            var lua = new LuaEngine();

            lua.AttachScene(scene, scene.Camera);
            lua.Load(values["script"]);

            // custom frame count; invalid output defaults to 60
            if (values["frameCount"] != "")
            {
                if (int.TryParse(values["frameCount"], out int frameVal))
                    totalFrames = frameVal >= 0 ? frameVal : 60;
            }

            for (int frame = 0; frame < totalFrames; frame++)
            {
                lua.Update(frame);

                buffer = renderer.Render(scene, scene.Camera, settings.Threading, upScaleFactor, debug, frame);

                ImageWriter.WritePNG($"{frameOutputDir}{Path.DirectorySeparatorChar}frame-{frame}.png", buffer);
            }

            // produce a GIF from frames
            if (values["produceGif"] == "enabled")
                GifBuilder.Build(frameOutputDir, gifOutputPath);

            return;
        }

        buffer = renderer.Render(scene, scene.Camera, settings.Threading, upScaleFactor, debug);

        string outputPath = values["output"];
        string imageFormat = values["outputFormat"];

        if (values["output"] != "")
        {
            if (imageFormat == "ppm")
                ImageWriter.WritePPM(outputPath, buffer);
            else if (imageFormat == "png")
            {
                ImageWriter.WritePNG(outputPath, buffer);

                if (values["preview"] == "enabled")
                {
                    // set directory to path so that shell can just call "<outputFile>.png" below
                    try
                    {
                        Directory.SetCurrentDirectory(Path.GetDirectoryName(outputPath)!);
                    }
                    catch (ArgumentException) { } // directory is already "." so continue

                    string outputFile = Path.GetFileName(outputPath);

                    // open output png image with default image viewing tool via shell execution
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = outputFile,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to open image: {ex.Message}");
                    }
                }
            }
            else
                throw new Exception($"Unsupported image format {values["output"]}");
        }
    }
}