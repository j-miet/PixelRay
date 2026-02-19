using PixelRay.Core;
using PixelRay.Mathematics;

namespace PixelRay.Ouput
{
    /// <summary>
    /// Writes data from FrameBuffer into a file
    /// </summary>
    public static class ImageWriter
    {
        /// <summary>
        /// Write buffer data into PPM file
        /// </summary>
        /// <param name="path">Image save path</param>
        /// <param name="buffer"></param>
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
    }
}