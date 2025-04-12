namespace ElementalAdventure.Client.Game.Logic;

public class GameWorld {
    private readonly int _tickInterval;
    private readonly Tilemap _tilemap;
    private readonly List<Entity> _entities;

    private long _tickTimestamp;

    public int TickInterval => _tickInterval;
    public Tilemap Tilemap => _tilemap;
    public List<Entity> Entities => _entities;
    public long TickTimestamp => _tickTimestamp;

    public GameWorld(int tickIntervalMilliseconds, Tilemap tilemap, List<Entity> entities) {
        _tickInterval = tickIntervalMilliseconds;
        _tilemap = tilemap;
        _entities = entities;

        _tickTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    public void Tick() {
        foreach (Entity entity in _entities) {
            entity.Position += entity.Velocity;
        }

        _tickTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}