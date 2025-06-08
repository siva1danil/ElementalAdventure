using ElementalAdventure.Common;
using ElementalAdventure.Common.Assets;
using ElementalAdventure.Common.Logging;
using ElementalAdventure.Common.Packets;
using ElementalAdventure.Common.Packets.Impl;
using ElementalAdventure.Server.Models;
using ElementalAdventure.Server.Storage;
using ElementalAdventure.Server.World;

namespace ElementalAdventure.Server.PacketHandlers;

public class DiePacketHandler : PacketRegistry.IPacketHandler {
    private readonly IDatabase _database;

    public DiePacketHandler(IDatabase database) {
        _database = database;
    }

    public void Handle(PacketConnection connection, IPacket ipacket) {
        if (ipacket is not DiePacket packet) {
            Logger.Info("Expected DiePacket, got " + ipacket.GetType().Name);
            return;
        }

        if (connection == null) {
            Logger.Error("Connection is null");
            return;
        }

        long uid = (long)connection.SessionStorage["uid"];

        PlayerProfile? profile = _database.GetPlayerProfile(uid);
        if (profile == null) {
            Logger.Error($"Player profile not found for UID {uid}");
            return;
        }
        profile = new PlayerProfile(profile.Value.Uid, new Random().Next(), 0);
        _database.UpdatePlayerProfile(profile.Value);
    }
}