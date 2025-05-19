namespace ElementalAdventure.Common.Packets;

public enum PacketType : ushort {
    HandshakeRequest = 0x0001,
    HandshakeResponse = 0x0002,
    LoginRequest = 0x0003,
    LoginResponse = 0x0004,
}