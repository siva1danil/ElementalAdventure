using ElementalAdventure.Common;
using ElementalAdventure.Common.Logging;
using ElementalAdventure.Common.Packets;
using ElementalAdventure.Common.Packets.Impl;
using ElementalAdventure.Server.Models;
using ElementalAdventure.Server.Storage;

namespace ElementalAdventure.Server.PacketHandlers;

public class LoginRequestPacketHandler : PacketRegistry.IPacketHandler {
    private readonly IDatabase _database;

    public LoginRequestPacketHandler(IDatabase database) {
        _database = database;
    }

    public void Handle(PacketConnection connection, IPacket ipacket) {
        if (ipacket is not LoginRequestPacket packet) {
            Logger.Info("Expected LoginRequestPacket, got " + ipacket.GetType().Name);
            return;
        }

        if (connection == null) {
            Logger.Error("Connection is null");
            return;
        }

        try {
            if (connection == null) return;
            ClientToken? token = _database.GetClientToken(packet.Provider, packet.Token);
            if (token == null) {
                PlayerProfile profile = _database.CreatePlayerProfile();
                token = _database.CreateClientToken(packet.Provider, packet.Token, profile.Uid);
            }
            connection.SessionStorage["uid"] = token.Value.Uid;
            _ = connection.SendAsync(new LoginResponsePacket() { ResultCode = 0, ResultMessage = "OK", Uid = token.Value.Uid });
        } catch (Exception ex) {
            _ = connection.SendAsync(new LoginResponsePacket() { ResultCode = 1, ResultMessage = $"Error processing login request: {ex.Message}", Uid = 0 });
        }
    }
}