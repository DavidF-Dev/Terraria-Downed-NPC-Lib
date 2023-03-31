/*
 *  DownedNPCSystem.cs
 *  DavidFDev
*/

using System;
using System.Collections.Generic;
using EasyPacketsLib;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace DownedNPCLib;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class DownedNPCSystem : ModSystem
{
    #region Static Fields and Constants

    /// <summary>
    ///     All downed npc counts by full name, including those from unloaded mods.
    ///     This is kept up-to-date in single player and server, not on client.
    /// </summary>
    private static readonly Dictionary<string, int> DownedNPCsByFullName = new();

    /// <summary>
    ///     Downed npc counts by npc type.
    ///     This is kept up-to-date in single player and multi player.
    /// </summary>
    private static int[] _downedNPCs;

    #endregion

    #region Static Methods

    /// <summary>
    ///     Allows you to make things happen when an NPC dies (for example, setting ModSystem fields).
    ///     This hook runs on the server/single player.
    /// </summary>
    private static void OnDoDeathEvents(On_NPC.orig_DoDeathEvents orig, NPC self, Player closestPlayer)
    {
        orig.Invoke(self, closestPlayer);

        _downedNPCs[self.type] += 1;
        DownedNPCsByFullName[GetFullNameFromType(self.type)] = _downedNPCs[self.type];

        if (Main.netMode == NetmodeID.Server)
        {
            ModContent.GetInstance<DownedNPCLibMod>().SendPacket(new DownedNPCPacket(self.type, _downedNPCs[self.type]));
        }
    }

    /// <summary>
    ///     Invoked by clients when notified by the server that an npc was downed.
    /// </summary>
    // ReSharper disable once RedundantAssignment
    private static void OnDownedNPCPacketReceived(in DownedNPCPacket packet, in SenderInfo sender, ref bool handled)
    {
        handled = true;

        // If received by client, then simply update the count given by the server
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            _downedNPCs[packet.Type] = packet.Count;
            return;
        }

        // If received by server, then a joining client needs to be synced
        // Note, this will sync only those with a count greater than zero
        foreach (var type in _downedNPCs)
        {
            if (_downedNPCs[type] > 0)
            {
                sender.Mod.SendPacket(new DownedNPCPacket(type, _downedNPCs[type]), sender.WhoAmI);
            }
        }
    }

    /// <summary>
    ///     Get the npc's full name from a loaded npc type.
    /// </summary>
    private static string GetFullNameFromType(int type)
    {
        return type < NPCID.Count ? NPCID.Search.GetName(type) : NPCLoader.GetNPC(type).FullName;
    }

    /// <summary>
    ///     Get the npc type from a loaded npc's full name.
    /// </summary>
    private static int GetTypeFromFullName(string fullName)
    {
        if (NPCID.Search.TryGetId(fullName, out var type))
        {
            return type;
        }

        if (ModContent.TryFind(fullName, out ModNPC npc))
        {
            return npc.Type;
        }

        return -1;
    }

    #endregion

    #region Properties

    /// <summary>
    ///     Downed npc counts by npc type.
    /// </summary>
    public static ReadOnlySpan<int> DownedNPCs => new(_downedNPCs);

    #endregion

    #region Methods

    public override void PostSetupContent()
    {
        // Initialise the array now that vanilla arrays have been resized and NPCs have been loaded by other mods
        _downedNPCs = new int[NPCLoader.NPCCount];
    }

    public override void Load()
    {
        On_NPC.DoDeathEvents += OnDoDeathEvents;
        Mod.AddPacketHandler<DownedNPCPacket>(OnDownedNPCPacketReceived);
    }

    public override void Unload()
    {
        On_NPC.DoDeathEvents -= OnDoDeathEvents;
        Mod.RemovePacketHandler<DownedNPCPacket>(OnDownedNPCPacketReceived);
        _downedNPCs = null;
    }

    public override void ClearWorld()
    {
        DownedNPCsByFullName.Clear();
        Array.Clear(_downedNPCs);
    }

    public override void SaveWorldData(TagCompound tag)
    {
        // Save all counts, including those from unloaded mods
        var downedNPCsTag = new TagCompound();
        foreach (var (fullName, count) in DownedNPCsByFullName)
        {
            if (count > 0)
            {
                downedNPCsTag.Add(fullName, count);
            }
        }

        tag.Add("downedNPCs", downedNPCsTag);
    }

    public override void LoadWorldData(TagCompound tag)
    {
        if (!tag.TryGet("downedNPCs", out TagCompound downedNPCsTag))
        {
            // None saved
            return;
        }

        // Load all counts, including those from unloaded mods
        foreach (var (fullName, count) in downedNPCsTag)
        {
            var c = (int)count;

            DownedNPCsByFullName.Add(fullName, c);

            // Track counts from loaded mods
            var type = GetTypeFromFullName(fullName);
            if (type != -1)
            {
                _downedNPCs[type] = c;
            }
        }
    }

    #endregion

    #region Nested Types

    // ReSharper disable once UnusedType.Local
    private sealed class DownedNPCsPlayer : ModPlayer
    {
        #region Methods

        public override void OnEnterWorld()
        {
            // If joining a server, request the server to sync info
            // Note, this will sync only those with a count greater than zero
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                Mod.SendPacket(DownedNPCPacket.RequestSync);
            }
        }

        #endregion
    }

    #endregion
}