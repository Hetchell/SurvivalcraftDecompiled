# SurvivalcraftDecompiled
Decompiled Version of SurvivalCraft in C#

SurvivalCraft(Version 2.2) is coded in C#. Credit for the source goes to Kaalus(Original game developer) and https://survivalcraft2mods.blogspot.com/2020/11/instalar-api-22-en-pc.html. 
Decompilation effort possible with DotPeek and DnSpy and programming headaches.
The game is still broken, but it is now able to be played to my usage. Useless code inside are for helping debugging. 

CURRENT BUGS/ISSUES:
1. <Fixed>
2. <Fixed>
3. <Fixed>
4. Terrain generation: 
        4a. <Fixed>
        4b. Ivy generate with offset to trees. 
        4c. Trees dissected by invisible chunks. 
        4d. Chunk generation broken if foreach loop used. 
5. "Play" button does not work as intended. Due to objectDisposedException. 
6. When no clip and explosive barrel are put down, the barrel becomes transparent even after no clip is turned off. You can not interact with it. Only until game level is exit can you reset barrel behaviour. 
--------------------------------------------------------------------------------------------------------------------------------------------------------------
TODO LIST: 
1. Remove the Chinese words and make language toggle. 
2. Break terrain generation. 
3. Implement teleportation(?). 
4. Implement command support(using external window?)
5. Make low level Akebi feature. 
6. Command blocks. 
