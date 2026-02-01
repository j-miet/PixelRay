namespace PixelRay;

/// <summary>
/// Abstract class for any traceable object. Requires implementation of Hit method which checks if ray hits this object.
/// Any class in PixelRay.Object must implement derive from this class.
/// </summary>
public abstract class HitObject
{
    /// <summary>
    /// Hit function abstraction for any traceable object
    /// </summary>
    /// <param name="r">Line ray</param>
    /// <param name="rayTMin">Ray line range start point; open interval so start/end are excluded</param>
    /// <param name="rayTMax">Ray line range endpoint</param>
    /// <param name="rec">HitRecord if hit line matched at least one root</param>
    /// <returns>Boolean; if ray intersected with HitObject or not</returns>
    public abstract bool Hit(Ray r, double rayTMin, double rayTMax, HitRecorder rec);
}