using PixelRay.Core;
using PixelRay.Core.Mathematics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace PixelRay.Output;

/// <summary>
/// Writes data from FrameBuffer into a file
/// </summary>
public static class ImageWriter
{
    /// <summary>
    /// Write into a ppm (P3) file
    /// </summary>
    /// <param name="path">Image save path</param>
    /// <param name="buffer">Pixel buffer</param>
    public static void WritePPM(string path, FrameBuffer buffer)
    {
        using StreamWriter writer = new(path);
        writer.WriteLine($"P3\n{buffer.Width} {buffer.Height}\n255");

        for (int y = 0; y < buffer.Height; y++)
        {
            for (int x = 0; x < buffer.Width; x++)
            {
                ColorRGB color = buffer.GetPixel(x, y);

                int r = (int)(255.999 * color.R);
                int g = (int)(255.999 * color.G);
                int b = (int)(255.999 * color.B);

                writer.WriteLine($"{r} {g} {b}");
            }
        }
    }

    /// <summary>
    /// Write into a png file
    /// </summary>
    /// <param name="path">Image save path</param>
    /// <param name="buffer">Pixel Buffer</param>
    public static void WritePNG(string path, FrameBuffer buffer)
    {
        int w = buffer.Width;
        int h = buffer.Height;

        using var img = new Image<Rgb24>(w, h);

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                ColorRGB pixelColor = buffer.GetPixel(x, y);

                img[x, y] = new Rgb24(
                    (byte)(pixelColor.R * 255),
                    (byte)(pixelColor.G * 255),
                    (byte)(pixelColor.B * 255)
                );
            }
        }

        img.Save(path);
    }
}
