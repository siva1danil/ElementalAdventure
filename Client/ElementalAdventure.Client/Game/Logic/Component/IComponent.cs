using ElementalAdventure.Client.Game.Logic.GameObject;

namespace ElementalAdventure.Client.Game.Logic.Component;

public interface IComponent {
    void Update(GameWorld world, Entity entity);
}