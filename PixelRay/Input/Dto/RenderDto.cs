using PixelRay.Rendering;

namespace PixelRay.Input.Dto;

/// <summary>
/// Raw json data, has to be normalized to RenderSettings format (mainly because Palette must become an object)
/// </summary>
public class RenderDto
{
    public int Width { get; set; }
    public int Height { get; set; }

    public double[][] Palette { get; set; } = [[]];

    public int LightingBands { get; set; } = 4;
    public int MaxBounces { get; set; } = 1;

    public bool Dithering { get; set; } = false;
    public int DitherLevels { get; set; } = 16;
    public int DitherDimension { get; set; } = 4;
}