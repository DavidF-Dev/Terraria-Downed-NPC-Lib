/*
 *  DownedNPC.cs
 *  DavidFDev
*/

using System;
using System.Diagnostics.Contracts;
using Terraria;

namespace DownedNPCLib;

/// <summary>
///     Access information about downed npcs in the current world.
/// </summary>
public static class DownedNPC
{
    #region Static Methods

    /// <summary>
    ///     Get all downed npc counts by npc type.
    ///     Length is equal to NPCLoader.NPCCount.
    /// </summary>
    [Pure]
    public static ReadOnlySpan<int> GetAll()
    {
        return DownedNPCSystem.DownedNPCs;
    }

    /// <summary>
    ///     Get whether the specified npc type has been downed in this world.
    /// </summary>
    [Pure]
    public static bool Get(int type)
    {
        return DownedNPCSystem.DownedNPCs[type] > 0;
    }

    /// <summary>
    ///     Get whether the specified npc type has been downed in this world.
    /// </summary>
    [Pure]
    public static bool Get(NPC npc)
    {
        return DownedNPCSystem.DownedNPCs[npc.type] > 0;
    }

    /// <summary>
    ///     Get how many times the specified npc type has been downed in this world.
    /// </summary>
    [Pure]
    public static int GetCount(int type)
    {
        return DownedNPCSystem.DownedNPCs[type];
    }

    /// <summary>
    ///     Get how many times the specified npc type has been downed in this world.
    /// </summary>
    [Pure]
    public static int GetCount(NPC npc)
    {
        return DownedNPCSystem.DownedNPCs[npc.type];
    }

    #endregion
}