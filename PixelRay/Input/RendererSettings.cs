using PixelRay.Rendering;

namespace PixelRay.Input;

/// <summary>
/// Settings data parsed from a render dto object
/// </summary>
public class RendererSettings
{
    public int Width { get; set; }
    public int Height { get; set; }

    public Palette Palette { get; set; } = new([]);

    public int LightingBands { get; set; } = 4;
    public int MaxBounces { get; set; } = 1;

    public bool Dithering { get; set; } = false;
    public int DitherLevels { get; set; } = 16;
    public int DitherDimension { get; set; } = 4;
}