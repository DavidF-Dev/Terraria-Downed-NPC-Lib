/*
 *  DownedCmd.cs
 *  DavidFDev
*/

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DownedNPCLib.Internals;

/// <summary>
///     Debug command to get or set the downed count.
/// </summary>
// ReSharper disable once UnusedType.Global
internal sealed class DownedCmd : ModCommand
{
    #region Properties

    public override string Command => "downed";

    public override string Usage => $"{Command} <get|get2|set>";

    public override string Description => "Get or set the downed count for a specified npc net id.";

    public override CommandType Type => CommandType.Chat | CommandType.Console;

    #endregion

    #region Methods

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        if (args.Length < 1)
        {
            caller.Reply($"Invalid usage: {Usage}");
            return;
        }

        if (args[0] == "get")
        {
            if (args.Length < 2 || !int.TryParse(args[1], out var netId) || netId <= NPCID.NegativeIDCount || netId >= NPCLoader.NPCCount)
            {
                caller.Reply($"Invalid usage: {Command} get <netId>", Color.Red);
                return;
            }

            var count = DownedNPC.GetCountByNetId(netId);
            caller.Reply($"{Lang.GetNPCName(netId)} downed [c/{Color.Yellow.Hex3()}:{count}] time(s) in world {Main.worldName}", Color.White);
        }
        else if (args[0] == "get2")
        {
            if (args.Length < 2 || !int.TryParse(args[1], out var type) || type < 0 || type >= NPCLoader.NPCCount)
            {
                caller.Reply($"Invalid usage: {Command} get2 <type>", Color.Red);
                return;
            }

            var count = DownedNPC.GetCountByType(type);
            caller.Reply($"{Lang.GetNPCName(type)} (& variants) downed [c/{Color.Yellow.Hex3()}:{count}] time(s) in world {Main.worldName}", Color.White);
        }
        else if (args[0] == "set")
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                caller.Reply("Cannot set the downed count from a multiplayer client", Color.Red);
                return;
            }

            if (args.Length < 3 || !int.TryParse(args[1], out var netId) || netId <= NPCID.NegativeIDCount || netId >= NPCLoader.NPCCount ||
                !int.TryParse(args[2], out var count) || count < 0)
            {
                caller.Reply($"Invalid usage: {Command} set <netId> <count>", Color.Red);
                return;
            }

            DownedNPCSystem.SetCount(netId, count);
            caller.Reply($"{Lang.GetNPCName(netId)} downed count set to [c/{Color.Yellow.Hex3()}:{count}] in world {Main.worldName}", Color.White);
        }
        else
        {
            caller.Reply($"Invalid usage: {Usage}", Color.Red);
        }
    }

    public override bool IsLoadingEnabled(Mod mod)
    {
#if DEBUG
        return true;
#else
        return false;
#endif
    }

    #endregion
}