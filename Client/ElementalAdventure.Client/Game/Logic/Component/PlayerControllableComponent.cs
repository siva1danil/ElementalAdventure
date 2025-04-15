using ElementalAdventure.Client.Game.Logic.GameObject;

namespace ElementalAdventure.Client.Game.Logic.Component;

public class PlayerControllableComponent : IComponent {
    public PlayerControllableComponent() { }

    public void Update(GameWorld world, Entity entity) {
        entity.PositionComponent.Velocity = world.Input;
    }
}