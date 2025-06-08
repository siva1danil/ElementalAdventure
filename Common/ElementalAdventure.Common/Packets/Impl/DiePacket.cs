namespace ElementalAdventure.Common.Packets.Impl;

public class DiePacket : IPacket {
    public PacketType Type => PacketType.Die;

    public void Serialize(BinaryWriter writer) { }

    public static DiePacket Deserialize(BinaryReader reader) {
        DiePacket packet = new();
        return packet;
    }
}