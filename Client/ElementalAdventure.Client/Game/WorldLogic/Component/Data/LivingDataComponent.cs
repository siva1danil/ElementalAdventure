namespace ElementalAdventure.Client.Game.WorldLogic.Component.Data;

public class LivingDataComponent : IDataComponent {
    private bool _targetableByEnemies, _targetableByPlayers;
    private float _health, _maxHealth;
    private float _movementSpeed;
    private int _invincibilityCounter = 0;

    public bool TargetableByEnemies { get => _targetableByEnemies; set => _targetableByEnemies = value; }
    public bool TargetableByPlayers { get => _targetableByPlayers; set => _targetableByPlayers = value; }
    public float Health { get => _health; set => _health = _invincibilityCounter > 0 ? _health : value; }
    public float MaxHealth { get => _maxHealth; set => _maxHealth = value; }
    public float MovementSpeed { get => _movementSpeed; set => _movementSpeed = value; }
    public int InvincibilityCounter { get => _invincibilityCounter; set => _invincibilityCounter = value; }

    public LivingDataComponent(bool targetableByEnemies, bool targetableByPlayers, float health, float maxHealth, float movementSpeed) {
        _targetableByEnemies = targetableByEnemies;
        _targetableByPlayers = targetableByPlayers;
        _health = health;
        _maxHealth = maxHealth;
        _movementSpeed = movementSpeed;
    }
}