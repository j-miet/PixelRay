using PixelRay.Input.Dto.Lights;
using PixelRay.Input.Dto.Objects;

namespace PixelRay.Input.Dto;

public class SceneViewDto
{
    public required CameraDto Camera { get; set; }
    public required RenderDto Render { get; set; }
    // no objects or lights required: keep this, maybe useful for testing
    public List<IObjectDto>? Objects { get; set; }
    public List<ILightDto>? Lights { get; set; }
}