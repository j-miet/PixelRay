![](docs/Images/Demo/default.png)

> More demo images can be found [here](docs/Images/Demo/)

## Table of contents

- [<u>How to use</u>](#how-to-use)
- [<u>Scene</u>](#scene)
- [<u>Pixel-look</u>](#pixel-look)
- [<u>Building from source</u>](#building-from-source)
- [<u>Running tests</u>](#running-tests)
- [<u>Future additions</u>](#future-additions)


## How to use

To produce an output image, paths for scene and output files are required:

```bash
PixelRay -i <inputPath> -o <outputPath>
```

If you want to automatically open the output after rendering, add the `-p`/`--preview` flag. This will attempt to 
call default image opener program via shell execution:

```bash
PixelRay -i scene.json -o output.png -p
```


#### All commands

- `-i <path>` or `--image <path>`

    Scene file path. Scenes are stored in **json** files (see Scenes section below).

- `-o <path>` or `--output <path>` 

    Output file path. Image format is either **png** or **ppm**-
- `-p` or `--preview`:

    Attempts to open the output image after rendering by executing the image file, thus calling the default viewing 
    tool in process. Only available for png images as basic editors seldom support ppm.
- `--debug <mode>`
    
    Renders image based on selected debug mode. These are:
    - `normals` = color objects based on normal directions. Red = x, Green = y, Blue = z
    - `distance` = color objects based on how far they're from camera. Uses gray scale: closer objects are white/light 
    gray and distant objects become darker. Missed rays produce red pixels.
    - `id` = each object gets a random color

    Modes cannot be mixed, only first one gets applied.


## Scene

Scenes use **json** format.
Default scene file can be found [here](docs/scene.json).

For all scene parameters and their explanations, see [Scene.md](docs/Scene.md)

## Pixel-look

To enforce pixelated theme:

- low resolutions only, use nearest-neighbor upscaling for higher resolutions
- lighting quantization
- hard shadows + simple soft shadow logic with fixed disc offsets instead of sampling. Adjustable light radius and
 shadow quantization bands
    - soft shadows can look quite rough and out of place without careful planning
- keep reflection bounces low
    - can always use ambient/directional lights if scene doesn't allow proper bouncing e.g. lack of reflective 
    surfaces or open scene where rays bounce to infinity
- ordered dithering
- custom color palettes. Works just fine without, but these can certainly change the aesthetics a lot

=> no anti-aliasing, resampling or realistic lighting (global illumination). Focus is on aliasing the initial output
 to achieve a stylistic look

## Building from source

`SixLabors.ImageSharp` is the only required third-party package
and happens to be well-supported on both Linux and macOS. It's used for producing output images in PNG format

---

To build the executable, you need to install [.NET SDK](https://learn.microsoft.com/en-us/dotnet/core/install/linux). 
PixelRay uses version 10.0.


Build command for .NET is

> dotnet publish -c Release -r \<rid\> \<args\>

There are generally two ways to build into an executable:
1. Self-contained exe. This dll is SILK.NET native dependency and needs be included
    - larger executable (~37 MB on Windows, ~39 MB on Ubuntu) but runs on its own, no .NET runtime installation 
    required for end user
2. minimal exe + DLLs 
    - very small, but end user must have **.NET 10.0 Runtime** installed

Instead of modifying *PixelRay.csproj* for each, both can be done by passing additional args.

- for args specification check 
[this](https://learn.microsoft.com/en-us/dotnet/core/deploying/single-file/overview?tabs=cli)
- examples below use 64-bit Windows as runtime identifier; for other RIDs, see 
[this](https://learn.microsoft.com/en-us/dotnet/core/rid-catalog#known-rids)

#### Powershell

1. Self-contained:
    ```powershell
    dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=true
    ```

2. minimal, but requires .NET runtime
    ```powershell
    dotnet publish -c Release -r win-x64 --self-contained false
    ```

#### Bash

No differences here, same commands work:

1. Self-contained
    ```bash
    dotnet publish -c Release -r win-x64 \
      --self-contained true \
      -p:PublishSingleFile=true \
      -p:EnableCompressionInSingleFile=true
    ```

2. minimal, but requires .NET
    ```bash
    dotnet publish -c Release -r win-x64 \
      --self-contained false
    ```

#### Output

Release build can be found in "PixelRay/bin/Release/net10.0/win-x64/publish" or similar depending on which RID was 
used e.g. linux-x64 instead of win-x64


## Running tests

[.NET SDK](https://learn.microsoft.com/en-us/dotnet/core/install/linux) version 10.0 is required.

[xUnit](https://xunit.net/docs/getting-started/v3/getting-started) is used for testing. You don't need to download 
this manually, tests.csproj includes all dependencies.

**To run tests:** cd into "PixelRay/tests" then use command `dotnet test`

## Future additions

The goal of this project is not to become become a large, heavily optimized ray/path tracing tool yet it should 
still have a good amount of customization options. So here's a short list of what most likely gets added:

- build-in color palettes + read palettes from files (so scene.json would only require the file path instead of 
copy-pasting all the colors)
- maybe depth of field/blur in some form
- materials/mediums: glass, matte, lambertian, fog/gas
    - maybe also simplistic emissive materials
- objects:
    - simple meshes for custom shapes. Also some kind of preview tool could be useful
    - maybe some new primitives
    - maybe new transforms like shear
- performance 
    - at least BHV when meshes get added
    - otherwise some smaller optimizations here and there
- scripting support via Lua language (complex scenes + animations)
- more unit tests + maybe a few integration tests

#### Priority
1. Lua scripting support
2. meshes + BHV
3. new materials/mediums
4. the rest