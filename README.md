# AccessBattle

This is a free non-commercial implementation of the game 
"Rai-Net Access Battlers" which was originally released 
by the Japanese company '5pb.'. 
AccessBattle is a private fan project and not affiliated with
'5pb.' in any way.

It is still in development. Right now you can play against
a very stupid AI. The final game will be playable over network.

- Estimated completion: End of August 2017
  - UPDATE: Due to some private problems, the release might be postponed by one month
- Current Status and next steps:
  - [x] Create and join games over server (only library functions)
  - [ ] UI rewrite (WIP)
    - [x] Display Board and cards
	- [ ] Menus (WIP)
	- [ ] Player interaction	
  - [ ] Synchronize game data between clients (WIP)
  - [ ] Game logic rewrite

The main game engine is inside a separate DLL so that people
can program their own user interface for it. The DLL should
be compatible with Mono and also run on Linux.

The user interface is implemented with WPF using .NET 4.5.
You need at least Windows Vista SP2, Windows 7 SP1 or newer 
to use it. I tested it on Windows 7.

The code is currently a complete mess. I am focusing on
making the game run. When it does I will clean it up
and add some documentation.

Third party notices:
This program uses the following external sources.
- Silkscreen font by Jason Kottke
- Newtonsoft.Json by James Newton-King

Special thanks:
5pb. and Nitroplus for creating Steins;Gate