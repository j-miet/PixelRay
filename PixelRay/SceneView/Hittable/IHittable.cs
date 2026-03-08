using PixelRay.Core;

namespace PixelRay.SceneView.Hittable;

/// <summary>
/// Interface for a hittable object on scene. When implementing IHittable, the standard is to use closed intervals to
/// include values: <br />
/// 1. tMin LTOR t LTOR tMax <br />
/// 2. value = 0 ==> AbsoluteVal(value) LTOR epsilon <br />
/// 3. value LT 0 ==> value LT -epsilon --- value > 0 ==> value > epsilon <br />
/// 4. value LTOR 0 ==> value LTOR epsilon --- value >= 0 ==> value > -epsilon
/// (Here LT = less/greater than, LTOR = less than or equal; docstring doesn't allow 'less than' sign) <br />
/// **It's important to stay consistent with this standard** so that excluding values is clear and won't cause bugs
/// </summary>
public interface IHittable
{
    /// <summary>
    /// Check if ray hits an object on given interval (tMin, tMax)
    /// </summary>
    /// <param name="ray">Camera ray through viewport</param>
    /// <param name="tMin">Ray starting point; in geometry objects start/end points are included</param>
    /// <param name="tMax">Ray end point; is included</param>
    /// <param name="hit">HitRecord struct. Uses keyword 'out' because it's passed by reference and its values are
    /// only initialized by the hittable's own implementation.
    /// </param>
    /// <returns>Boolean value of whether ray intersects the hittable or not</returns>
    bool Hit(Ray ray, double tMin, double tMax, out HitRecord hit);
}