using ElementalAdventure.Client.Game.Data;
using ElementalAdventure.Client.Game.Logic.GameObject;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.Logic.Component.Behaviour;

public class EnemyBehaviourComponent : IBehavourComponent {
    private readonly EnemyType _enemyType;

    private bool _facingRight = true;

    public EnemyBehaviourComponent(EnemyType enemyType) {
        _enemyType = enemyType;
    }

    public void Update(GameWorld world, Entity entity) {
        // Follow player
        Entity? player = null;
        foreach (Entity e in world.Entities)
            if (e.LivingDataComponent?.TargetableByEnemies ?? false)
                player = e;
        if (player != null) {
            Vector2 direction = player.PositionDataComponent.Position - entity.PositionDataComponent.Position;
            entity.PositionDataComponent.Velocity = direction.LengthSquared > 0.0f ? direction.Normalized() : Vector2.Zero;
        } else {
            entity.PositionDataComponent.Velocity = Vector2.Zero;
        }

        // Tick movement
        entity.PositionDataComponent.Position += entity.PositionDataComponent.Velocity * entity.LivingDataComponent!.MovementSpeed;

        // Update Z
        if (entity.PositionDataComponent.Z != world.Tilemap.MidgroundZ) entity.PositionDataComponent.Z = world.Tilemap.MidgroundZ;

        // Update texture
        if (entity.PositionDataComponent.Velocity.X > 0.0f) _facingRight = true;
        else if (entity.PositionDataComponent.Velocity.X < 0.0f) _facingRight = false;
        if (entity.PositionDataComponent.Velocity.LengthSquared > 0.0f) {
            entity.TextureDataComponent.Visible = true;
            entity.TextureDataComponent.TextureAtlas = _enemyType.TextureAtlas;
            entity.TextureDataComponent.Texture = _facingRight ? _enemyType.TextureWalkRight : _enemyType.TextureWalkLeft;
        } else {
            entity.TextureDataComponent.Visible = true;
            entity.TextureDataComponent.TextureAtlas = _enemyType.TextureAtlas;
            entity.TextureDataComponent.Texture = _facingRight ? _enemyType.TextureIdleRight : _enemyType.TextureIdleLeft;
        }
    }
}