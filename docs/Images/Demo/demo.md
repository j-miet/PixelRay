All images use same scene:
- only rendering and camera settings change
- only exception is ambient lighting which is set to 0. In demo12 some ambient lighting + stronger directional light is applied to compensate for removal of area light sources

#### default (README image)

```json
"camera": {
    "position": [0, 0, 0],
    "lookAt": [0, 0, -1],
    "upDirection": [0, 1, 0],
    "fov": 90
},
"render": {
    "threading": true,
    "width": 250,
    "height": 140,
    "upScaleFactor": 3,
    "palette": [],
    "lightingBands": 4,
    "maxBounces": 1,
    "dithering": true,
    "ditherLevels": 12,
    "ditherDimension": 4
}
```

#### demo1 (distant camera)

```json
"camera": {
    "position": [0, 0, 2],
    "lookAt": [0, 0, -1],
    "upDirection": [0, 1, 0],
    "fov": 90
},
"render": {
    "threading": true,
    "width": 250,
    "height": 140,
    "upScaleFactor": 3,
    "palette": [],
    "lightingBands": 4,
    "maxBounces": 1,
    "dithering": true,
    "ditherLevels": 12,
    "ditherDimension": 4
}
```

#### demo2 (view from above)

```json
"camera": {
    "position": [0, 1, 0],
    "lookAt": [0, -1, 0],
    "upDirection": [0, 0, -1],
    "fov": 90
},
"render": {
    "threading": true,
    "width": 250,
    "height": 140,
    "upScaleFactor": 3,
    "palette": [],
    "lightingBands": 4,
    "maxBounces": 1,
    "dithering": true,
    "ditherLevels": 12,
    "ditherDimension": 4
}
```

#### demo3 (downward 45 degree angle)

```json
"camera": {
    "position": [0, 1, -1],
    "lookAt": [0, 0, -1],
    "upDirection": [0, 1, 0],
    "fov": 90
},
"render": {
    "threading": true,
    "width": 250,
    "height": 140,
    "upScaleFactor": 3,
    "palette": [],
    "lightingBands": 4,
    "maxBounces": 1,
    "dithering": true,
    "ditherLevels": 12,
    "ditherDimension": 4
}
```

#### demo4 (behind the scene)

```json
"camera": {
    "position": [0, 0, -4],
    "lookAt": [0, 0, -1],
    "upDirection": [0, 1, 0],
    "fov": 90
},
"render": {
    "threading": true,
    "width": 250,
    "height": 140,
    "upScaleFactor": 3,
    "palette": [],
    "lightingBands": 4,
    "maxBounces": 1,
    "dithering": true,
    "ditherLevels": 12,
    "ditherDimension": 4
}
```

#### demo5 (grayscale)

```json
"camera": {
    "position": [0, 0, 0],
    "lookAt": [0, 0, -1],
    "upDirection": [0, 1, 0],
    "fov": 90
},
"render": {
    "threading": true,
    "width": 250,
    "height": 140,
    "upScaleFactor": 3,
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
    "lightingBands": 4,
    "maxBounces": 1,
    "dithering": true,
    "ditherLevels": 12,
    "ditherDimension": 4
}
```

#### demo6 (no dithering)

```json
"camera": {
    "position": [0, 0, 0],
    "lookAt": [0, 0, -1],
    "upDirection": [0, 1, 0],
    "fov": 90
},
"render": {
    "threading": true,
    "width": 250,
    "height": 140,
    "upScaleFactor": 3,
    "palette": [],
    "lightingBands": 4,
    "maxBounces": 1,
    "dithering": false,
    "ditherLevels": 12,
    "ditherDimension": 4
}
```

#### demo7 (lightingbands=2, dithering)

```json
"camera": {
    "position": [0, 0, 0],
    "lookAt": [0, 0, -1],
    "upDirection": [0, 1, 0],
    "fov": 90
},
"render": {
    "threading": true,
    "width": 250,
    "height": 140,
    "upScaleFactor": 3,
    "palette": [],
    "lightingBands": 2,
    "maxBounces": 1,
    "dithering": true,
    "ditherLevels": 12,
    "ditherDimension": 4
}
```

#### demo8 (lightingbands=2, no dithering)

```json
"camera": {
    "position": [0, 0, 0],
    "lookAt": [0, 0, -1],
    "upDirection": [0, 1, 0],
    "fov": 90
},
"render": {
    "threading": true,
    "width": 250,
    "height": 140,
    "upScaleFactor": 3,
    "palette": [],
    "lightingBands": 2,
    "maxBounces": 1,
    "dithering": false,
    "ditherLevels": 12,
    "ditherDimension": 4
}
```

#### demo9 (lightingbands=4, dithering, ditherlevels=4)

```json
"camera": {
    "position": [0, 0, 0],
    "lookAt": [0, 0, -1],
    "upDirection": [0, 1, 0],
    "fov": 90
},
"render": {
    "threading": true,
    "width": 250,
    "height": 140,
    "upScaleFactor": 3,
    "palette": [],
    "lightingBands": 4,
    "maxBounces": 1,
    "dithering": true,
    "ditherLevels": 4,
    "ditherDimension": 4
}
```

#### demo10 (lightingbands=4, dithering, ditherlevels=2)

```json
"camera": {
    "position": [0, 0, 0],
    "lookAt": [0, 0, -1],
    "upDirection": [0, 1, 0],
    "fov": 90
},
"render": {
    "threading": true,
    "width": 250,
    "height": 140,
    "upScaleFactor": 3,
    "palette": [],
    "lightingBands": 4,
    "maxBounces": 1,
    "dithering": true,
    "ditherLevels": 2,
    "ditherDimension": 4
}
```

#### demo11 (lightingbands=32, no dithering)

```json
"camera": {
    "position": [0, 0, 0],
    "lookAt": [0, 0, -1],
    "upDirection": [0, 1, 0],
    "fov": 90
},
"render": {
    "threading": true,
    "width": 250,
    "height": 140,
    "upScaleFactor": 3,
    "palette": [],
    "lightingBands": 32,
    "maxBounces": 1,
    "dithering": false,
    "ditherLevels": 12,
    "ditherDimension": 4
}
```

#### demo12 (lightingbands=32, maxBounces=0, no dithering, remove area lights)

```json
"camera": {
    "position": [0, 0, 0],
    "lookAt": [0, 0, -1],
    "upDirection": [0, 1, 0],
    "fov": 90
},
"render": {
    "threading": true,
    "width": 250,
    "height": 140,
    "upScaleFactor": 3,
    "palette": [],
    "lightingBands": 32,
    "maxBounces": 0,
    "dithering": false,
    "ditherLevels": 12,
    "ditherDimension": 4
}
.
.
.
"lights": [
    {
        "type": "ambient",
        "color": [1, 1, 1],
        "intensity": 0.05
    },
    {
        "type": "directional",
        "direction": [-0.2, 0.5, 1],
        "color": [0.5, 0.5, 0.5]
    }
]
```