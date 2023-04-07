/*
 *  DownedNPCPacket.cs
 *  DavidFDev
*/

using System.IO;
using EasyPacketsLib;

namespace DownedNPCLib.Internals;

/// <summary>
///     Packet used to sync a downed npc count between server and client.
/// </summary>
internal readonly struct DownedNPCPacket : IEasyPacket<DownedNPCPacket>
{
    #region Static Fields and Constants

    /// <summary>
    ///     Sent by client to request a sync from the server.
    ///     Note, this will sync only those with a count greater than zero.
    /// </summary>
    public static readonly DownedNPCPacket RequestSync = new(-1, -1);

    #endregion

    #region Fields

    /// <summary>
    ///     Actual index in the downed array; not the npc net id or type.
    /// </summary>
    public readonly int Index;

    /// <summary>
    ///     Downed count.
    /// </summary>
    public readonly int Count;

    #endregion

    #region Constructors

    public DownedNPCPacket(int index, int count)
    {
        Index = index;
        Count = count;
    }

    #endregion

    #region Methods

    void IEasyPacket<DownedNPCPacket>.Serialise(BinaryWriter writer)
    {
        writer.Write(Index);
        writer.Write(Count);
    }

    DownedNPCPacket IEasyPacket<DownedNPCPacket>.Deserialise(BinaryReader reader, in SenderInfo sender)
    {
        return new DownedNPCPacket(reader.ReadInt32(), reader.ReadInt32());
    }

    #endregion
}