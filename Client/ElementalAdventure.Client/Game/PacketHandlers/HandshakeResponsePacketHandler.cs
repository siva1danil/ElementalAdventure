using ElementalAdventure.Client.Game.SystemLogic;
using ElementalAdventure.Client.Game.SystemLogic.Command;
using ElementalAdventure.Common;
using ElementalAdventure.Common.Packets;
using ElementalAdventure.Common.Packets.Impl;

namespace ElementalAdventure.Client.Game.PacketHandlers;

public class HandshakeResponsePacketHandler : PacketRegistry.IPacketHandler {
    private readonly ClientContext _context;
    private readonly string _authProvider, _authToken;

    public HandshakeResponsePacketHandler(ClientContext context, string authProvider, string authToken) {
        _context = context;
        _authProvider = authProvider;
        _authToken = authToken;
    }

    public void Handle(PacketConnection connection, IPacket ipacket) {
        if (ipacket is not HandshakeResponsePacket packet) {
            _context.CommandQueue.Enqueue(new CrashCommand("Expected HandshakeResponsePacket, got " + ipacket.GetType().Name));
            return;
        }

        if (packet.ResultCode != 0)
            _context.CommandQueue.Enqueue(new CrashCommand("Handshake failed: " + packet.ResultMessage));
        else if (_context.PacketClient.Connection == null)
            _context.CommandQueue.Enqueue(new CrashCommand("Handshake failed: Connection is null"));
        else
            _ = _context.PacketClient.Connection.SendAsync(new LoginRequestPacket { Provider = _authProvider, Token = _authToken });
    }
}