-- Each move one of the following functions is called  (depending on the state of button):
function onNoButton()
    return goRight
end


function onButton()
    return jump
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
