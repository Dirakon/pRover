-- Each move one of the following functions is called  (depending on the state of button):

state = 0
function onNoButton()
    if state == 0 then
        --go forward--
        if distanceRight == 0 then
            return jump
        else
            return goRight
        end
    elseif state == 1 then
        --go backwards--
        if distanceLeft == 0 then
            return jump
        else
            return goLeft
        end
    elseif state == 2 then
        --stay-
        return skip
    elseif state == 3 then
        --go forward--
        if distanceRight == 0 then
            return jump
        else
            return goRight
        end
    elseif state == 4 then
        --shoot--
        return shoot
    end

end

function onButton()
    state = state + 1
    if state > 4 then
        state = 0
    end
    return onNoButton()
end


-- You can use following returns:
skip = 0
goRight = 1
goLeft = 2
jump = 3
aimLeft = 4
aimRight = 5
aimUp = 6
shoot = 7
shootLeft = 8
shootRight = 9
shootUp = 10

-- You can also use following variables (they will dynamically change):
distanceDown = 0
distanceUp = 0
distanceRight = 0
distanceLeft = 0
