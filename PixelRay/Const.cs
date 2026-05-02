namespace PixelRay;

/// <summary>
/// Global constants and enums
/// </summary>
public static class Const
{
    public enum DebugMode
    {
        None,
        Normals,            // visualize surface normals
        DepthHeat,          // visualize distance along ray
        ObjectID            // visualize which object was hit
    }

}