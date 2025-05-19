namespace ElementalAdventure.Common.Packets.Impl;

public class LoginResponsePacket : IPacket {
    public PacketType Type => PacketType.LoginResponse;

    public byte ResultCode { get; set; } = 0;
    public string ResultMessage { get; set; } = string.Empty;
    public long Uid { get; set; } = 0;

    public void Serialize(BinaryWriter writer) {
        writer.Write(ResultCode);
        writer.Write(System.Text.Encoding.UTF8.GetByteCount(ResultMessage));
        writer.Write(System.Text.Encoding.UTF8.GetBytes(ResultMessage));
        writer.Write(Uid);
    }

    public static LoginResponsePacket Deserialize(BinaryReader reader) {
        LoginResponsePacket packet = new LoginResponsePacket();
        packet.ResultCode = reader.ReadByte();
        packet.ResultMessage = System.Text.Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));
        packet.Uid = reader.ReadInt64();
        return packet;
    }
}