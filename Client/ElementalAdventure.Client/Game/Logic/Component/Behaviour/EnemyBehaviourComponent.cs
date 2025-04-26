using ElementalAdventure.Client.Game.Data;
using ElementalAdventure.Client.Game.Logic.GameObject;
using ElementalAdventure.Client.Game.Utils;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.Logic.Component.Behaviour;

public class EnemyBehaviourComponent : IBehavourComponent {
    private readonly EnemyType _enemyType;

    private readonly WeakReference<Entity?> _target = new(null);
    private bool _facingRight = true;
    private int _searchCounter = 0;

    public EnemyBehaviourComponent(EnemyType enemyType) {
        _enemyType = enemyType;
    }

    public void Update(GameWorld world, Entity entity) {
        // Find target
        if (_searchCounter == 0 && !_target.TryGetTarget(out Entity? _)) FindTarget(world, entity);
        _searchCounter = (_searchCounter + 1) % 20;

        // Follow target
        entity.PositionDataComponent.Velocity = (_target.TryGetTarget(out Entity? target) && target != null) ? (target.PositionDataComponent.Position - entity.PositionDataComponent.Position).NormalizedOrZero() : Vector2.Zero;

        // Tick movement
        entity.PositionDataComponent.Position += entity.PositionDataComponent.Velocity * entity.LivingDataComponent!.MovementSpeed;

        // Update Z
        entity.PositionDataComponent.Z = world.Tilemap.GetNormalizedDepth(world.Tilemap.Midground, entity.PositionDataComponent.Position.Y, _enemyType.DepthLayerOffset, _enemyType.DepthHeightOffset);

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

    private void FindTarget(GameWorld world, Entity entity) {
        foreach (Entity e in world.Entities)
            if (e.LivingDataComponent?.TargetableByEnemies ?? false)
                _target.SetTarget(e);
    }
}