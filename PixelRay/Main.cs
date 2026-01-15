namespace PixelRay
{
    class CreatePixelRay
    {
        static void Main(string[] args)
        {
            Vec3 v = new(1, 2, 2);
            Vec3 w = new(2, 2, 6);
            Vec3 a = v + w;
            Console.WriteLine(a);
            Render();

        }

        static void Render()
        {
            int imageWidth = 256;
            int imageHeight = 256;
            List<(int, int, int)> image = [];

            string filePath = "../../../image.ppm";
            string pixels = "P3\n" + imageWidth + " " + imageHeight + "\n255\n"; // ppm file identifier

            // image generation
            for (int j = 0; j < imageHeight; j++)
            {
                Console.WriteLine("\rScan lines remaining: " + (imageHeight - j) + " ");
                for (int i = 0; i < imageWidth; i++)
                {
                    rgbColor pixelColor = new((double)i / (imageWidth - 1), (double)j / (imageHeight - 1), 0);
                    Color.WriteColor(image, pixelColor);
                }
            }

            pixels += string.Join("\n", image.ToArray());
            Console.WriteLine(pixels);
            File.WriteAllText(filePath, pixels); // save path
        }
    }
}
