using ElementalAdventure.Client.Game.WorldLogic.GameObject;

namespace ElementalAdventure.Client.Game.WorldLogic.Component.Behaviour;

public interface IBehaviourComponent {
    void Update(GameWorld world, Entity entity);
}