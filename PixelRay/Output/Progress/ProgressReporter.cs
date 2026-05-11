namespace PixelRay.Output.Progress;

/// <summary>
/// Handles rendering progress updates via a callback function
/// </summary>
public class ProgressReporter(int total, Action<RenderProgress> callback)
{
    private int _done;
    private readonly int _total = total;
    private readonly Action<RenderProgress> _callback = callback;

    public void Increment(int step = 1)
    {
        int done = Interlocked.Add(ref _done, step);

        // throttle updates (important for console performance)
        if (done % 500 == 0 || done == _total)
        {
            _callback(new RenderProgress(
                done,
                _total,
                (double)done / _total
            ));
        }
    }
}