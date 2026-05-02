using System.Text.Json;
using System.Text.Json.Serialization;
using PixelRay.Input.Dto.Objects;

namespace PixelRay.Input.DtoConverters;

/// <summary>
/// For processing object json data into dtos
/// </summary>
public class ObjectDtoConverter : JsonConverter<IObjectDto>
{
    public override IObjectDto Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        string type = root.GetProperty("type").GetString() ?? throw new Exception("Missing type field in object");

        return type switch
        {
            "sphere" => JsonSerializer.Deserialize<SphereDto>(root.GetRawText(), options)
                ?? throw new Exception("Sphere deserialization failed"),
            "plane" => JsonSerializer.Deserialize<PlaneDto>(root.GetRawText(), options)
                ?? throw new Exception("Plane deserialization failed"),
            "disc" => JsonSerializer.Deserialize<DiscDto>(root.GetRawText(), options)
                ?? throw new Exception("Disc deserialization failed"),
            "triangle" => JsonSerializer.Deserialize<TriangleDto>(root.GetRawText(), options)
                ?? throw new Exception("Triangle deserialization failed"),
            "cylinder" => JsonSerializer.Deserialize<CylinderDto>(root.GetRawText(), options)
                ?? throw new Exception("Cylinder deserialization failed"),
            "cone" => JsonSerializer.Deserialize<ConeDto>(root.GetRawText(), options)
                ?? throw new Exception("Cone deserialization failed"),
            "torus" => JsonSerializer.Deserialize<TorusDto>(root.GetRawText(), options)
                ?? throw new Exception("Torus deserialization failed"),
            "quadric" => JsonSerializer.Deserialize<QuadricDto>(root.GetRawText(), options)
                ?? throw new Exception("Quadric deserialization failed"),
            "aabox" => JsonSerializer.Deserialize<AABoxDto>(root.GetRawText(), options)
                ?? throw new Exception("AABox deserialization failed"),
            _ => throw new Exception($"Unknown object type: {type}")
        };
    }

    public override void Write(Utf8JsonWriter writer, IObjectDto value, JsonSerializerOptions options)
        => throw new NotImplementedException();
}