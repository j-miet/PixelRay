using System.Text.Json;
using PixelRay.Input.Dto;
using PixelRay.Input.DtoConverters;

namespace PixelRay.Input;

/// <summary>
/// Loads scene from a Json file
/// </summary>
public static class SceneLoader
{
    private static JsonSerializerOptions? options;

    public static SceneViewDto Load(string path)
    {
        string json;
        try
        {
            json = File.ReadAllText(path);
        }
        catch (FileNotFoundException)
        {
            throw new Exception($"Input json file not found on path: {path}");
        }

        options ??= new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        options.Converters.Add(new ObjectDtoConverter());
        options.Converters.Add(new LightDtoConverter());
        options.Converters.Add(new MaterialDtoConverter());

        return JsonSerializer.Deserialize<SceneViewDto>(json, options) ?? throw new JsonException("Invalid json");
    }
}