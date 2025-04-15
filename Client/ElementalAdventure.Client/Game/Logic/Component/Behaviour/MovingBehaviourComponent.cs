using ElementalAdventure.Client.Game.Logic.GameObject;

namespace ElementalAdventure.Client.Game.Logic.Component.Behaviour;

public class MovingBehaviourComponent : IBehavourComponent {
    public void Update(GameWorld world, Entity entity) {
        entity.PositionDataComponent.Position += entity.PositionDataComponent.Velocity * entity.EntityType.Speed;
        if (entity.PositionDataComponent.Z != world.Tilemap.MidgroundZ) entity.PositionDataComponent.Z = world.Tilemap.MidgroundZ;
    }
}