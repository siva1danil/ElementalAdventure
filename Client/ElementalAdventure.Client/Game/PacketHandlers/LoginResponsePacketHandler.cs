using ElementalAdventure.Client.Game.Scenes;
using ElementalAdventure.Client.Game.SystemLogic;
using ElementalAdventure.Client.Game.SystemLogic.Command;
using ElementalAdventure.Common;
using ElementalAdventure.Common.Packets;
using ElementalAdventure.Common.Packets.Impl;

namespace ElementalAdventure.Client.Game.PacketHandlers;

public class LoginResponsePacketHandler : PacketRegistry.IPacketHandler {
    private readonly ClientContext _context;

    public LoginResponsePacketHandler(ClientContext context) {
        _context = context;
    }

    public void Handle(PacketConnection connection, IPacket ipacket) {
        if (ipacket is not LoginResponsePacket packet) {
            _context.CommandQueue.Enqueue(new CrashCommand("Expected LoginResponsePacket, got " + ipacket.GetType().Name));
            return;
        }

        if (packet.ResultCode != 0)
            _context.CommandQueue.Enqueue(new CrashCommand("Login failed: " + packet.ResultMessage));
        else if (_context.PacketClient.Connection == null)
            _context.CommandQueue.Enqueue(new CrashCommand("Login failed: Connection is null"));
        else {
            _context.CommandQueue.Enqueue(new SetSceneCommand(new GameScene(_context)));
            _ = connection.SendAsync(new LoadWorldRequestPacket());
        }
    }
}