namespace PixelRay.Output.Progress;

/// <summary>
/// Handles rendering progress updates via a callback function
/// </summary>
/// <param name="updateRate">How many pixels to read before sending progress update. Default is 500, internal lower
/// bound is 100: values below this and probably even a bit above it will cause a lot of flickering in progress 
/// prints. If updateRate >= total, progress will only update once from 0% to 100%
/// Any value < 100 will default to 500 except 0 which sets updateRate = total
/// </param>
public class ProgressReporter(int frame, int total, Action<RenderProgress> callback, int updateRate = 500)
{
    private int _done;
    private readonly int _total = total;
    private readonly int _updateRate = updateRate == 0 ? total : updateRate >= 100 ? updateRate : 500;
    private readonly Action<RenderProgress> _callback = callback;

    public void Increment(int step = 1)
    {
        int done = Interlocked.Add(ref _done, step);

        // throttle updates (important for console performance)
        if (done % _updateRate == 0 || done == _total)
        {
            _callback(new RenderProgress(
                frame,
                done,
                _total,
                (double)done / _total
            ));
        }
    }
}