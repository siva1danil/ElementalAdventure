using ElementalAdventure.Common.Assets;

namespace ElementalAdventure.Common.Packets.Impl;

public class SpawnEntityPacket : IPacket {
    public PacketType Type => PacketType.SpawnEntity;

    public AssetID EntityType { get; set; }
    public (float X, float Y) Position { get; set; }

    public void Serialize(BinaryWriter writer) {
        writer.Write(EntityType.Value);
        writer.Write(Position.X);
        writer.Write(Position.Y);
    }

    public static SpawnEntityPacket Deserialize(BinaryReader reader) {
        SpawnEntityPacket packet = new SpawnEntityPacket();
        packet.EntityType = new AssetID(reader.ReadInt32());
        packet.Position = (reader.ReadSingle(), reader.ReadSingle());
        return packet;
    }
}