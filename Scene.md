**This document is about static scenes creation.**

**If you'd like to add scripting to your scene, see [Lua.md](Lua.md)**.

## Right-hand coordinates:

**X increases to right, Y increases upwards, camera points to negative Z**

So when you setup camera:

- standard: 
    - `lookAt = (0, 0, -1)`, `up = (0, 1, 0)` matches to this base system

- custom:

    `lookAt` and `up` directions define an orthonormal basis where lookAt is the forward axis, up the upward axis. 
    These produce the right axis. Finally right and forward produce orthogonal upward axis if `up` isn't a valid one 
    for this new basis already

    => `lookAt` is the forward direction, `up` can be used for rotating output image

## Json file

Here are all the scene entities and their parameters

- 3D points/vectors are written as `[x, y, z]` 
- objects, materials and lights use type idenfication via `type` parameter 

### Rendering

#### camera
- `origin`: `[x, y, z]`
    - origin/eye point/center of the camera
- `lookAt`: `[x, y, z]`
    - camera direction
- `up`: `[x, y, z]`
    - upward axis (useful for rotating)
- `fov`: `decimal >= 0`
    - vertical field of view

#### render

- `threading`: `boolean, default=true` 
    - for parallelized rendering
- `width`: `integer >= 0` 
    - image width
- `height`: `integer >= 0` 
    - image height
- `upScaledFactor`: `integer >= 0, default=1` 
    - image upscaling factor. Upscaling uses nearest neighbor copy: if factor is N, each pixel becomes a NxN block
- `palette`: `list of [x, y, z], default=[] ` 
    - color palette, default is [] which means no palette. You can also just leave this field out for default value.
    - otherwise colors have to be applied as a list of colors, even if just single one e.g. `[[0.5, 0.5, 0.5]]`
    - example of a simple grayscale palette:

        ```json
        "palette": [
            [0, 0, 0],
            [0.1, 0.1, 0.1], 
            [0.2, 0.2, 0.2],
            [0.3, 0.3, 0.3],
            [0.4, 0.4, 0.4],
            [0.5, 0.5, 0.5],
            [0.6, 0.6, 0.6],
            [0.7, 0.7, 0.7],
            [0.8, 0.8, 0.8],
            [0.9, 0.9, 0.9],
            [1, 1, 1]
        ],
        ```
- `lightingBands`: `integer >= 0, default=4` 
    - lighting quantization levels
- `maxBounces`: `integer >= 0, default=1` 
    - lighting ray bounces (if material is reflective)
- `dithering`: `boolean, default=false`
    - enable orderer dithering
- `ditherLevels`: `integer >= 0, default=16` 
    - dithering quantization levels
- `ditherDimension`: `integer, default=4` 
    - Bayer matrix dimension: either 4 or 8


### Objects

Each object has optional `name` field. This can be used for accessing specific object in Lua scripts.

- `sphere` unit sphere
- `disc` unit disc pointing at (0, 1, 0)
- `plane` plane pointing at (0, 1, 0)
- `triangle` triangle defined by its three vertices
    - vertices are given as `v1`, `v2` and `v3`, each of type `[x, y, z]`
- `cylinder` bottom cap unit circle at (0, 0, 0), top cap unit circle (0, 1, 0)
- `cone` apex at (0, 0, 0), axis normal (0, 1, 0) which means base is unit circle with base (0, 1, 0)
- `torus` torus pointing at (0, 1, 0)
    - requires minor and major radii. Default values are `0.2` and `0.1` respectively.
- `quadric` defined by quadric equation parameters and bounds
    - factors: `a`, `b`, `c`, `d`, `e`, `f`, `g`, `h`, `i`, `j`
    - `minBounds` and `maxBounds` define x,y,z axis bounds so objects don't become infinite. Both use `[x, y, z]` 
    format.
- `AAbox` axis-aligned box, **cannot be rotated**
    - `minBounds` and `maxBounds` are used for limiting box boundaries. These use `[x, y, z]` format.

Use transforms to change object geometry, and materials to change color & how light is reflected

#### transforms
- `translate`: `[x, y, z]` 
    - translation/shifting/moving object position alongside vector (x, y, z)
    - it doesn't move object TO (x, y, z), instead adds this vector to your current position
- `rotation`: `list of [x, y, z, w]` 
    - 4D vector [x, y, z, w] where (x,y,z) is the rotation axis and w the angle in degrees (not radians!)
    - rotations are always treated as a list of rotations so even for a single rotation, use `[[1, 0, 0, 90]]`. If no 
    rotations are needed, simply don't include "rotation" field in your transform

    And multiple rotations would be applied in order like this
    ```json
    "rotation": [
        [1, 0, 0, 90],
        [0, 1, 0, 5.625]
    ]
    ```
    => 90 degree x-axis rotation followed by 5.625 y-axis rotation
- `scale`: `[x, y, z]` 
    
    - non-uniform scaling also supported (= all axes don't need to have same value)


### Materials

Each material has these parameters:
- `color`: `[x, y, z]` 
    - RGB color where each of x/y/z uses values from closed interval [0, 1]. These are then internally converted to 
    [0, 255] range.
    - therefore [0, 0, 0] is black, [1, 1, 1] is white and so on
- `bounce`: `decimal [0-1], default=0`
    - amount of light that gets reflected, lower value means faint/no reflection, higher values most/all of indirect 
    lighting color gets applied
    - requires reflective surface, otherwise this value means nothing. 
- `linearBounce`: `boolean, default=false`
    - whether linear regression is applied for reflected lighting
    - normal formula is just `direct + indirect * bounce`
    - if linearBounce=true, this becomes `direct * (1 - mat.Bounce) + indirect * mat.Bounce`. This can be used for 
    smoother/cleaner reflections if material's darker direct light color has less impact.

Material types:
- `surface`
    - general surface material with reflection and roughness controls
    - `reflection`: `decimal [0-1], default=0` controls distribution of projections to diffusions. 0 = all reflections 
    are randomly sampled (full diffusion), 1 = everything is projected (surface becomes a mirror). Everything between 
    these puts more emphasis on another and 0.5 is the 50/50 point: either random or sampled
    - `roughness`: `decimal [0, 1], default=0` applied linear regression to projections. 0 = projections stay as they 
    are, 0 > projections will tilt to random directions, 1 = projections become diffused. If you already use 
    reflection=0, roughness has no impact (as directions are already fully randomized)
- `mirror`
    - every light ray will be projected to direction defined by standard vector projection formula
    - also uses `bounce=1` by default for clear images. However `linearBounce` is not enabled by default.


### Lighting

Each light source has these parameters:
- `color`: `[x, y, z]` RGB color, each x/y/z in on [0, 1] interval
- `intensity`: `decimal >= 0, default=1` 
    - A multiplier applied to final light color
    - light sources should use 1 as default e.g. apply the light's own shading color without weakening/amplifying it

Light types:
- `ambient` Global color value which gets added to each object on scene
- `directional` 
    - global directional light source, all rays come from same direction to scene
    - direction is TO source of light ray. 
        - for example if standard right-hand coordinate system is used: you want light to arrive on scene from the sky 
        at a 45 degree downward y-angle. Then you would point from (0, 0, 0) TO (0, 1, 1) whereas light arrives to 
        camera FROM direction (0, -1, -1). 
        
        => so (0, 1, 1) is the direction you'd input
- `point` emits light radially and can produce soft shadows
    - `lightRadius`: `decimal >= 0, default=0` controls radius of light to produce soft shadow. If 0, only hard shadow 
    are produced.
    - `shadowBands`: `integer >= 0, default=0` controls quantization of shadow contrast. 
        - 0/1 (single band is same as 0) = "smooth" shadow colors
        - values > 0 limits color range
    - uses attenuation `1 / (0.01 + distance^2)`
- `spot` 
    - spotlight with outer/inner angle
    - angles are in degrees and use **full angles**. Values >= 90 should be avoided, otherwise light will clip behind 
    the light and produce odd visuals
    - direction is FROM spotlight center i.e. where you want the spotlight to point to from center point
    - `position`: `[x, y, z]` is the light position
    - `direction`: `[x, y, z]` is the spotlight cone direction
    - `outerAngle`: `decimal >= 0` is the outer cone angle. Objects outside are fully shadowed
    - `innerAngle`: `decimal >= 0` is the inner cone angle. Objects inside this are fully lighted. Objects between 
    inner and outer angle get dimmer towards outer edge.
    - has `lightRadius` and `shadowBands` values similarly to `point` light type. They also use same default values (0).

<u>Issue with plane normals</u>  
As planes use fixed normals, they can cause weird interactions with radius-based lights. For example a plane very very far away can still get faintly colored when everything around it should be black. Also: if two planes intersect, their intersection point normal will naturally jump a bit in that when moving between planes. If light is pointed here, it might light the first plane well but entirely shadow the second one, making intersection lighting quite bad.
- I guess fixing this would require more aggressive attenuation specifically for planes + use some form of smoothing near intersection edges.
-  as a fix you could instead use a very distant and large-radius sphere as a background/floor. Sphere uses smooth normal curvature and overcomes this issue

#### shadows

Lights without radius control (`ambient`, `directional`) cannot apply soft shadows


For radius types:

- hard shadows (lightRadius = 0)
- soft shadows (lightRadius > 0)
    - follows basic Monte Carlo integration logic, but instead of random samples uses fixed unit disc offsets that scale 
    with radius
    - very simple, but can also hard to control: requires tuning with  + shadow bands, but shadows can still look 
    quite rough. 
    - some improvements could be done in the future, but these will do for now. Hard shadow can always be used, they 
    probably look better for pixelated style anyway.


## Example

> Comments are **not supported** in actual json format and would have to be removed before use

```json
{
    "camera": { // camera controls
        "position": [0, 0, 0],
        "lookAt": [0, 0, -1],
        "upDirection": [0, 1, 0],
        "fov": 90
    },
    "render": { // renderer configurations
        "threading": true, // default is 'true' so this line is unnecessary. Keep enabled for major rendering speed boost
        "width": 250,
        "height": 140,
        "upScaleFactor": 3,
		"palette": [], // can also just drop this line if no palette is applied
		"lightingBands": 4,
		"maxBounces": 1,
		"dithering": true, // ordered dithering, levels/dim do nothing when this is disabled
		"ditherLevels": 12,
		"ditherDimension": 4
    },
    "objects": [ // all scene objects go under this
        {
            "type": "sphere", // use type to define objects/materials/lights
            "material": { // material defines visuals (color & lighting+shadows)
                "type": "surface",
                "color": [0.7, 0.2, 0.2]
            },
			"transform": { // transforms are used for changing object's default geometry (position, size/shape, rotation)
                "scale": [0.05, 0.05, 0.05],
                "position": [-0.3, -0.2, -0.28]
            }
        },
        {
            "type": "triangle",
            "v1": [0.2, -0.25, -0.3], // some objects don't have default geometry values so 'type' alone is not enough
            "v2": [1.1, -0.25, -0.8],
            "v3": [0.9, 0.25, -0.65],
            "material": {
                "type": "surface",
                "color": [0.8, 0.9, 0.1]
            }
        },
        {
            "type": "cylinder",
            "material": {
                "type": "mirror",
                "color": [0.5, 0.5, 0.9],
                "linearBounce": true
            },
            "transform": {
                "position": [-0.6, -0.25, -0.75],
                "scale": [0.25, 0.35, 0.25]
            }
        },
        {
            "type": "cone",
            "material": {
                "type": "surface",
                "color": [0.8, 0.1, 0.1]
            },
            "transform": {
                "position": [-0.6, 0.45, -0.75],
                "scale": [0.25, 0.35, 0.25],
                "rotation": [
                    [1, 0, 0, 180]
                ]
            }
        },
        {
            "type": "disc",
            "material": {
                "type": "surface",
                "color": [0.9, 0.7, 0.3],
                "reflectivity": 0,
                "bounce": 1
            },
            "transform": {
                "position": [-1.55, 0.7, -1.2],
                "scale": [0.25, 1, 0.25],
                "rotation": [ // multiple rotations can be applied by listing them one after another. Angles use degrees (not radians)
                    [1, 0, 0, 90],
                    [0, 1, 0, 5.625]
                ]
            }
        },
        {
            "name": "torus1" // optional name, useful for referring to specific objects when scripting
            "type": "torus",
            "minorRadius": 0.2,
            "majorRadius": 0.1,
            "material": {
                "type": "surface",
                "color": [0.5, 1, 0.5]
            },
            "transform": {
                "position": [0, 0.05, -0.75],
                "rotation": [
                    [1, 0, 0, 90]
                ]
            }
        },
        {
            "type": "quadric",
            "coefficients": [ // general quadric equation
                1, -1, 1,
                0, 0, 0,
                0, 0, 0,
                -0.2
            ],
            "minBounds": [-3, -3, -3], // lower x/y/z bounds
            "maxBounds": [3, 3, 3], // upper x/y/z bounds
            "material": {
                "type": "surface",
                "color": [0.6, 0.6, 0.6]
            },
            "transform": {
                "position": [2, 0.5, -3]
            }
        },
        {
            "type": "aabox", // axis-aligned box
            "minBounds": [-0.05, -0.05, -0.05],
            "maxBounds": [0.05, 0.05, 0.05],
            "material": {
                "type": "surface",
                "color": [1, 0.65, 0]              
            },
            "transform": { // AABox doesn't support rotations (you can try, but they become deformed)
                "position": [0.22, -0.2, -0.45]
            }
        },
        {
			"type": "plane",
			"material": {
				"type": "surface",
				"color": [0.1, 0.8, 0.8]
			},
			"transform": {
				"position": [0, -0.25, 0]
			}
		},
		{
			"type": "plane",
			"material": {
				"type": "surface",
				"color": [1, 1, 1]
			},
			"transform": {
				"position": [0, 0, -20],
				"rotation": [
					[1, 0, 0, 90]
				]
			}
		}
    ],
    "lights": [
		{
			"type": "ambient",
			"color": [1, 1, 1],
			"intensity": 0.0
		},
        {
            "type": "directional",
            "direction": [-0.2, 0.5, 1], // direction is TO source
            "color": [0.1, 0.1, 0.1]
        },
		{
			"type": "point",
			"position": [0, 0.5, -0.05],
			"color": [0.5, 0.5, 0.5],
			"intensity": 1.5,
            "lightRadius": 0.1,
            "shadowBands": 4
		},
        {
            "type": "spot",
            "position": [1.5, -0.15, 0.3],
            "direction": [-1, 0, -1], // direction is FROM 'position' as expected
            "outerAngle": 10,
            "innerAngle": 8,
            "color": [0.9, 0.6, 0.3],
            "intensity": 3,
            "lightRadius": 0,
            "shadowBands": 0
        },
        {
            "type": "spot",
            "position": [-0.6, 0.45, -0.74],
            "direction": [-1, 0.3, -1],
            "outerAngle": 25,
            "innerAngle": 15,
            "color": [1, 1, 1],
            "intensity": 1.5,
            "lightRadius": 0,
            "shadowBands": 0
        }
    ]
}

```