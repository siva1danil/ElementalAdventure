namespace ElementalAdventure.Client.Game.WorldLogic.Component.Data;

public class ProjectileDataComponent : IDataComponent {
    private bool _targetsEnemies, _targetsPlayers;
    private float _damage;
    private float _speed;

    public bool TargetsEnemies { get => _targetsEnemies; set => _targetsEnemies = value; }
    public bool TargetsPlayers { get => _targetsPlayers; set => _targetsPlayers = value; }
    public float Damage { get => _damage; set => _damage = value; }
    public float Speed { get => _speed; set => _speed = value; }

    public ProjectileDataComponent(bool targetsEnemies, bool targetsPlayers, float damage, float speed) {
        _targetsEnemies = targetsEnemies;
        _targetsPlayers = targetsPlayers;
        _damage = damage;
        _speed = speed;
    }
}