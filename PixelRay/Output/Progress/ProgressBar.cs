namespace PixelRay.Output.Progress;

/// <summary>
/// Visual update bar for terminal
/// </summary>
public class ProgressBar(int width = 30)
{
    private readonly int _width = width;

    public void Update(RenderProgress p)
    {
        int filled = (int)(p.Percent * _width);

        Console.Write("\r[");
        Console.Write(new string('#', filled));
        Console.Write(new string('-', _width - filled));
        Console.Write($"] {p.Percent:P1} ({p.Done}/{p.Total})");
    }
}