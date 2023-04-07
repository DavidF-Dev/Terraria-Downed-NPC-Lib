/*
 *  DownedNPCLibMod.cs
 *  DavidFDev
*/

using System;
using Terraria.ModLoader;

namespace DownedNPCLib.Internals;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class DownedNPCLibMod : Mod
{
    #region Methods

    public override object Call(params object[] args)
    {
        if (args.Length < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(args));
        }

        if (args[0] is not string callName)
        {
            throw new ArgumentException($"Invalid argument 0: '{args[0]?.GetType().Name}'. Expected: '{nameof(String)}'.", nameof(args));
        }

        // Usage: int result = mod.Call("GetCountByNetId", NPCID.Pinky);
        // Usage: int result = mod.Call("GetCountByType", NPCID.SnowFlinx);
        var getCountByNetId = callName == "GetCountByNetId";
        var getCountByType = callName == "GetCountByType";
        if (getCountByNetId || getCountByType)
        {
            if (args.Length < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(args));
            }

            int id;
            try
            {
                id = Convert.ToInt32(args[1]);
            }
            catch (Exception)
            {
                throw new ArgumentException($"Invalid argument 1: '{args[1]?.GetType().Name}'. Expected: '{nameof(Int32)}'.", nameof(args));
            }

            var count = getCountByNetId ? DownedNPC.GetCountByNetId(id) : DownedNPC.GetCountByType(id);
            return count;
        }

        throw new ArgumentException($"Invalid mod call: '{callName}'.", nameof(args));
    }

    #endregion
}