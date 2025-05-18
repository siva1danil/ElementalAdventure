namespace ElementalAdventure.Common.Packets;

public class HandshakeResponsePacket : IPacket {
    public PacketType Type => PacketType.HandshakeResponse;

    public byte ResultCode { get; set; } = 0;
    public string ResultMessage { get; set; } = string.Empty;

    public void Serialize(BinaryWriter writer) {
        writer.Write(ResultCode);
        writer.Write(System.Text.Encoding.UTF8.GetByteCount(ResultMessage));
        writer.Write(System.Text.Encoding.UTF8.GetBytes(ResultMessage));
    }

    public static HandshakeResponsePacket Deserialize(BinaryReader reader) {
        HandshakeResponsePacket packet = new HandshakeResponsePacket();
        packet.ResultCode = reader.ReadByte();
        packet.ResultMessage = System.Text.Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));
        return packet;
    }
}