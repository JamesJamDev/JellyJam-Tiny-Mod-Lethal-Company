# CHANGELOG
Version 1.0.3
- Fixed non-host players not being able to change size
- /Stuck was added
    - Only works in the ship
    - Is used for when you spawn in the floor and cannot move
- Reverted player being bigger when first spawning in to prevent being stuck (above change replaces it) as it was causing issues

Version 1.0.2
- Added a command "/speed" that allows you to use the normal player values like speed, jumpheight and reach distance while tiny
- Optimized a few parts of the code

Version 1.0:
- Visor will no longer show and block most the screen
- Player jumps slightly higher to make the map more accessible
- Player turns to normal size while interacting with the Terminal so you don't have to blindly type
- Hoarding Bugs can pick up the player for a tiny bit when colliding (Warning: they do NOT like it when they hold you)
- /Size can be used to toggle your tiny status (Client-Side only currently)
- Running now takes stamina and the new walk speed is around the same speed as the old running speed
- Player size changed to 33%
- Adjusted gravity to be 25% less effective on the small players
- (REVERTED IN 1.0.3) When spawning in, the player is normal sized for a few seconds, this is to help combat getting stuck when spawning back in (Note: this may not have fixed it entirely but it should help)

Pre Release:
- Changed Player size to 20%
- Adjusted Jump, Speed, and Grab Distance values