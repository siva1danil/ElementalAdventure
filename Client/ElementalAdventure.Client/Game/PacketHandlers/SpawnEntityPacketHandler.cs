using ElementalAdventure.Client.Game.SystemLogic;
using ElementalAdventure.Client.Game.SystemLogic.Command;
using ElementalAdventure.Common;
using ElementalAdventure.Common.Packets;
using ElementalAdventure.Common.Packets.Impl;

namespace ElementalAdventure.Client.Game.PacketHandlers;

public class SpawnEntityPacketHandler : PacketRegistry.IPacketHandler {
    private readonly ClientContext _context;

    public SpawnEntityPacketHandler(ClientContext context) {
        _context = context;
    }

    public void Handle(PacketConnection connection, IPacket ipacket) {
        if (ipacket is not SpawnEntityPacket packet) {
            _context.CommandQueue.Enqueue(new CrashCommand("Expected SpawnEntityPacket, got " + ipacket.GetType().Name));
            return;
        }

        _context.CommandQueue.Enqueue(new SpawnEntityCommand(packet.EntityType, packet.Position));
    }
}