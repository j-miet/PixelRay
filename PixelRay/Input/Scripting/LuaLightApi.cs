using PixelRay.Core.Mathematics;
using PixelRay.SceneView.Lighting;

namespace PixelRay.Input.Scripting;

/// <summary>
/// Light API
/// </summary>
public class LuaLightApi(ILight light)
{
    public void Color(double x, double y, double z) => _light.Color = new ColorRGB(x, y, z);
    public void Intensity(double value) => _light.Intensity = value;

    public void Direction(double x, double y, double z)
    {
        if (_light is DirectionalLight dir)
            dir.Direction = new(x, y, z); // from hit to source
        else if (_light is SpotLight spot)
            spot.Direction = new(x, y, z); // from source to hit
    }

    // SpotLight inherits from PointLight so these all work on both
    public void Position(double x, double y, double z)
    {
        if (_light is PointLight point)
        {
            point.Position = new(x, y, z);
        }
    }

    public void LightRadius(double radius)
    {
        if (_light is PointLight point)
        {
            point.LightRadius = radius;
        }
    }

    public void ShadowBands(int bands)
    {
        if (_light is PointLight point)
        {
            point.ShadowBands = bands;
        }
    }

    // SpotLight
    public void OuterAngle(double angle)
    {
        if (_light is SpotLight spot)
        {
            spot.OuterAngle = angle;
        }
    }

    public void InnerAngle(double angle)
    {
        if (_light is SpotLight spot)
        {
            spot.InnerAngle = angle;
        }
    }

    private readonly ILight _light = light;

}