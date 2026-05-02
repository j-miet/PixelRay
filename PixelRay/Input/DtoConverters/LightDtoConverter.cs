using System.Text.Json;
using System.Text.Json.Serialization;
using PixelRay.Input.Dto.Lights;

namespace PixelRay.Input.DtoConverters;

/// <summary>
/// For processing light json data into dtos
/// </summary>
public class LightDtoConverter : JsonConverter<ILightDto>
{
    public override ILightDto Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        string type = root.GetProperty("type").GetString() ?? throw new Exception("Missing type field in light");

        return type switch
        {
            "directional" => JsonSerializer.Deserialize<DirectionalLightDto>(root.GetRawText(), options)
                ?? throw new Exception("DirectionalLight deserialization failed"),
            "ambient" => JsonSerializer.Deserialize<AmbientLightDto>(root.GetRawText(), options)
                ?? throw new Exception("AmbientLight deserialization failed"),
            "point" => JsonSerializer.Deserialize<PointLightDto>(root.GetRawText(), options)
                ?? throw new Exception("PointLight deserialization failed"),
            _ => throw new Exception($"Unknown light type: {type}")
        };
    }

    public override void Write(Utf8JsonWriter writer, ILightDto value, JsonSerializerOptions options)
        => throw new NotImplementedException();
}