/*
 *  DownedNPC.cs
 *  DavidFDev
*/

using System.Diagnostics.Contracts;
using DownedNPCLib.Internals;
using Terraria.ID;

namespace DownedNPCLib;

/// <summary>
///     Access information about downed npcs in the current world.
/// </summary>
public static class DownedNPC
{
    #region Static Methods

    /// <summary>
    ///     Get whether the specified npc net id has been downed in this world.
    /// </summary>
    [Pure]
    public static bool GetByNetId(int netId)
    {
        return DownedNPCSystem.GetCount(netId) > 0;
    }

    /// <summary>
    ///     Get whether the specified npc type has been downed in this world, including any associated net ids.
    /// </summary>
    [Pure]
    public static bool GetByType(int type)
    {
        if (DownedNPCSystem.GetCount(type) > 0)
        {
            return true;
        }

        for (var i = -1; i > NPCID.NegativeIDCount; i--)
        {
            if (NPCID.FromNetId(i) == type && DownedNPCSystem.GetCount(i) > 0)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///     Get how many times the specified npc net id has been downed in this world.
    /// </summary>
    [Pure]
    public static int GetCountByNetId(int netId)
    {
        return DownedNPCSystem.GetCount(netId);
    }

    /// <summary>
    ///     Get how many times the specified npc type has been downed in this world, including any associated net ids.
    /// </summary>
    [Pure]
    public static int GetCountByType(int type)
    {
        var count = DownedNPCSystem.GetCount(type);
        for (var i = -1; i > NPCID.NegativeIDCount; i--)
        {
            if (NPCID.FromNetId(i) == type)
            {
                count += DownedNPCSystem.GetCount(i);
            }
        }

        return count;
    }

    #endregion
}