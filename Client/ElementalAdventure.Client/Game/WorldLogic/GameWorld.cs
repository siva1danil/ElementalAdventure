using ElementalAdventure.Client.Game.SystemLogic;
using ElementalAdventure.Client.Game.WorldLogic.Command;
using ElementalAdventure.Client.Game.WorldLogic.GameObject;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.WorldLogic;

public class GameWorld {
    private readonly double _tickInterval;

    private int _floor = 0;
    private readonly Tilemap _tilemap;
    private Vector2 _exit;
    private readonly List<Entity> _entities;
    private readonly Queue<ICommand> _commands;

    private Vector2 _input;
    private Vector2 _attackInput;
    private bool _interactInput;
    private long _tickTimestamp;
    private bool _isPaused = false;

    public double TickInterval => _tickInterval;

    public int Floor { get => _floor; set => _floor = value; }
    public Tilemap Tilemap => _tilemap;
    public Vector2 Exit { get => _exit; set => _exit = value; }
    public List<Entity> Entities => _entities;

    public Vector2 Input { get => _input; set => _input = value; }
    public Vector2 AttackInput { get => _attackInput; set => _attackInput = value; }
    public bool InteractInput { get => _interactInput; set => _interactInput = value; }
    public long TickTimestamp => _tickTimestamp;
    public bool IsPaused { get => _isPaused; set => _isPaused = value; }

    public GameWorld(double tickInterval, Tilemap tilemap, List<Entity> entities) {
        _tickInterval = tickInterval;

        _tilemap = tilemap;
        _entities = entities;

        _input = Vector2.Zero;
        _tickTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        _commands = [];
    }

    public void Tick(ClientContext context) {
        while (_commands.Count > 0)
            _commands.Dequeue().Execute(this, context);
        if (_isPaused) return;
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