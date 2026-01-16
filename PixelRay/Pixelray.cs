namespace PixelRay
{
    using rgbColor = Vec3;
    using Point3 = Vec3;

    public class PixelRay
    {
        public PixelRay()
        {
            _aspectRatio = 16.0 / 9.0;
            _imageWidth = 400;
            _focalLength = 1.0;
            _viewportHeight = 2.0;
            _outputFilePath = "../../../image.ppm"; ; // TODO change this to point into bin folder
        }

        public PixelRay(double aspectRatio,
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
            int imageHeight = (int)(_imageWidth / _aspectRatio);
            imageHeight = (imageHeight < 1) ? 1 : imageHeight;

            // Camera
            double viewportWidth = _viewportHeight * ((double)_imageWidth / imageHeight);
            Point3 cameraCenter = new(0, 0, 0);

            // Calculate the vectors across the horizontal and down the vertical viewport edges
            Vec3 viewportW = new(viewportWidth, 0, 0);
            Vec3 viewportH = new(0, -_viewportHeight, 0);

            // Calculate the horizontal and vertical delta vectors from pixel to pixel
            Vec3 pixelDeltaW = viewportW / _imageWidth;
            Vec3 pixelDeltaH = viewportH / imageHeight;

            // Calculate the location of the upper left pixel
            Point3 viewportUpperLeft = cameraCenter - new Vec3(0, 0, _focalLength) - viewportW / 2 - viewportH / 2;
            Point3 pixelUpperLeft = viewportUpperLeft + 0.5 * (pixelDeltaW + pixelDeltaH); // TODO Check this later

            // Render

            List<string> image = [];
            string filePath = _outputFilePath;
            string pixels = "P3\n" + _imageWidth + " " + imageHeight + "\n255\n"; // ppm file identifier

            for (int j = 0; j < imageHeight; j++)
            {
                //Console.WriteLine("\rScan lines remaining: " + (imageHeight - j) + " ");
                for (int i = 0; i < _imageWidth; i++)
                {
                    Point3 pixelCenter = pixelUpperLeft + (i * pixelDeltaW) + (j * pixelDeltaH);
                    Vec3 rayDirection = pixelCenter - cameraCenter;
                    Ray ray = new(cameraCenter, rayDirection);

                    rgbColor pixelColor = RayColor(ray);
                    Color.WriteColor(image, pixelColor);
                }
            }

            pixels += string.Join("\n", image.ToArray());
            //Console.WriteLine(pixels);
            File.WriteAllText(filePath, pixels); // save path

            Console.WriteLine("\rDone.\n");
        }

        public static rgbColor RayColor(Ray r)
        {
            Vec3 unitDirection = Vec3.Unit(r.Direction);
            double scalar = 0.5 * (unitDirection.Y + 1.0);
            return (1.0 - scalar) * new rgbColor(1.0, 1.0, 1.0) + scalar * new rgbColor(0.5, 0.7, 1.0);
        }

        private readonly double _aspectRatio;
        private readonly int _imageWidth;
        private readonly double _focalLength;
        private readonly double _viewportHeight;
        private readonly string _outputFilePath;
    }
}