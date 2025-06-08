namespace ElementalAdventure.Common.Packets.Impl;

public class LoadWorldResponsePacket : IPacket {
    public PacketType Type => PacketType.LoadWorldResponse;

    public int[,,] Tilemap { get; set; } = new int[0, 0, 0];
    public (float, float, float, float)[] Walls { get; set; } = [];
    public (float, float) PlayerPosition { get; set; } = (0.0f, 0.0f);
    public (float, float) Exit { get; set; } = (0.0f, 0.0f);
    public int Midground { get; set; } = 0;

    public void Serialize(BinaryWriter writer) {
        writer.Write((ushort)Tilemap.GetLength(0));
        writer.Write((ushort)Tilemap.GetLength(1));
        writer.Write((ushort)Tilemap.GetLength(2));
        for (int z = 0; z < Tilemap.GetLength(0); z++)
            for (int y = 0; y < Tilemap.GetLength(1); y++)
                for (int x = 0; x < Tilemap.GetLength(2); x++)
                    writer.Write(Tilemap[z, y, x]);
        writer.Write((ushort)Walls.Length);
        for (int i = 0; i < Walls.Length; i++) {
            writer.Write(Walls[i].Item1);
            writer.Write(Walls[i].Item2);
            writer.Write(Walls[i].Item3);
            writer.Write(Walls[i].Item4);
        }
        writer.Write(PlayerPosition.Item1);
        writer.Write(PlayerPosition.Item2);
        writer.Write(Exit.Item1);
        writer.Write(Exit.Item2);
        writer.Write((ushort)Midground);
    }

    public static LoadWorldResponsePacket Deserialize(BinaryReader reader) {
        LoadWorldResponsePacket packet = new();
        int zLength = reader.ReadUInt16();
        int yLength = reader.ReadUInt16();
        int xLength = reader.ReadUInt16();
        packet.Tilemap = new int[zLength, yLength, xLength];
        for (int z = 0; z < zLength; z++)
            for (int y = 0; y < yLength; y++)
                for (int x = 0; x < xLength; x++)
                    packet.Tilemap[z, y, x] = reader.ReadInt32();
        int wallsLength = reader.ReadUInt16();
        packet.Walls = new (float, float, float, float)[wallsLength];
        for (int i = 0; i < wallsLength; i++) {
            float item1 = reader.ReadSingle();
            float item2 = reader.ReadSingle();
            float item3 = reader.ReadSingle();
            float item4 = reader.ReadSingle();
            packet.Walls[i] = (item1, item2, item3, item4);
        }
        packet.PlayerPosition = (reader.ReadSingle(), reader.ReadSingle());
        packet.Exit = (reader.ReadSingle(), reader.ReadSingle());
        packet.Midground = reader.ReadUInt16();
        return packet;
    }
}