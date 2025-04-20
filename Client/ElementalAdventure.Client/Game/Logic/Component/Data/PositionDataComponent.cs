using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.Logic.Component.Data;

public class PositionDataComponent : IDataComponent {
    private Vector2 _lastPosition, _position, _velocity;
    private float _z;

    public Vector2 LastPosition => _lastPosition;
    public Vector2 Position { get => _position; set { _lastPosition = _position; _position = value; } }
    public Vector2 Velocity { get => _velocity; set => _velocity = value; }
    public float Z { get => _z; set => _z = value; }

    public PositionDataComponent(Vector2 position, Vector2 velocity = new(), float z = 0.0f) {
        _lastPosition = _position = position;
        _velocity = velocity;
        _z = z;
    }
}