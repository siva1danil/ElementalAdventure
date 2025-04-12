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

        _tickTimestamp = (long)DateTime.UtcNow.TimeOfDay.TotalMilliseconds;
    }

    public void Tick() {
        foreach (Entity entity in _entities) {
            entity.PositionLast = entity.PositionCurrent;
            entity.PositionCurrent += entity.Velocity;
        }

        _tickTimestamp = (long)DateTime.UtcNow.TimeOfDay.TotalMilliseconds;
    }
}