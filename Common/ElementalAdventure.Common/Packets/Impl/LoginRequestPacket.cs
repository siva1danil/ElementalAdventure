namespace ElementalAdventure.Common.Packets.Impl;

public class LoginRequestPacket : IPacket {
    public PacketType Type => PacketType.LoginRequest;

    public string Provider { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;

    public void Serialize(BinaryWriter writer) {
        writer.Write(System.Text.Encoding.UTF8.GetByteCount(Provider));
        writer.Write(System.Text.Encoding.UTF8.GetBytes(Provider));
        writer.Write(System.Text.Encoding.UTF8.GetByteCount(Token));
        writer.Write(System.Text.Encoding.UTF8.GetBytes(Token));
    }

    public static LoginRequestPacket Deserialize(BinaryReader reader) {
        LoginRequestPacket packet = new LoginRequestPacket();
        packet.Provider = System.Text.Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));
        packet.Token = System.Text.Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));
        return packet;
    }
}