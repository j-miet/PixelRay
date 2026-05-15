**This document is about creating scripts to produce dynamic scenes**.

**You still need a static scene file for initial state. If you don't have one, see [Scene.md](Scene.md)**

**Scene and script file templates used for producing README image + gif can be in [scene-template](../scene-template/) directory**


## How to

- create a scene json file
- for objects you want to manipulate: add `name` field and add identifier e.g. `"name": "sphere1"`
- write a script in Lua format
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
    obj = scene:GetObject("torus1")
    ```

    You should use a descriptive name, especially in complex scenes:

    ```lua
    torus1 = scene:GetObject("torus1")
    ```

Currently only transforms can be manipulated.

### Transforms

#### Order of operations (IMPORTANT)

For static scenes, PixelRay will always perform translation in this order:  
    
    scale -> rotate -> translate

Reason is that all geometry objects primitives are origin-centered so matrix transforms behave as expected.

**What this means for animations:**

- translations work always
- scaling and rotating: 
    - requires you to translate object into origin (0, 0, 0)
    - then perform scaling and/or rotating
    - then translate back to actual position

    For example: object at (1, 0.5, -2), you want to scale it. You then first move to (0, 0, 0) by applying inverse translation (-1, -0.5, 2) then scale, then apply translation (1, 0.5, -2) again.

**It's important to follow this principle, otherwise your animations will not produce correct frames.**

---

#### Transform commands

- `ResetTransform()`  
    Resets object back to base transformation defined in json scene file

    - this is good for parametric equations: instead of using previous state, reset back to initial then let **t** define new state
    - it also good to use this for stability purposes: long animations require stacking lots of transformations -> lots of matrix multiplication -> possible numerical instability

    **Example:**
    ```lua
    obj:ResetTransform()
    ```

- `Translate(x, y, z)`  
    Move object to new position along vector (x, y, z) from current position

    - this doesn't move you TO position (x, y, z), but rather adds the vector so you move along it's direction and magnitude (= summing two vectors)
    
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
    
    This would first perform 90 degree x-direction rotation, then same for y-direction and finally for z-direction. In this case, outcome is same as a single (0, 1, 0, 90) rotation.




