using ElementalAdventure.Client.Game.Components.Data;
using ElementalAdventure.Client.Game.Components.Utils;
using ElementalAdventure.Client.Game.WorldLogic.GameObject;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.WorldLogic.Component.Behaviour;

public class EnemyBehaviourComponent : IBehaviourComponent {
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
        Vector2 movement = (_target.TryGetTarget(out Entity? target) && target != null) ? (target.PositionDataComponent.Position - entity.PositionDataComponent.Position) : Vector2.Zero;
        entity.PositionDataComponent.Velocity = movement.LengthSquared > 1.0f ? movement.NormalizedOrZero() : movement;

        // Tick movement
        Vector2 position = entity.PositionDataComponent.Position;
        Vector2 velocity = entity.PositionDataComponent.Velocity * entity.LivingDataComponent!.MovementSpeed;
        if (entity.HitboxDataComponent != null) {
            Vector2 newPositionX = new(position.X + velocity.X, position.Y);
            Box2 hitboxX = new(entity.HitboxDataComponent.Box.Min + newPositionX, entity.HitboxDataComponent.Box.Max + newPositionX);
            Box2 collider = default;
            foreach (Box2 wall in world.Tilemap.Walls) {
                if (wall.Intersects(hitboxX)) {
                    collider = wall;
                    break;
                }
            }
            if (collider == default)
                position.X += velocity.X;
            else if (velocity.X != 0)
                position.X = velocity.X > 0 ? collider.Min.X - entity.HitboxDataComponent.Box.Max.X - 0.01f : collider.Max.X + (0.0f - entity.HitboxDataComponent.Box.Min.X) + 0.01f;

            Vector2 newPositionY = new(position.X, position.Y + velocity.Y);
            Box2 hitboxY = new(entity.HitboxDataComponent.Box.Min + newPositionY, entity.HitboxDataComponent.Box.Max + newPositionY);
            Box2 colliderY = default;
            foreach (Box2 wall in world.Tilemap.Walls) {
                if (wall.Intersects(hitboxY)) {
                    colliderY = wall;
                    break;
                }
            }
            if (colliderY == default)
                position.Y += velocity.Y;
            else if (velocity.Y != 0)
                position.Y = velocity.Y > 0 ? colliderY.Min.Y - entity.HitboxDataComponent.Box.Max.Y - 0.01f : colliderY.Max.Y + (0.0f - entity.HitboxDataComponent.Box.Min.Y) + 0.01f;
        } else {
            position += new Vector2(velocity.X, velocity.Y);
        }
        entity.PositionDataComponent.Position = position;

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