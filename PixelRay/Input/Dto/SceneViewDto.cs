using PixelRay.Input.Dto.Instances;
using PixelRay.Input.Dto.Lights;

namespace PixelRay.Input.Dto;

public class SceneViewDto
{
    public required CameraDto Camera { get; set; }
    public required RenderDto Render { get; set; }
    // no objects or lights required: keep this, maybe useful for testing
    public List<IInstanceDto>? Objects { get; set; }
    public List<ILightDto>? Lights { get; set; }
}