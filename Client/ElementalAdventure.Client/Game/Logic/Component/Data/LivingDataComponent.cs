namespace ElementalAdventure.Client.Game.Logic.Component.Data;

public class LivingDataComponent : IDataComponent {
    private bool _targetableByEnemies, _targetableByPlayers;
    private float _movementSpeed;

    public bool TargetableByEnemies { get => _targetableByEnemies; set => _targetableByEnemies = value; }
    public bool TargetableByPlayers { get => _targetableByPlayers; set => _targetableByPlayers = value; }
    public float MovementSpeed { get => _movementSpeed; set => _movementSpeed = value; }

    public LivingDataComponent(bool targetableByEnemies, bool targetableByPlayers, float movementSpeed) {
        _targetableByEnemies = targetableByEnemies;
        _targetableByPlayers = targetableByPlayers;
        _movementSpeed = movementSpeed;
    }
}