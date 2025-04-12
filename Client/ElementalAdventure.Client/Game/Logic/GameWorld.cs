namespace ElementalAdventure.Client.Game.Logic;

public class GameWorld {
    private readonly Tilemap _tilemap;
    private readonly List<Entity> _entities;

    public Tilemap Tilemap => _tilemap;
    public List<Entity> Entities => _entities;

    public GameWorld(Tilemap tilemap, List<Entity> entities) {
        _tilemap = tilemap;
        _entities = entities;
    }
}