using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Gif;

namespace PixelRay.Output;

public static class GifBuilder
{
    public static void Build(
        string framesDir,
        string outputPath,
        int frameDelay = 5)
    {
        var files = Directory
            .GetFiles(framesDir, "*.png")
            .OrderBy(f =>
            {
                var name = Path.GetFileNameWithoutExtension(f);
                var digits = new string(
                    [.. name.SkipWhile(c => !char.IsDigit(c)).TakeWhile(char.IsDigit)]
                );
                return int.TryParse(digits, out var n) ? n : 0;
            })
            .ToList();

        if (files.Count == 0)
            throw new Exception("No PNG frames found.");

        using var first = Image.Load<Rgba32>(files[0]);
        using var gif = first.Clone();

        ConfigureFrame(gif.Frames.RootFrame, frameDelay);

        gif.Metadata.GetGifMetadata().RepeatCount = 0;

        foreach (var file in files.Skip(1))
        {
            using var img = Image.Load<Rgba32>(file);
            using var clone = img.Clone();

            ConfigureFrame(clone.Frames.RootFrame, frameDelay);

            gif.Frames.AddFrame(clone.Frames.RootFrame);
        }

        gif.Save(
            outputPath,
            new GifEncoder
            {
                ColorTableMode = GifColorTableMode.Local
            }
        );
    }

    private static void ConfigureFrame(
        ImageFrame<Rgba32> frame,
        int delay)
    {
        var meta = frame.Metadata.GetGifMetadata();
        meta.FrameDelay = delay;
        meta.DisposalMethod = GifDisposalMethod.RestoreToBackground;
    }
}