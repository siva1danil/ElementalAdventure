using ElementalAdventure.Server.Models;

namespace ElementalAdventure.Server.Storage;

public interface IDatabase {
    public void Connect();
    public void Disconnect();

    public PlayerProfile CreatePlayerProfile();
    public PlayerProfile? GetPlayerProfile(long uid);
    public ClientToken CreateClientToken(string provider, string token, long uid);
    public ClientToken? GetClientToken(string provider, string token);
}