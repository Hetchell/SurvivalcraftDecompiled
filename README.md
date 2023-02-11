# SurvivalcraftDecompiled
Decompiled Version of SurvivalCraft in C#

SurvivalCraft(Version 2.2) is coded in C#. Credit for the source goes to Kaalus(Original game developer) and https://survivalcraft2mods.blogspot.com/2020/11/instalar-api-22-en-pc.html. 
Decompilation effort possible with DotPeek and DnSpy and programming headaches.
The game is still broken, but it is now able to be played to my usage. Useless code inside are for helping debugging. 

CURRENT BUGS/ISSUES:
1. <Fixed>
2. Explosive barrels unable to light fuse. 
3. Magma does not behave properly => will eat sand up? 
4. Terrain generation: 
        4a. Traps generate wrongly. 
        4b. Ivy generate with offset to trees. 
        4c. Trees dissected by invisible chunks. 
        4d. Chunk generation broken if foreach loop used. 
5. "Play" button does not work as intended. 
--------------------------------------------------------------------------------------------------------------------------------------------------------------
TODO LIST: 
1. Make creative fly speed variable by up/down button. Now only set to two speeds. => Find out why speed is limited.
2. Break terrain generation. 
3. Implement teleportation(?). 
