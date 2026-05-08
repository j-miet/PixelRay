using PixelRay.Core.Mathematics;

namespace PixelRay.Rendering;

/// <summary>
/// Dithering (currently only for ordered dithering) tools
/// </summary>
public static class Dither
{
    /// <summary>
    /// Return threshold value
    /// </summary>
    /// <param name="dim">Bayer matrix dimension. Only values 4 and 8 are supported.</param>
    public static double GetThreshold(int x, int y, int dim)
    {
        int value;
        if (dim == 8)
            value = BayerM8[x % 8, y % 8];
        else
            value = BayerM4[x % 4, y % 4];

        // add 0.5 to each value: this way each range is normalized to the center which avoids bias
        return (value + 0.5) / (dim * dim);
    }

    /// <summary>
    /// Quantizes a color by applying the Bayer bias threshold then snapping to closest step level
    /// </summary>
    /// <param name="c">RGB color component value</param>
    /// <param name="levels">Quantization levels</param>
    /// <param name="threshold">Boundary threshold</param>
    /// <returns></returns>
    public static double DitherQuantize(double c, int levels, double threshold)
    {
        // map color to discrete steps and apply boundary shifting
        double scaled = c * (levels - 1) + threshold;

        return Math.Clamp(Math.Round(scaled) / (levels - 1), 0.0, 1.0);
    }

    // 4x4 Bayer Matrix, minus the 1/16 factor (has to be applied separately)
    private static readonly int[,] BayerM4 =
    {
        {  0,  8,  2, 10 },
        { 12,  4, 14,  6 },
        {  3, 11,  1,  9 },
        { 15,  7, 13,  5 }
    };

    // 8x8 Bayer Matrix, minus the 1/64 factor (has to be applied separately)
    private static readonly int[,] BayerM8 =
    {
        {  0, 32,  8, 40,  2, 34, 10, 42 },
        { 48, 16, 56, 24, 50, 18, 58, 26 },
        { 12, 44,  4, 36, 14, 46,  6, 38 },
        { 60, 28, 52, 20, 62, 30, 54, 22 },
        {  3, 35, 11, 43,  1, 33,  9, 41 },
        { 51, 19, 59, 27, 49, 17, 57, 25 },
        { 15, 47,  7, 39, 13, 45,  5, 37 },
        { 63, 31, 55, 23, 61, 29, 53, 21 }
    };
}