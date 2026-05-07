using PixelRay.Core.Mathematics;
using PixelRay.Input.Dto;
using PixelRay.Rendering;
using PixelRay.SceneView;

using static PixelRay.Input.InputUtils;

namespace PixelRay.Input;

/// <summary>
/// Scene creation
/// </summary>
public static class SceneBuilder
{
    /// <summary>
    /// Builds scene from a dto
    /// </summary>
    /// <param name="dto">Data transfer object produced from input file</param>
    public static (Scene scene, Camera camera, RendererSettings settings) Build(SceneViewDto dto)
    {
        if (dto.Camera == null)
            throw new Exception("Camera is missing from scene file");

        Scene scene = new();

        Camera camera = new(
            ToVec3(dto.Camera.Position),
            ToVec3(dto.Camera.LookAt),
            ToVec3(dto.Camera.UpDirection),
            dto.Camera.Fov,
            (double)dto.Render.Width / dto.Render.Height
        );

        if (dto.Objects != null)
        {
            foreach (var obj in dto.Objects)
                scene.AddObject(obj.Build());
        }

        if (dto.Lights != null)
        {
            foreach (var light in dto.Lights)
                scene.AddLight(light.Build());
        }

        RendererSettings settings = LoadRendererSettings(dto.Render);

        return (scene, camera, settings);
    }

    /// <summary>
    /// Creates an settings object for easy rendering settings access
    /// </summary>
    private static RendererSettings LoadRendererSettings(RenderDto dto)
    {
        List<ColorRGB> colors = [];
        foreach (var p in dto.Palette)
        {
            colors.Add(ToColor(p));
        }

        RendererSettings settings = new()
        {
            Width = dto.Width,
            Height = dto.Height,
            UpScaleFactor = dto.UpScaleFactor,
            Palette = new Palette([.. colors]),

            LightingBands = dto.LightingBands,
            MaxBounces = dto.MaxBounces,

            Dithering = dto.Dithering,
            DitherLevels = dto.DitherLevels,
            DitherDimension = dto.DitherDimension,

            Threading = dto.Threading
        };

        return settings;

    }
}