global using RGBColor = PixelRay.Vec3;
global using Point3 = PixelRay.Vec3;
using PixelRay.Objects;

namespace PixelRay;

public class RayTracer
{
    public RayTracer()
    {
        _aspectRatio = 16.0 / 9.0;
        _imageWidth = 400;
        _focalLength = 1.0;
        _viewportHeight = 2.0;
        _outputFilePath = "../../../Imgs/ImageGradientSphereNormals.ppm"; // TODO change this to point into bin folder
    }

    public RayTracer(
        double aspectRatio,
        int imageWidth,
        double focalLength,
        double viewportHeight,
        string outputFilePath)
    {
        _aspectRatio = aspectRatio;
        _imageWidth = imageWidth;
        _focalLength = focalLength;
        _viewportHeight = viewportHeight;
        _outputFilePath = outputFilePath;
    }

    public void Render()
    {
        // Define static image and viewport data
        int imageHeight = (int)(_imageWidth / _aspectRatio);
        imageHeight = (imageHeight < 1) ? 1 : imageHeight;

        double viewportWidth = _viewportHeight * ((double)_imageWidth / imageHeight);
        Point3 cameraCenter = new(0, 0, 0);

        Vec3 viewportW = new(viewportWidth, 0, 0);
        Vec3 viewportH = new(0, -_viewportHeight, 0);

        Vec3 pixelDeltaW = viewportW / _imageWidth;
        Vec3 pixelDeltaH = viewportH / imageHeight;

        Point3 viewportUpperLeft = cameraCenter - new Vec3(0, 0, _focalLength) - viewportW / 2 - viewportH / 2;
        Point3 pixelUpperLeft = viewportUpperLeft + 0.5 * (pixelDeltaW + pixelDeltaH);

        // Rendering
        List<string> image = [];
        string filePath = _outputFilePath;
        string pixels = "P3\n" + _imageWidth + " " + imageHeight + "\n255\n"; // ppm file identifier

        for (int j = 0; j < imageHeight; j++)
        {
            Console.WriteLine("\rScan lines remaining: " + (imageHeight - j) + " ");
            for (int i = 0; i < _imageWidth; i++)
            {
                Point3 pixelCenter = pixelUpperLeft + (i * pixelDeltaW) + (j * pixelDeltaH);
                Vec3 rayDirection = pixelCenter - cameraCenter;
                Ray ray = new(cameraCenter, rayDirection);

                Sphere sphere = new(new Point3(0, 0, -1), 0.5);
                HitRecorder rec = new();
                RGBColor pixelColor;

                sphere.Hit(ray, 0, sphere.Center.Length() + sphere.Radius, rec);
                pixelColor = RayColor(ray, rec);

                Color.AddColor(image, pixelColor);
            }
        }

        pixels += string.Join("\n", image.ToArray());
        File.WriteAllText(filePath, pixels); // save path

        Console.WriteLine("\rDone.\n");
    }

    /// <summary>
    /// Define RGBColor based on ray and HitRecord data
    /// </summary>
    /// <param name="r">Ray object</param>
    /// <param name="rec">HitRecord object</param>
    /// <returns></returns>
    public static RGBColor RayColor(Ray r, HitRecorder rec)
    {
        if (rec.normal != null && rec.t > 0.0)
        {
            Vec3 N = rec.normal; // unit normal of ray intersection with hit object
            return 0.5 * new RGBColor(N.X + 1, N.Y + 1, N.Z + 1);
        }

        // default background gradient if ray hits no object
        Vec3 unitDirection = Vec3.Unit(r.Direction);
        double a = 0.5 * (unitDirection.Y + 1.0);
        return (1.0 - a) * new RGBColor(1.0, 1.0, 1.0) + a * new RGBColor(0.5, 0.7, 1.0);
    }

    private readonly double _aspectRatio;
    private readonly int _imageWidth;
    private readonly double _focalLength;
    private readonly double _viewportHeight;
    private readonly string _outputFilePath;
}