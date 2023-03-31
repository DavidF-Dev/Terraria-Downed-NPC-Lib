/*
 *  DownedNPCPacket.cs
 *  DavidFDev
*/

using System.IO;
using EasyPacketsLib;

namespace DownedNPCLib;

internal readonly struct DownedNPCPacket : IEasyPacket<DownedNPCPacket>
{
    #region Static Fields and Constants

    public static readonly DownedNPCPacket RequestSync = new(-1, -1);

    #endregion

    #region Fields

    public readonly int Type;

    public readonly int Count;

    #endregion

    #region Constructors

    public DownedNPCPacket(int type, int count)
    {
        Type = type;
        Count = count;
    }

    #endregion

    #region Methods

    void IEasyPacket<DownedNPCPacket>.Serialise(BinaryWriter writer)
    {
        writer.Write(Type);
        writer.Write(Count);
    }

    DownedNPCPacket IEasyPacket<DownedNPCPacket>.Deserialise(BinaryReader reader, in SenderInfo sender)
    {
        return new DownedNPCPacket(reader.ReadInt32(), reader.ReadInt32());
    }

    #endregion
}