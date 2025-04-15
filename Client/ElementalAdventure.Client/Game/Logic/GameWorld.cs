using ElementalAdventure.Client.Game.Logic.Command;
using ElementalAdventure.Client.Game.Logic.GameObject;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.Logic;

public class GameWorld {
    private readonly int _tickInterval;

    private readonly Tilemap _tilemap;
    private readonly List<Entity> _entities;
    private readonly Queue<ICommand> _commands;

    private Vector2 _input;
    private long _tickTimestamp;

    public int TickInterval => _tickInterval;

    public Tilemap Tilemap => _tilemap;
    public List<Entity> Entities => _entities;

    public Vector2 Input { get => _input; set => _input = value; }
    public long TickTimestamp => _tickTimestamp;

    public GameWorld(int tickInterval, Tilemap tilemap, List<Entity> entities) {
        _tickInterval = tickInterval;

        _tilemap = tilemap;
        _entities = entities;

        _input = Vector2.Zero;
        _tickTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        _commands = [];
    }

    public void AddCommand(ICommand command) => _commands.Enqueue(command);

    public void Tick() {
        foreach (ICommand command in _commands) {
            command.Execute(this);
        }

        foreach (Entity entity in _entities) {
            entity.PositionComponent.Update(this, entity);
            entity.PlayerControllableComponent?.Update(this, entity);
        }

        _tickTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}