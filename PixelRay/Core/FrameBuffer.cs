using PixelRay.Mathematics;

namespace PixelRay.Core;

public class FrameBuffer(int width, int height)
{
    public int Width { get; } = width;
    public int Height { get; } = height;

    public void SetPixel(int x, int y, ColorRGB color)
    {
        _pixels[x, y] = color;
    }

    public ColorRGB GetPixel(int x, int y)
    {
        return _pixels[x, y];
    }

    private readonly ColorRGB[,] _pixels = new ColorRGB[width, height];
}