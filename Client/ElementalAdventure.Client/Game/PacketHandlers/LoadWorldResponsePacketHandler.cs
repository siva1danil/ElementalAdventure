using ElementalAdventure.Client.Game.Scenes;
using ElementalAdventure.Client.Game.SystemLogic;
using ElementalAdventure.Client.Game.SystemLogic.Command;
using ElementalAdventure.Common;
using ElementalAdventure.Common.Assets;
using ElementalAdventure.Common.Packets;
using ElementalAdventure.Common.Packets.Impl;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.PacketHandlers;

public class LoadWorldResponsePacketHandler : PacketRegistry.IPacketHandler {
    private readonly ClientWindow _window;
    private readonly ClientContext _context;

    public LoadWorldResponsePacketHandler(ClientWindow window, ClientContext context) {
        _window = window;
        _context = context;
    }

    public void Handle(PacketConnection connection, IPacket ipacket) {
        if (ipacket is not LoadWorldResponsePacket packet) {
            _context.CommandQueue.Enqueue(new CrashCommand("Expected LoadWorldResponsePacket, got " + ipacket.GetType().Name));
            return;
        }

        AssetID[,,] tilemap = new AssetID[packet.Tilemap.GetLength(0), packet.Tilemap.GetLength(1), packet.Tilemap.GetLength(2)];
        for (int z = 0; z < packet.Tilemap.GetLength(0); z++)
            for (int y = 0; y < packet.Tilemap.GetLength(1); y++)
                for (int x = 0; x < packet.Tilemap.GetLength(2); x++)
                    tilemap[z, y, x] = new AssetID(packet.Tilemap[z, y, x]);
        Box2[] walls = new Box2[packet.Walls.Length];
        for (int i = 0; i < packet.Walls.Length; i++)
            walls[i] = new Box2(new Vector2(packet.Walls[i].Item1, packet.Walls[i].Item2), new Vector2(packet.Walls[i].Item1 + packet.Walls[i].Item3, packet.Walls[i].Item2 + packet.Walls[i].Item4));
        _context.CommandQueue.Enqueue(new SetTilemapCommand(tilemap, walls, packet.Midground, new Vector2(packet.PlayerPosition.Item1, packet.PlayerPosition.Item2), new Vector2(packet.Exit.Item1, packet.Exit.Item2), packet.Floor));
    }
}