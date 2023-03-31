# Downed NPC Library
[![Release](https://img.shields.io/github/v/release/DavidF-Dev/Terraria-Downed-NPC-Lib?style=flat-square)](https://github.com/DavidF-Dev/Terraria-Downed-NPC-Lib/releases/latest)
[![Downloads](https://img.shields.io/steam/downloads/2955286119?style=flat-square)](https://steamcommunity.com/sharedfiles/filedetails/?id=2955286119)
[![File Size](https://img.shields.io/steam/size/2955286119?style=flat-square)](https://steamcommunity.com/sharedfiles/filedetails/?id=2955286119)
[![Issues](https://img.shields.io/github/issues/DavidF-Dev/Terraria-Downed-NPC-Lib?style=flat-square)](https://github.com/DavidF-Dev/Terraria-Downed-NPC-Lib/issues)
[![License](https://img.shields.io/github/license/DavidF-Dev/Terraria-Downed-NPC-Lib?style=flat-square)](https://github.com/DavidF-Dev/Terraria-Downed-NPC-Lib/blob/main/LICENSE.md)

A Terraria tModLoader library mod for keeping track of how many times NPCs are killed per world.<br />
This includes enemies, town NPCs, bosses, critters, etc.

## Usage
### Requirements
- tModLoader for `1.4.4`.

### Referencing the library
- Add `modReferences = DownedNPCLib` to your mod's `build.txt` file.
- Add `DownedNPCLib.dll` to your project as a reference (download from [Releases](https://github.com/DavidF-Dev/Terraria-Downed-NPC-Lib/releases/latest)).
- Subscribe to the library mod on the [Steam Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=2955286119).

Note: this library depends on my [Easy Packet Library](https://github.com/DavidF-Dev/Terraria-Easy-Packets-Lib).
tModLoader should handle subscribing to it automatically.
In the odd case that it doesn't, simply subscribe to it manually.

### Public methods
All calls are available via the `DownedNPC` static class.
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

The count is updated when an NPC is killed, **after** kill-related hooks.

### Mod calls
The basic methods of this library are available via [mod calls](https://github.com/tModLoader/tModLoader/wiki/Expert-Cross-Mod-Content#call-aka-modcall-intermediate).
If using this approach, you do not need to reference the DLL in your project.
```csharp
Mod mod = ModLoader.GetMod("DownedNPCLib");
int pinkyKills = (int)mod.Call("GetCountByNetId", NPCID.Pinky);
int flinxKills = (int)mod.Call("GetCountByType", NPCID.SnowFlinx);
if ((int)mod.Call("GetByNetId", NPCID.SmallZombie) > 0)
{
}
```

## Contact & Support

If you have any questions or would like to get in contact, shoot me an email at `contact@davidfdev.com`.<br>
Alternatively, you can send me a direct message on Twitter at [@DavidF_Dev](https://twitter.com/DavidF_Dev).
