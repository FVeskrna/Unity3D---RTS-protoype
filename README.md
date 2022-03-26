# Unity3D-RTS protoype
Simple grid map generator expanded with features commonly found in RTS games.

This project started as a simple  grid map generator featuring grasslands, lakes, beaches, mountains, forests and other vegetation. As time went by I have started expanding this project with vision of some RTS type of game. This game is highly in progress as it serves mainly as learning tool for me, to learn new things in RTS style games. In this project, I'm trying to use as little game assets as possible and trying to figure my own ways to implement different kinds of functionilities.


# Map Generation
![image](https://user-images.githubusercontent.com/99544664/160245107-a539dead-e404-4abb-983e-1011213d2d79.png)

First thing user is greeted with is map generation setting. On the left side there are some presets that user can use. User can also fully set all the values of map generation to create all kinds of creations. Value range of these settings are very little restricted to allow a lot of variety in generation.
Lakes, mountains and forests are all genereted by the same algorythm.
At first "seed" of the structure is randomly placed on the grid. This step is tried several times before the algorythm gives up as it didn't find any free space. After that, the seed expands on all four directions with value indicating the the chance of actually spreding. Each time, this expandation happens, the chance is lowered. This results in randomly shaped structures.
After mountains are spread with algorythm described above, they raise into height. Algorythm goes through every mountain "block" and checks, if it has at least three neighbors. This step is reaped several times, until height limit set in inital settings is reached. To optimaze the game, similiar algorythm checks every mountain block to check if it is visible froum outside, if not, it simply deletes it as it is uselles to keep. This makes the mountains hollow.

![image](https://user-images.githubusercontent.com/99544664/160245738-66abc042-2e90-4267-b5df-12de2c68fa9d.png)

# Placing buildings
![image](https://user-images.githubusercontent.com/99544664/160245761-c0f9dcc8-af7f-4d88-871a-41daef7e8d77.png)

Every cell in grid map has a boolen value to show if it's already occupied. For example, cell with tree on top is already marked as occupied. With this paremeter, we can simply check, if a place on which we are trying to place a building on is occupied or not. User can also rotated placeable object with designited key.
This allow to make certein conditions, which needs to be met to place a building. For example, to place a fishing hut, 2 of 4 cells need to be in water, otherwise, user is infomed by text that he can't place this building there.

