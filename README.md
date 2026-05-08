![](docs/Images/Demo/default.png)

# A pixel-themed, cross-compatible ray tracer in C#

Some demo images, both old and new, can be found [here](docs/Images/Demo/)

## Table of contents

- [<u>How to use</u>](#how-to-use)
- [<u>Scene</u>](#scene)
    - [<u>Rendering</u>](#rendering)
    - [<u>Objects</u>](#objects)
    - [<u>Materials</u>](#materials)
    - [<u>Lights</u>](#lights)
- [<u>Building from source</u>](#building-from-source)


## How to use

Every command requires at least two things: scene file path + either

- output file path

    ```bash
    PixelRay -i <inputPath> -o <outputPath>
    ```

    or

- preview flag:

    ```bash
    PixelRay -i <inputPath> -p <previewDelay>
    ```

Output = store image in .png or .ppm format  
Preview = display image in a SILK.NET gui window, loading it gradually (higher values add delay, 0 = loads as quickly as 
possible) then flushing data after window is closed

Of course previewing can be done with outputs as well:

```bash
PixelRay -i scene.json -o output.png -p 0
```

#### All commands

- `-i <path>` scene file path. Scenes are stored in **json** files (see Scenes section below)
    - alternative: `--image <path>`
- `-o <path>`: output file path. Image format is either "png" or "ppm"
    - alternative: `--output <path>`
- `-p <delay>`: produce a preview image with SILK.NET library. Delay displays image gradually, loading pixels left to 
right, row by row. 0 = no delay.
    - alternative: `--preview <delay>`
- `--debug <mode>`: applies a debug mode to output/preview. Has following modes:
    - `normals` = color objects based on normal directions. Red = x, Green = y, Blue = z
    - `distance` = color objects based on how far they're from camera. Uses gray scale: closer objects are white/light 
    gray and distant objects become darker. Missed rays are red dots.
    - `id` = each object gets random color


## Scene

Scenes use **json** format.
Default scene file can be found [here](docs/scene.json).


### Rendering

#### camera
- `origin` origin/eye point
- `lookAt` camera direction
- `up` upward axis (useful for rotating)
- `fov` field of view

#### render

- `threading` for parallelized rendering, disabled by default
- `width` image width
- `height` image height
- `upScaledFactor` image upscaling factor
- `palette` color palette, default is [] which means no palette
- `lightingBands` lighting quantization levels
- `maxBounces` lighting ray bounces (if material if reflective)
- `dithering` enable orderer dithering
- `ditherLevels` dithering quantization levels
- `ditherDimension` Bayer matrix dimension: either 4 or 8


### Objects

#### primitives:
- `sphere` unit sphere
- `disc` unit disc pointing at (0, 1, 0)
- `plane` plane pointing at (0, 1, 0)
- `triangle` triangle defined by its three vertices
- `cylinder` bottom cap unit circle at (0, 0, 0), top cap unit circle (0, 1, 0)
- `cone` apex at (0, 0, 0), axis normal (0, 1, 0) which means base is unit circle with base (0, 1, 0)
- `torus` torus pointing at (0, 1, 0), requires minor and major radii
- `quadric` defined by quadric equation parameters
- `AAbox` axis-aligned box, cannot be rotated

Use transforms to change object geometry

#### transforms
- `position` translation/shifting
- `rotation` 4D vector [x, y, z, w] where (x,y,z) is the rotation axis and w the angle (degrees)
- `scaling` both uniform and non-uniform


### Materials
- `surface` general surface material with reflection and roughness controls
- `mirror`


### Lighting
- `ambient`
- `directional`
- `point` emits radially
- `spot` spotlight, outer/inner angle uses degrees

#### shadows
- hard shadows (lightRadius = 0)
- soft shadows (lightRadius > 0)
    - follows Monte Carlo integration logic, but instead of random samples uses fixed unit disc offsets that scale with radius
    - simple: requires tuning with radius+shadow bands, but shadows can still look quite rough. Some improvements could be done in the future


## Pixel-look

To enforce pixelated theme:

- low resolutions only, use nearest-neighbor upscaling for higher res
- lighting quantization
- hard shadows + simple soft shadow logic: radius and quantization
- keep reflection bounces low (0-3)  
    - can always use ambient/directional lights if scene doesn't allow proper bouncing e.g. lack of reflective surfaces or open scene where rays bounce to infinity
- ordered dithering
- custom color palettes. Works just fine without, but these can certainly change the aesthetics a lot
- no anti-aliasing/resampling

## Building from source

Code should be cross-platform compatible: both third-party packages
- SILK.NET (for quick image previewing)
- ImageSharp (for .png output format)

are also supported on Linux and macOs.

---

To build the executable, use

> dotnet publish -c Release -r \<os\> \<args\>

For example Windows 64-bit would be

`dotnet publish -c Release -r win-x64 <args>`

There are generally two ways to build into an executable:
1. Self-contained exe with glfw3.dll. This dll is SILK.NET native dependency and needs be included
    - large exe (~37 MB) but runs on its own, no .NET runtime required except for building
2. minimal exe + DLLs 
    - very small, but **requires .NET 10.0 Runtime** installation which can be downloaded 
    [here](https://dotnet.microsoft.com/en-us/download)

Instead of modifying *PixelRay.csproj* for each, both can be done by passing additional args.

- for args specification check 
[this](https://learn.microsoft.com/en-us/dotnet/core/deploying/single-file/overview?tabs=cli)
- examples below use Windows as runtime; for other identifiers, see 
[this](https://learn.microsoft.com/en-us/dotnet/core/rid-catalog#known-rids).

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

Same as Windows (here backward slashes used for compact presentation)

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
      --self-contained true \
      -p:PublishSingleFile=true \
      -p:EnableCompressionInSingleFile=true
    ```

#### Output

Release build can be found in "PixelRay/bin/Release/net10.0/win-x64/publish" or similar.


## Future additions

The goal of this project is not to become become a large, heavily optimized ray/path tracing tool. Yet it should still have good amount of customization options so here's a short list of what could be added:

- build-in color palettes + read palettes from files (so scene.json would only require the file path instead of copy-pasting all the colors)
- depth of field/blur in some form
- material/medium like glass, matte, lambertian, fog/gas
    - maybe eventually emissive materials
- objects:
    - shaders/meshes for custom shapes. Also some kind of preview tool could be useful
    - maybe some new primitives
    - maybe new transforms like shear
- performance (at least BHV, some minor optimizations here and there)
- scripting support via Lua language (complex scene creation + animation support)
- unit tests (better later than never: current ones are outdated)