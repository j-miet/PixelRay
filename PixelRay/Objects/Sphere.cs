namespace PixelRay.Objects;

/// <summary>
/// 3D ball object i.e. a sphere
/// </summary>
/// <param name="center">center point</param>
/// <param name="radius">radius</param>
public class Sphere(Point3 center, double radius) : HitObject
{
    public override bool Hit(Ray r, double rayTMin, double rayTMax, HitRecorder rec)
    {
        Vec3 oc = _center - r.Origin;
        // here the coefficients are from a quadratic equation a*t^2 + bt + c = 0, with substitution b = -2*h
        // Then solutions become of form { (h +- sqrt(h^2-a*c)) / a }.
        // This way the final coefficients become easier to calculate (less operations)
        // Another simplification is also used: dot(v, v) = |v|^2
        double a = r.Direction.LengthSquared();
        double h = Vec3.Dot(r.Direction, oc);
        double c = oc.LengthSquared() - radius * radius;

        double discriminant = h * h - a * c;
        if (discriminant < 0)
        {
            return false;
        }

        // Find nearest root in acceptable range i.e. open interval (rayTMin, rayTMax)
        double sqrtDiscriminant = Math.Sqrt(discriminant);
        double root = (h - sqrtDiscriminant) / a;
        if (root <= rayTMin || root >= rayTMax)
        {
            root = (h + sqrtDiscriminant) / a;
            if (root <= rayTMin || root >= rayTMax)
            {
                return false;
            }
        }

        rec.t = root;
        rec.p = r.At(rec.t);
        rec.normal = (rec.p - _center) / _radius;

        return true;
    }

    private readonly Point3 _center = center;
    public Point3 Center { get => _center; }
    private readonly double _radius = Math.Max(0, radius);
    public double Radius { get => _radius; }
}
