torus1 = scene:GetObject("torus1")

function update(t)
    torus1:ResetTransform() -- this resets transform back to default in scene file. Useful for parametrized equations

    frames = 195 -- total frames required ()
    rotationFrames = 120 -- 120 is good for smooth full camera rotation

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

    -- torus y-axis rotation into opposite direction
    unit = 360 / frames;
    torus1:Translate(0, -0.05, 0.75)
    torus1:Rotate(0, 1, 0, -2*unit*t)
    torus1:Translate(0, 0.05, -0.75)
end