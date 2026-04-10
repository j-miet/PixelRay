using PixelRay.Core;
using PixelRay.Core.Mathematics;

namespace PixelRay.SceneView.Hittable;

/// <summary>
/// Interface for a hittable object on scene. When implementing IHittable, <br/>
/// 1. treat ray range as closed interval <br/>
/// 2. value 0 as closed interval [-epsilon, epsilon] <br/>
/// For 2. this means compare operators follow these rules: <br/>
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
    /// Check if ray hits an object on given closed interval
    /// </summary>
    /// <param name="ray">Camera ray through viewport</param>
    /// <param name="rayT">Closed interval of accepted ray intersection values ([tMin, tMax])</param>
    /// <param name="hit">HitRecord struct. Uses keyword 'out' because it's passed by reference and its values are
    /// only initialized by the hittable's own implementation.
    /// </param>
    /// <returns>Boolean value of whether ray intersects the hittable or not</returns>
    bool Hit(Ray ray, Interval rayT, out HitRecord hit);
}