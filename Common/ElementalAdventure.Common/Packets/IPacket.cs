namespace ElementalAdventure.Common.Packets;

public interface IPacket {
    public PacketType Type { get; }
    public void Serialize(BinaryWriter writer);
}