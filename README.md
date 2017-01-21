# SpaceWarrior
Game implemented with WPF's Canvas Control.

# How to play/build
1. Run build.cmd
2. navigate to /build
3. have fun

# Control the Player
* move up:    W
* move left:  A
* move down:  S
* move right: D
* shoot:		Space
* menu:		ESC

# Introducing timeSinceLastFrame
The game loop is aware of the time the client needs to calculate and draw
the objects (Player, Enemies, Bullets, ...). This is needed for hardware independent
speed of the game objects. (See ViewModels.MainViewModel.RunWorker())

# Implementation of the enemies movement
The enemies movement is implemented through vector calculations.
The goal is that the enemies moves to our player with a given speed.

## Example:

### Given:
* Players positon (vP) = (x=1, y=2)
* Enemy positon (vE)   = (x=5, y=3)
* speedX, speedY       = 100 // Enemy speed x and y
* timeSinceLastFrame   = 0.5

### Caltulating the vector pointed from the enemy to the player
* vR = vP - vE
* vR = (1,2) - (5,3)
* vR = (-4,-1)

vR is the vector wich points from the enemy to the player.
Because vectors not just gives us the direction but also the speed by its lenght
we have to normalize this vector to give it a length of 1. Otherwise the opponent would move slower,
the closer he comes to the player. For this we need to calculate the current lenght with the theorem of Pythagoras.

### Calculating the length of vR
sqrt(-4² + -1²) = srq(17) = ~4.12

### Normalize vR by devide it by its length
(-4,-1) : 4.12 = ~(0.97,-0.24)

### Calculating the speedX and speedY
Now we are able to define our own speed.

* speedX = 0.97 * (speedX * timeSinceLastFrame)
* speedY = -0.24 * (speedY * timeSinceLastFrame)

* EnemyPosX += speedX
* EnemyPosY += speedY

(see Entities.Enemy.Update())

# TODO's and 'nice to have'
+ Add collision detection (bullets, enemies, etc.)
+ Add animations for collisions
+ Random spawning of enemies
+ Stop game loop when the user is in the menu
+ Add sounds
+ Add highscore
+ Add dirrefent levels
+ Add different levels of difficulty
+ Add power-up's
+ Add different weapons
+ CI (AppVeyor)
