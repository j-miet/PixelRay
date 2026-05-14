using System.Text.Json;
using System.Text.Json.Serialization;
using PixelRay.Input.Dto.Instances;

namespace PixelRay.Input.DtoConverters;

/// <summary>
/// For processing object json data into dtos
/// </summary>
public class InstanceDtoConverter : JsonConverter<IInstanceDto>
{
    public override IInstanceDto Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        string type = root.GetProperty("type").GetString() ?? throw new Exception("Missing type field in object");

        return type switch
        {
            "sphere" => JsonSerializer.Deserialize<SphereDto>(root.GetRawText(), options)!,
            "plane" => JsonSerializer.Deserialize<PlaneDto>(root.GetRawText(), options)!,
            "disc" => JsonSerializer.Deserialize<DiscDto>(root.GetRawText(), options)!,
            "triangle" => JsonSerializer.Deserialize<TriangleDto>(root.GetRawText(), options)!,
            "cylinder" => JsonSerializer.Deserialize<CylinderDto>(root.GetRawText(), options)!,
            "cone" => JsonSerializer.Deserialize<ConeDto>(root.GetRawText(), options)!,
            "torus" => JsonSerializer.Deserialize<TorusDto>(root.GetRawText(), options)!,
            "quadric" => JsonSerializer.Deserialize<QuadricDto>(root.GetRawText(), options)!,
            "aabox" => JsonSerializer.Deserialize<AABoxDto>(root.GetRawText(), options)!,
            _ => throw new Exception($"Unknown object type: {type}")
        };
    }

    public override void Write(Utf8JsonWriter writer, IInstanceDto value, JsonSerializerOptions options)
        => throw new NotImplementedException();
}