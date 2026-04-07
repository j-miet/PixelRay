## Pixel-graphic ray tracer

### Features

- primitives:
    - Sphere
    - Disc
    - Plane
    - Triangle
    - Cylinder
    - Cone
    - Torus
    - Quadric (general class)

- affine transforms via 4x4 matrices
    - rotation
    - transforms/shifting
    - scaling (both uniform and non-uniform)

- lighting
    - directional light

- camera
    - camera origin/eye point
    - width
    - height
    - viewport height
    - focal length (= distance to viewport)

- rendering
    - width
    - height
    - color palette
    - lighting bands
    - ambient color factor
    - rendering config
        - image upscale factor
        - debug mode

### TODO (including random stuff that might or might now get implemented)

- light: light source type, ambient lighting, reflections, refractions, global illumination
- soft shadows (umbra, penumbra, antumbra)
- camera: FOV setting
- materials
- clean up code: 
    - optimize performance
        - Bounding Volume Hierarchy (BHV) support (torus uses a lazy bounding sphere logic already)
        - AABB (axis-aligned bounding boxes)
    - clean up codebase
        - can split long code into subroutines
        - clean up object implementations and ask if all the logic is required (e.g. quadratic checks when left root is the first to hit)
        - naming conventions
    - updating comments (= remove excess ones)
    that always connects first
- new primitives: 
    - superquadrics class
    - mobius strip
    - quadrilaterals class
    - paths/curves
- more matrix transforms e.g. shear and maybe an example of nonlinear transform
- creating scenes from JSON files. Add camera/renderer settings, too.
- shaders and triangle meshes
- turning camera -> moving camera?
    - also before moving camera, dynamic re-rendering e.g. modify scene, camera, renderer parameters -> retrace ->
    render scene again. 
    - Implementation: 
        - run CLI command, read changes from a file
        - modify tracer class attributes
        - trace + render
        - display new image
- unit tests
