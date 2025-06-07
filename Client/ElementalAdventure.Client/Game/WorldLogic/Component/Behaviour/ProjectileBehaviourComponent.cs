using ElementalAdventure.Client.Game.WorldLogic.GameObject;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.WorldLogic.Component.Behaviour;

public class ProjectileBehaviourComponent : IBehaviourComponent {
    public ProjectileBehaviourComponent() {
        // No initialization needed for now
    }

    public void Update(GameWorld world, Entity entity) {
        // Update velocity
        entity.PositionDataComponent.Velocity = entity.PositionDataComponent.Velocity.Normalized() * entity.ProjectileDataComponent!.Speed;

        // Tick movement
        Vector2 position = entity.PositionDataComponent.Position;
        position += entity.PositionDataComponent.Velocity;
        entity.PositionDataComponent.Position = position;

        // Update Z
        entity.PositionDataComponent.Z = world.Tilemap.GetNormalizedDepth(world.Tilemap.Midground, position.Y, 0, 0);

        // Check for collisions with entities
        foreach (Entity target in world.Entities) {
            if (target == entity || target.LivingDataComponent == null || (!entity.ProjectileDataComponent!.TargetsEnemies && target.LivingDataComponent.TargetableByPlayers) || (!entity.ProjectileDataComponent.TargetsPlayers && target.LivingDataComponent.TargetableByEnemies))
                continue;

            if ((entity.ProjectileDataComponent.TargetsEnemies && target.LivingDataComponent.TargetableByPlayers) || (entity.ProjectileDataComponent.TargetsPlayers && target.LivingDataComponent.TargetableByEnemies)) {
                target.LivingDataComponent.Health -= entity.ProjectileDataComponent.Damage;
                world.RemoveEntity(entity);
                break;
            }
        }

        // Check for collisions with tilemap walls
        foreach (Box2 wall in world.Tilemap.Walls) {
            Box2 hitbox = new(entity.HitboxDataComponent!.Box.Min + position, entity.HitboxDataComponent.Box.Max + position);
            if (wall.Intersects(hitbox)) {
                world.RemoveEntity(entity);
                break;
            }
        }
    }
}