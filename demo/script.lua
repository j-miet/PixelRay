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