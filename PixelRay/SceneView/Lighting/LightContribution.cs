namespace PixelRay.SceneView.Lighting;

public readonly record struct LightContribution(
    double Shading,
    double Attenuation = 1.0
);