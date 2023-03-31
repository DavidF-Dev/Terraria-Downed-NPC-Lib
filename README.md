# Downed NPC Library
[![Release](https://img.shields.io/github/v/release/DavidF-Dev/Terraria-Downed-NPC-Lib?style=flat-square)](https://github.com/DavidF-Dev/Terraria-Downed-NPC-Lib/releases/latest)
[![Downloads](https://img.shields.io/steam/downloads/0?style=flat-square)](https://steamcommunity.com/sharedfiles/filedetails/?id=0)
[![File Size](https://img.shields.io/steam/size/0?style=flat-square)](https://steamcommunity.com/sharedfiles/filedetails/?id=0)
[![Issues](https://img.shields.io/github/issues/DavidF-Dev/Terraria-Downed-NPC-Lib?style=flat-square)](https://github.com/DavidF-Dev/Terraria-Downed-NPC-Lib/issues)
[![License](https://img.shields.io/github/license/DavidF-Dev/Terraria-Downed-NPC-Lib?style=flat-square)](https://github.com/DavidF-Dev/Terraria-Downed-NPC-Lib/blob/main/LICENSE.md)

A Terraria tModLoader library mod for keeping track of how many times NPCs are killed per world.<br />
This includes enemies, town NPCs, bosses, critters, etc.

## Usage
### Requirements
- tModLoader for `1.4.4`.
- `Side = Both` in `build.txt` (default if not specified).

### Referencing the library
- Add `modReferences = DownedNPCLib` to your mod's `build.txt` file.
- Add `DownedNPCLib.dll` to your project as a reference (download from [Releases](https://github.com/DavidF-Dev/Terraria-Downed-NPC-Lib/releases/latest)).
- Subscribe to the library mod on the [Steam Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=0).

Note: this library depends on my [Easy Packet Library](https://github.com/DavidF-Dev/Terraria-Easy-Packets-Lib).
tModLoader should handle subscribing to it automatically.
In the odd case that it doesn't, simply subscribe to it manually.

### Public methods
All calls can be made through the `DownedNPC` static class.
```csharp
int pinkyKills = DownedNPC.GetCountByNetId(NPCID.Pinky);
int flinxKills = DownedNPC.GetCountByType(NPCID.SnowFlinx);
if (DownedNPC.GetByNetId(NPCID.SmallZombie)
{
}
```
There are two versions of each method: one for <ins>net ids</ins>, and the other for <ins>types</ins>. What is the difference?
- Net ids are used in special cases to distinguish some vanilla NPCs (slimes, zombies) that share a common type.
- Types are what most NPCs use, including all modded NPCs.

In most cases you should use the type variant, unless you need to check a specific NPC net id.

### Mod calls
WIP.

## Contact & Support

If you have any questions or would like to get in contact, shoot me an email at `contact@davidfdev.com`.<br>
Alternatively, you can send me a direct message on Twitter at [@DavidF_Dev](https://twitter.com/DavidF_Dev).