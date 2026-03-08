## Pixel-graphic ray tracer

### Features

- primites:
    - Sphere
    - Disc
    - Plane
    - Triangle
    - Cylinder (custom height, render bottom and top discs)
    - Cone (custom height, renders base disc)
    - Torus
    - Quadric (general class)

- affine transforms via 4x4 matrices
    - rotation
    - transforms/shifting
    - scaling (both uniform and non-uniform)

- lighting
    - directional light

### TODO

- Maybe combine default HitObjects and Transformed ones. Just let user to modify public fields if they want to customize
them.
    - Pros: 
        - easy to use build-in transform features so often no matrix transforms required
        - always optional to use these features
    - Cons
        - default uses fixed position (origin), normal and parameter values; transformed object use custom positions,
      normals and parameters which makes hit calculations more general and thus heavier even when using same default
      parameters
        - building new object to support pre-transforms is difficult/time consuming (of course could always just skip
        this separation for new objects)

- light: light source type, ambient lighting, reflections, refractions, global illumination
- camera: FOV setting
- materials
- clean up code: optimizing performance and code in general, updating comments (= remove excess ones)
    - Bounding Volume Hierarchy (BHV) support (torus uses a lazy bounding sphere logic already)
    - AABB (axis-aligned bounding boxes)
    - clean up code: do the quadratic check require so much logic to handle both solutions. After all it's the left root
    that always connects first
- superquadrics class
- creating scenes from JSON files. Load camera/rendered setting too?
- shaders and triangle meshes
- turning camera -> moving camera?
- ray marching
- unit tests
