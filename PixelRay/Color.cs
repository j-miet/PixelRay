namespace PixelRay;

static class Color
{
    public static void WriteColor(List<string> image, rgbColor color)
    {
        double r = color[0];
        double g = color[1];
        double b = color[2];

        // scale colors from [0, 255] to [0, 1]
        int rByte = (int)(255.999 * r);
        int gByte = (int)(255.999 * g);
        int bByte = (int)(255.999 * b);

        image.Add(rByte + " " + gByte + " " + bByte);
    }
}
