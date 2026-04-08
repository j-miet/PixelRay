namespace PixelRay.Core.Mathematics;

/// <summary>
/// General real-valued interval from min to max. Can check conditions for closed, open and half-open types.
/// </summary>
public readonly struct Interval(double min = double.NegativeInfinity, double max = double.PositiveInfinity)
{
    public readonly double Min = min;
    public readonly double Max = max;

    public bool InClosed(double t) => Min <= t && t <= Max;
    public bool InOpen(double t) => Min < t && t < Max;
    public bool InLeftOpen(double t) => Min < t && t <= Max;
    public bool InRightOpen(double t) => Min <= t && t < Max;
}