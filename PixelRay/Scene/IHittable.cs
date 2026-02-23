using PixelRay.Core;

namespace PixelRay.SceneObjects;

/// <summary>
/// Interface for hittable object on scene
/// </summary>
public interface IHittable
{
    /// <summary>
    /// Check if ray hits an object on given interval (tMin, tMax)
    /// </summary>
    /// <param name="ray">Camera ray through viewport</param>
    /// <param name="tMin">Ray starting point; in geometry objects end points are not included</param>
    /// <param name="tMax">Ray end point; end point not included</param>
    /// <param name="hit">HitRecord struct. Uses keyword 'out' because it's passed by reference and its values are
    /// only initialized by the hittable's own implementation.
    /// </param>
    /// <returns>Boolean value of whether ray intersects the hittable or not</returns>
    bool Hit(Ray ray, double tMin, double tMax, out HitRecord hit);
}