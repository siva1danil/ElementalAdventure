namespace ElementalAdventure.Common.Packets;

public class HandshakeRequestPacket : IPacket {
    public PacketType Type => PacketType.HandshakeRequest;

    public int ClientVersion { get; set; } = 0;

    public void Serialize(BinaryWriter writer) {
        writer.Write(ClientVersion);
    }

    public static HandshakeRequestPacket Deserialize(BinaryReader reader) {
        HandshakeRequestPacket packet = new HandshakeRequestPacket();
        packet.ClientVersion = reader.ReadInt32();
        return packet;
    }
}