namespace ElementalAdventure.Common.Packets;

public enum PacketType : ushort {
    HandshakeRequest = 0x0001,
    HandshakeResponse = 0x0002,
    LoginRequest = 0x0003,
    LoginResponse = 0x0004,
    LoadWorldRequest = 0x0005,
    LoadWorldResponse = 0x0006,
    SpawnEntity = 0x0007
}