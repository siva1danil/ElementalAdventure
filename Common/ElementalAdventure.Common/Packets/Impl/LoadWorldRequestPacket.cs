namespace ElementalAdventure.Common.Packets.Impl;

public class LoadWorldRequestPacket : IPacket {
    public PacketType Type => PacketType.LoadWorldRequest;

    public void Serialize(BinaryWriter writer) {
        //
    }

    public static LoadWorldRequestPacket Deserialize(BinaryReader reader) {
        return new LoadWorldRequestPacket();
    }
}