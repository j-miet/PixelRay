namespace PixelRay.Output.Progress;

/// <summary>
/// Data record for rendering progression updates
/// </summary>
public readonly record struct RenderProgress(
    int Frame,
    int Done,
    int Total,
    double Percent
);