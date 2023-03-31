using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace DownedNPCLib;

// ReSharper disable once UnusedType.Global
internal sealed class DownedNPCCommand : ModCommand
{
    #region Properties

    public override string Command => "downed";

    public override string Usage => $"{Command} <type>";

    public override string Description => "Get how many times the specified npc type has been downed in this world.";

    public override CommandType Type => CommandType.Chat | CommandType.Console;

    #endregion

    #region Methods

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        if (args.Length == 0 || !int.TryParse(args[0], out var type) || type < 0 || type >= NPCLoader.NPCCount)
        {
            caller.Reply($"Invalid usage: {Usage}");
            return;
        }

        caller.Reply($"{Lang.GetNPCName(type)} downed {DownedNPC.GetCount(type)} times", Color.White);
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