using ElementalAdventure.Client.Game.Components.Data;
using ElementalAdventure.Client.Game.WorldLogic.Command;
using ElementalAdventure.Client.Game.WorldLogic.GameObject;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.WorldLogic.Component.Behaviour;

public class ProjectileBehaviourComponent : IBehaviourComponent {
    private readonly ProjectileType _type;

    public ProjectileBehaviourComponent(ProjectileType type) {
        _type = type;
    }

    public void Update(GameWorld world, Entity entity) {
        // Tick movement
        Vector2 position = entity.PositionDataComponent.Position;
        position += entity.PositionDataComponent.Velocity;
        entity.PositionDataComponent.Position = position;

        // Update Z
        entity.PositionDataComponent.Z = world.Tilemap.GetNormalizedDepth(world.Tilemap.Midground, position.Y, 0, 0);

        // Check for collisions with entities
        foreach (Entity target in world.Entities) {
            if (target == entity || !target.Has<PlayerBehaviourComponent>() || !target.Has<EnemyBehaviourComponent>())
                continue;
            if (_type.TargetsEnemies && !entity.Has<EnemyBehaviourComponent>())
                continue;
            if (_type.TargetsPlayers && !entity.Has<PlayerBehaviourComponent>())
                continue;

            Box2 hitbox = new(entity.HitboxDataComponent!.Box.Min + position, entity.HitboxDataComponent.Box.Max + position);
            Box2 other = new(target.HitboxDataComponent!.Box.Min + target.PositionDataComponent.Position, target.HitboxDataComponent.Box.Max + target.PositionDataComponent.Position);
            if (hitbox.Intersects(other)) {
                target.LivingDataComponent!.Health -= _type.Damage;
                world.AddCommand(new RemoveEntityCommand(entity));
                break;
            }
        }

        // Check for collisions with tilemap walls
        foreach (Box2 wall in world.Tilemap.Walls) {
            Box2 hitbox = new(entity.HitboxDataComponent!.Box.Min + position, entity.HitboxDataComponent.Box.Max + position);
            if (wall.Intersects(hitbox)) {
                world.AddCommand(new RemoveEntityCommand(entity));
                break;
            }
        }

        // Update texture
        entity.TextureDataComponent.Texture = _type.Texture;
        entity.TextureDataComponent.TextureAtlas = _type.TextureAtlas;
        entity.TextureDataComponent.Visible = true;
    }
}