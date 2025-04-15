using ElementalAdventure.Client.Game.Logic.GameObject;

namespace ElementalAdventure.Client.Game.Logic.Component.Behaviour;

public class ControllableBehaviourComponent : IBehavourComponent {
    public ControllableBehaviourComponent() { }

    public void Update(GameWorld world, Entity entity) {
        entity.PositionDataComponent.Velocity = world.Input;
    }
}