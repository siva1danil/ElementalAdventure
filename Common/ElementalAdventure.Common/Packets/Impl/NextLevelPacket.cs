namespace ElementalAdventure.Common.Packets.Impl;

public class NextLevelPacket : IPacket {
    public PacketType Type => PacketType.NextLevelRequest;

    public void Serialize(BinaryWriter writer) { }

    public static NextLevelPacket Deserialize(BinaryReader reader) {
        NextLevelPacket packet = new();
        return packet;
    }
}