**This document is about creating scripts to produce dynamic scenes**.

**You still need a static scene file for initial state. If you don't have one, see [Scene.md](Scene.md)**

**Scene and script file templates used for producing README image + gif can be 
in [scene-template](../scene-template/) directory**


## How to

- create a scene json file
- for objects and lights you want to manipulate: add `name` field with a good, easy to write identifier e.g. `"name": "sphere"`
- write a script and save it in Lua format `.lua`
- use `PixelRay -i <scene> -s <script> <frames> -g` to produce frames and combine them into a gif

## Lua API

`camera` is for camera access, you can manipulate
- position (`camera:Position`)
- look direction (`camera:LookAt`)
- upward axis (`camera:Up`)
- field of view (`camera:Fov`)
- aspect ratio (`camera:AspectRatio`)
    - no internal calculation are performed so this just a raw value

`scene` can be used to access the scene itself

- To access an object instance, use `GetObject(name)` and store reference to a variable:

    ```lua
    obj = scene:GetObject("torus")
    ```

    You should use a descriptive name, especially in complex scenes:

    ```lua
    torus = scene:GetObject("torus")
    ```

- Similarly, to access a light, use `GetLight(name)`:

    ```lua
    dir = scene:GetLight("directionalLight")
    ```

Currently only transforms can be manipulated.

### Transforms

- `ResetTransform()`  
    Resets object back to base transformation defined in json scene file

    - this is good for parametric equations: instead of using previous state, reset back to initial then let **t** 
    define new state
    - it also good to use this for stability purposes: long animations require stacking lots of transformations -> 
    lots of matrix multiplication -> possible numerical instability

    **Example:**
    ```lua
    obj:ResetTransform()
    ```

- `Translate(x, y, z)`  
    Move object to new position along vector (x, y, z) from current position

    - this doesn't move you TO position (x, y, z), but rather adds the vector so you move along it's direction and 
    magnitude (= summing two vectors)
    
    **Example:**
    ```lua
    obj:Translate(1, -3, 1)
    ```

    Here, if `obj` current position is (1, 2, 3), `obj:Translate(1, -3, 1)` moves it to (2, -1, 4)

- `Scale(x, y, z)`  
    Scale each object axis individually

    **Example:**
    ```lua
    obj:Scale(0.5, 1, 1)
    ```

    Scales object in x-direction to 0.5, others stay unchanged
    
- `ScaleUniform(t)`  
    Scale all axes equally

    **Example:**
    ```lua
    obj:Scale(0.5)
    ```

    This is same as using `obj:Scale(0.5, 0.5, 0.5)`

- `Rotate(x, y, z, w)`  
    Rotate object with respect to axis direction (x, y, z) using angle w (in degrees)

    **Example:**
    ```lua
    obj:Rotate(0, 1, 0, 90)
    ```

    This would rotate object around y-axis for 90 degrees

- `RotateMultiple(r1, r2, ...)`  
    Apply multiple rotation one after another
    - useful for complex rotations: instead of calculating final rotation, you can progress step by step
    - uses Lua tables {x1, y1, z1, w1} to separate rotations

    **Example:**
    ```lua
    obj:RotateMultiple({1, 0, 0, 90}, {0, 1, 0, 90}, {0, 0, 1, 90})
    ```
    
    This would first perform 90 degree x-direction rotation, then same for y-direction and finally for z-direction. 
    In this case, outcome is same as a single (0, 1, 0, 90) rotation.

### Lights

- `Color(r, g, b)`  
    Color of light source. Each colors is normalized to interval [0, 1], values leaving this boundary get clamped to
    0 or 1.

    **Example:**
    ```lua
    light:Color(1, 1, 1)
    ```

    translates to (255, 255, 255) or white color.

- `Intensity(value)`

    Light intensity, effectively controls the multiplier `Intensity / (0.01 + distance^2)`.

    **Example:**
    ```lua
    light:Intensity(2)
    ```

- `Direction(x, y, z)`
    Light direction, applies only to directional and spot light types.
    
    Both use opposite direction principle:
    - DirectionalLight: from ray hit point to source
    - SpotLight: from source to ray hit point

    **Example:**
    ```lua
    directional:Direction(1, 1, 1)
    ```
    Here you can think light from origin to (1, 1, 1). Thus global light ray arrives from (-1, -1, -1), or visually
    from upward right in a 45 degree angle but then angles another 45 degrees to positive z-direction.

    ```lua
    spot:Direction(1, 1, 1)
    ```
    For a spotlight this just simply points from source position to direction (1, 1, 1) as you'd expected

- `Position(x, y, z)`

    Light source position/origin if PointLight or SpotLight.

    **Example:**
    ```lua
    point:Direction(0, 0, 0)
    ```

- `LightRadius(radius)`

    Sampling radius for soft shadows. Works only for area lights e.g. PointLight and SpotLight.
    
    - soft shadows are very simple and there also quite rough-looking, especially in animations
    - use radius=0 for only hard shadows i.e. this light source will not contribute to soft shadows. 
    - sampling is actually not random, but rather uses fixed offsets. For this reason the shadow can look very blocky
    or spread out
    - very low values usually work even in animation. Even moderate radii will start to produce said side effects.

    **Example:**
    ```lua
    point:LightRadius(0.5)
    ```

- `ShadowBands(bands)`

    Soft shadow color quantization steps so an integer value. Works only with LightRadius > 0. 
    - values 0/1 mean no quantization, 
    then from 2 and higher will nudge shadow color to closest interval.
    - therefore higher values add smoothness, low values cause very clear shading bands

    **Example:**
    ```lua
    point:ShadowBands(4)
    ```

- `OuterAngle`

    SpotLight outer cone angle. Points outside this angle are entirely shadowed
    - uses degrees, NOT radians
    - uses full angles e.g. 10 degrees means that plane angle is 5 degrees to left and right.
    - values beyond 90 degree will probably look weird and extends behind the pointing direction

    **Example:**
    ```lua
    spot:OuterAngle(60)
    ```

- `InnerAngle`

    SpotLight inner cone angle. Points inside this angle are fully lighted. Anything between inner and outer angles
    will gradually become dimmer towards outer edge.
    - uses degrees
    - inner angle cannot exceed outer angle
    - also uses full angles

    ```lua
    spot:InnerAngle(45)
    ```

## Example

```lua
-- objects and lights
sphere = scene:GetObject("sphere")
torus = scene:GetObject("torus")
box = scene:GetObject("box")

pointLight = scene:GetLight("point")

-- camera controls
frames = 195 -- total frames required ()
rotationFrames = 120 -- 120 is good for smooth full camera rotation
unit = 360 / frames; -- split circle into frame units. Then unit*t matches always a full rotation

-- bouncing ball height controls
frequency = 1.0
amplitude = 0.2
decay = 0.3

function update(t)
    -- resets transform back to default in scene file. Useful for frame time dependent equations
    torus:ResetTransform()
    sphere:ResetTransform()
    box:ResetTransform()

    if t < 15 then
        -- keep camera at default position
    elseif t >= 15 and t < 30 then
        -- gradually move camera to look towards cone + zoom in a bit
        cosAngle = math.cos((2*math.pi/rotationFrames)*(t-15))
        sinAngle = math.sin((2*math.pi/rotationFrames)*(t-15))

        camera:LookAt(-sinAngle, 0, -cosAngle)
        camera:Position(-0.02*(t-15), 0, -0.025*(t-15))
    elseif t >= 30 and t < 45 then
        -- keep camera pointed at cone
    elseif t >= 45 and t < 60 then
        -- revert back to default position
        cosAngle = math.cos((2*math.pi/rotationFrames)*(59-t))
        sinAngle = math.sin((2*math.pi/rotationFrames)*(59-t))

        camera:LookAt(-sinAngle, 0, -cosAngle)
        camera:Position(-0.02*(59-t), 0, -0.025*(59-t))
    elseif t >= 60 and t < 75 then
        -- keep camera at default position
    else
        -- full xz-plane camera rotation around center
        cosAngle = math.cos((2*math.pi/rotationFrames)*(t-75))
        sinAngle = math.sin((2*math.pi/rotationFrames)*(t-75))
        center = {0, 0, -1}
        radius = 1.5
        x = radius*(sinAngle-center[1])
        z = radius*(cosAngle+center[3])

        camera:Position(x, 0, z)
    end

    -- box uniform scaling
    if t < 35 then
        -- reduce size
        scale = 1.0-t*0.02
        box:ScaleUniform(scale)
    elseif t >= 35 and t < 70 then
        -- then scale back to original
        scale = 1.0-(70-t)*0.02
        box:ScaleUniform(scale)
    end

    -- bouncing sphere
    frame = t / 60 -- 60 fps
    bounces = frame * frequency
    current_amplitude = amplitude * (decay ^ math.floor(bounces))
    h = math.abs(math.sin(bounces*math.pi)) * current_amplitude
    sphere:Translate(0, h, 0)

    -- torus y-axis rotates into opposite direction of camera
    torus:Rotate(0, 1, 0, -2*unit*t)

    -- pointlight pulsing with yellow color
    if t > 90 and t < 130 then
        -- gradually change intensity and color
        pointLight:Intensity(1.5+(t-90)*0.5)
        pointLight:Color(
            0.5 - (t-90)*0.0125,
            0.5 - (t-90)*0.0125,
            0.5 + (t-90)*0.0125
        )
    elseif t >= 130 and t <= 170 then
        -- then revert back
        pointLight:Intensity(1.5+(170-t)*0.5)
        pointLight:Color(
            0.5 - (170-t)*0.0125,
            0.5 - (170-t)*0.0125,
            0.5 + (170-t)*0.0125
        )
    end
end

```