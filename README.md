# GOI Polar Controls Mod
This mod replaces the main control scheme of Getting Over It with an entirely new one.

Your mouse controls the rotation of the mouse and the extension of the hammer directly, via the x and y axes of your mouse respectively.\
This is in stark contrast to the vanilla control scheme of the game, where your mouse controls a cursor and the joints' motors are given input torques based on math to align with this cursor.

This control scheme is by comparison simpler, and aligns with the actual components of the game world.\
That does NOT mean that the game is made easier in any way, if you somehow stumbled into here searching for some cheat code to beat the game for your first time quickly, you are sorely mistaken.\
In summary: expect hell regardless of who you are.

## Installation and Usage
This mod is built in the BepInEx framework, that means you'll need to [install BepInEx](https://docs.bepinex.dev/articles/user_guide/installation/index.html).\
Once you've done that, navigate to the [releases](https://github.com/BarackOBusiness/GOI-Polar-Controls/releases) section of this repository and download the latest version of the mod.\
Place the download in the `BepInEx/plugins` folder inside of your game's root folder, and you're done with installation.

### Gameplay
As stated in the initial description, your mouse now directly controls the rotation of the hammer and the extension of the tip.\
Move your mouse left and right to rotate the hammer counterclockwise and clockwise respectively.\
Move your mouse up and down to push the hammer out and in respectively.

### Settings
This mod's default sensitivity is tuned to roughly a 1600 dpi mouse, as I considered that a conservative average of what people use.\
If your sensitivity doesn't feel right you are in luck as it is in fact configurable.\
To configure the sensitivity for both values, open the `BepInEx/config/goiplugins.ext.polarcoordinates.cfg` file in a text editor.\
Then modify the `Rotation Sensitivity` values and `Slider Sensitivity` values to your liking, restart the game after each change.

## Compilation
Make sure you have a dotnet sdk installed, version 7.x is verified to work here.\
Clone the repository, and create the directory `lib/net46` within the project root, copy GOIs Assembly-CSharp.dll and Rewired_Core.dll into it.\
Use `dotnet build` to compile the project, and the dll can then be found in the `bin/Debug/net46` folder
