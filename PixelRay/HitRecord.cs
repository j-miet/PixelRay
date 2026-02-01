namespace PixelRay;

/// <summary>
/// Storage class to keep records of most recent ray hit
/// </summary>
public class HitRecorder
{
    public Point3? p;
    public Vec3? normal;
    public double t;
}