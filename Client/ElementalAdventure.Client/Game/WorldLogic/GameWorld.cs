using ElementalAdventure.Client.Game.WorldLogic.Command;
using ElementalAdventure.Client.Game.WorldLogic.GameObject;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.WorldLogic;

public class GameWorld {
    private readonly double _tickInterval;

    private readonly Tilemap _tilemap;
    private readonly List<Entity> _entities;
    private readonly Queue<ICommand> _commands;

    private Vector2 _input;
    private long _tickTimestamp;

    public double TickInterval => _tickInterval;

    public Tilemap Tilemap => _tilemap;
    public List<Entity> Entities => _entities;

    public Vector2 Input { get => _input; set => _input = value; }
    public long TickTimestamp => _tickTimestamp;

    public GameWorld(double tickInterval, Tilemap tilemap, List<Entity> entities) {
        _tickInterval = tickInterval;

        _tilemap = tilemap;
        _entities = entities;

        _input = Vector2.Zero;
        _tickTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        _commands = [];
    }

    public void Tick() {
        while (_commands.Count > 0)
            _commands.Dequeue().Execute(this);
        foreach (Entity entity in _entities)
            entity.Update(this);
        _tickTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    public void AddCommand(ICommand command) {
        _commands.Enqueue(command);
    }

    public void RemoveEntity(Entity entity) {
        _entities.Remove(entity);
    }
}