using System.Text.Json;
using System.Text.Json.Serialization;
using PixelRay.Input.Dto.Materials;

namespace PixelRay.Input.DtoConverters;

/// <summary>
/// For processing material json data into dtos
/// </summary>
public class MaterialDtoConverter : JsonConverter<IMaterialDto>
{
    public override IMaterialDto Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        string type = root.GetProperty("type").GetString() ?? throw new Exception("Missing type field in material");

        return type switch
        {
            "surface" => JsonSerializer.Deserialize<SurfaceMaterialDto>(root.GetRawText(), options)!,
            "mirror" => JsonSerializer.Deserialize<MirrorMaterialDto>(root.GetRawText(), options)!,
            _ => throw new Exception($"Unknown material type: {type}")
        };
    }

    public override void Write(Utf8JsonWriter writer, IMaterialDto value, JsonSerializerOptions options)
        => throw new NotImplementedException();
}