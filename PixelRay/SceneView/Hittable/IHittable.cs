using PixelRay.Core;
using PixelRay.Core.Mathematics;

namespace PixelRay.SceneView.Hittable;

/// <summary>
/// Interface for a hittable object on scene. When implementing IHittable, the standard for this codebase is to use 
/// closed intervals when including boundary values: <br/>
/// 1. tMin LTOR t LTOR tMax, or equally, t in [tMin, tMax] <br/>
/// 2. value = 0 ==> AbsoluteVal(value) LTOR epsilon, or equally, value in [-eps, eps] <br />
/// 3. value LT 0 ==> value LT -epsilon --- value > 0 ==> value > epsilon, or equally, value in (eps, inf) <br />
/// 4. value LTOR 0 ==> value LTOR epsilon, or equally value in [epsilon, inf)<br/>
/// OR value >= 0 ==> value > -epsilon, or equally, value in (-eps, inf)<br/><br/>
/// --- Here LT = less/greater than, LTOR = less than or equal; docstring doesn't allow 'less than' sign <br/><br/>
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
    bool Hit(Ray ray, Interval rayT, out HitRecord hit);
}