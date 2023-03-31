/*
 *  DownedNPCSystem.cs
 *  DavidFDev
*/

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
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
    ///     Downed npc counts by npc net id, shifted by |NPCID.NegativeIDCount| to account for negative ids.
    ///     This is kept up-to-date in single player and multi player.<br /><br />
    ///     Negative ids (net ids):
    ///     <code>0 -> NPCID.NegativeIDCount</code>
    ///     Positive ids (types):
    ///     <code>NPCID.NegativeIDCount -> (NPCLoader.NPCCount - NPCID.NegativeIDCount)</code>
    /// </summary>
    private static int[] _downedNPCs;

    #endregion

    #region Static Methods

    /// <summary>
    ///     Get how many times the specified npc net id has been downed in this world.
    /// </summary>
    [Pure]
    public static int GetCount(int netId)
    {
        if (netId <= NPCID.NegativeIDCount || netId >= NPCLoader.NPCCount)
        {
            throw new ArgumentOutOfRangeException(nameof(netId));
        }

        return _downedNPCs[GetIndex(netId)];
    }

    /// <summary>
    ///     Set how many times the specified npc net id has been downed in this world.
    ///     Updates clients if called by the server. Cannot be called by clients.
    /// </summary>
    public static void SetCount(int netId, int count)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            throw new Exception($"{nameof(SetCount)} called by multiplayer client.");
        }

        if (netId <= NPCID.NegativeIDCount || netId >= NPCLoader.NPCCount)
        {
            throw new ArgumentOutOfRangeException(nameof(netId));
        }

        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        // Follows same logic as OnDoDeathEvents
        var index = GetIndex(netId);
        _downedNPCs[index] = count;
        DownedNPCsByFullName[GetFullNameFromNetId(netId)] = count;

        if (Main.netMode == NetmodeID.Server)
        {
            ModContent.GetInstance<DownedNPCLibMod>().SendPacket(new DownedNPCPacket(index, count));
        }
    }

    /// <summary>
    ///     Get the actual index in <see cref="_downedNPCs" /> for the specified npc net id.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetIndex(int netId)
    {
        return netId < 0 ? Math.Abs(netId) - 1 : netId + Math.Abs(NPCID.NegativeIDCount);
    }

    /// <summary>
    ///     Allows you to make things happen when an NPC dies (for example, setting ModSystem fields).
    ///     This hook runs on the server/single player.
    /// </summary>
    private static void OnDoDeathEvents(On_NPC.orig_DoDeathEvents orig, NPC self, Player closestPlayer)
    {
        orig.Invoke(self, closestPlayer);

        var index = GetIndex(self.netID);
        var count = ++_downedNPCs[index];
        DownedNPCsByFullName[GetFullNameFromNetId(self.netID)] = count;

        if (Main.netMode == NetmodeID.Server)
        {
            ModContent.GetInstance<DownedNPCLibMod>().SendPacket(new DownedNPCPacket(index, count));
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
            _downedNPCs[packet.Index] = packet.Count;
            return;
        }

        // If received by server, then a joining client needs to be synced
        // Note, this will sync only those with a count greater than zero
        for (var i = 0; i < _downedNPCs.Length; i++)
        {
            var count = _downedNPCs[i];
            if (count > 0)
            {
                sender.Mod.SendPacket(new DownedNPCPacket(i, count), sender.WhoAmI);
            }
        }
    }

    /// <summary>
    ///     Get the npc's full name from a loaded npc net id.
    /// </summary>
    private static string GetFullNameFromNetId(int netId)
    {
        return netId < NPCID.Count ? NPCID.Search.GetName(netId) : NPCLoader.GetNPC(netId).FullName;
    }

    /// <summary>
    ///     Get the npc net id from a loaded npc's full name.
    /// </summary>
    private static bool GetNetIdFromFullName(string fullName, out int netId)
    {
        if (NPCID.Search.TryGetId(fullName, out netId))
        {
            return true;
        }

        if (ModContent.TryFind(fullName, out ModNPC npc))
        {
            netId = npc.Type;
            return true;
        }

        netId = default;
        return false;
    }

    #endregion

    #region Methods

    public override void PostSetupContent()
    {
        // Initialise the array now that vanilla arrays have been resized and NPCs have been loaded by other mods
        _downedNPCs = new int[NPCLoader.NPCCount + Math.Abs(NPCID.NegativeIDCount)];
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
        
        Mod.Logger.Debug($"Saved {downedNPCsTag.Count} entries for world '{Main.worldName}'.");
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
            if (GetNetIdFromFullName(fullName, out var netId))
            {
                _downedNPCs[GetIndex(netId)] = c;
            }
        }

        Mod.Logger.Debug($"Loaded {downedNPCsTag.Count} entries for world '{Main.worldName}'.");
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