using ElementalAdventure.Common;
using ElementalAdventure.Common.Logging;
using ElementalAdventure.Common.Packets;
using ElementalAdventure.Common.Packets.Impl;

namespace ElementalAdventure.Server.PacketHandlers;

public class HandshakeRequestPacketHandler : PacketRegistry.IPacketHandler {
    public void Handle(PacketConnection connection, IPacket ipacket) {
        if (ipacket is not HandshakeRequestPacket packet) {
            Logger.Info("Expected HandshakeRequestPacket, got " + ipacket.GetType().Name);
            return;
        }

        if (connection == null)
            Logger.Error("Connection is null");
        else
            _ = connection.SendAsync(new HandshakeResponsePacket() { ResultCode = 0, ResultMessage = "OK" });
    }
}