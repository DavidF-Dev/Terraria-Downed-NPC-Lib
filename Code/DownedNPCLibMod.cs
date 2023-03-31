/*
 *  DownedNPCLibMod.cs
 *  DavidFDev
*/

using Terraria.ModLoader;

namespace DownedNPCLib;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class DownedNPCLibMod : Mod
{
    #region Static Fields and Constants

    private const byte ModCallGetByNetId = 0;
    private const byte ModCallGetByType = 1;

    #endregion

    #region Methods

    public override object Call(params object[] args)
    {
        // TODO: Implement mod calls
        return null;
    }

    #endregion
}