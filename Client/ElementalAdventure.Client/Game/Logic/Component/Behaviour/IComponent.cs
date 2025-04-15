using ElementalAdventure.Client.Game.Logic.GameObject;

namespace ElementalAdventure.Client.Game.Logic.Component.Behaviour;

public interface IBehavourComponent {
    void Update(GameWorld world, Entity entity);
}