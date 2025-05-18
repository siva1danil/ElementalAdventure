namespace ElementalAdventure.Server.Models;

public readonly struct PlayerProfile(long uid) {
    public readonly long Uid = uid;
}
