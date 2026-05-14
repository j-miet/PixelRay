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
    public static (Scene scene, RendererSettings settings) Build(SceneViewDto dto)
    {
        if (dto.Camera == null)
            throw new Exception("Camera is missing from scene file");

        Scene scene = new()
        {
            Camera = BuildCamera(dto.Camera, dto.Render)
        };

        int id = 0;

        if (dto.Objects != null)
        {
            foreach (var obj in dto.Objects)
            {
                var instance = obj.Build();

                instance.Id = id++;

                scene.AddObject(instance);

                if (!string.IsNullOrEmpty(instance.Name))
                    scene.NameLookup[instance.Name] = instance.Id;
            }
        }

        if (dto.Lights != null)
        {
            foreach (var light in dto.Lights)
                scene.AddLight(light.Build());
        }

        RendererSettings settings = LoadRendererSettings(dto.Render);

        return (scene, settings);
    }

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

    private static Camera BuildCamera(CameraDto cam, RenderDto render)
    {
        return new Camera(
            ToVec3(cam.Position),
            ToVec3(cam.LookAt),
            ToVec3(cam.UpDirection),
            cam.Fov,
            (double)render.Width / render.Height
        );
    }
}