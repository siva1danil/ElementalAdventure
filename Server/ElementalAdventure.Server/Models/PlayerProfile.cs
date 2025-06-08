namespace ElementalAdventure.Server.Models;

public readonly struct PlayerProfile(long uid, int seed = 0, int floor = 0) {
    public readonly long Uid = uid;
    public readonly int Seed = seed;
    public readonly int Floor = floor;
}
