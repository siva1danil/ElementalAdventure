using ElementalAdventure.Client.Game.Components.Data;
using ElementalAdventure.Client.Game.Components.Utils;
using ElementalAdventure.Client.Game.WorldLogic.Command;
using ElementalAdventure.Client.Game.WorldLogic.Component.Data;
using ElementalAdventure.Client.Game.WorldLogic.GameObject;
using ElementalAdventure.Common.Assets;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.WorldLogic.Component.Behaviour;

public class PlayerBehaviourComponent : IBehaviourComponent {
    private readonly AssetManager _assetManager;
    private readonly PlayerType _playerType;
    private bool _facingRight = true;
    private long _attackedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public PlayerBehaviourComponent(AssetManager assetManager, PlayerType playerType) {
        _assetManager = assetManager;
        _playerType = playerType;
    }

    public void Update(GameWorld world, Entity entity) {
        // Update invincibility
        if (entity.LivingDataComponent!.InvincibilityCounter > 0) {
            entity.LivingDataComponent.InvincibilityCounter--;
        }

        // Update velocity
        entity.PositionDataComponent.Velocity = world.Input;

        // Interact
        if (world.InteractInput) {
            float distance = (world.Exit - entity.PositionDataComponent.Position).LengthSquared;
            if (distance < 1.0f) {
                world.IsPaused = true;
                world.AddCommand(new NextLevelCommand());
            }
            world.InteractInput = false;
        }

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
        entity.PositionDataComponent.Z = world.Tilemap.GetNormalizedDepth(world.Tilemap.Midground, entity.PositionDataComponent.Position.Y, _playerType.DepthLayerOffset, _playerType.DepthHeightOffset);

        // Update texture
        if (entity.PositionDataComponent.Velocity.X > 0.0f) _facingRight = true;
        else if (entity.PositionDataComponent.Velocity.X < 0.0f) _facingRight = false;
        if (entity.PositionDataComponent.Velocity.LengthSquared > 0.0f) {
            entity.TextureDataComponent.Visible = true;
            entity.TextureDataComponent.TextureAtlas = _playerType.TextureAtlas;
            entity.TextureDataComponent.Texture = _facingRight ? _playerType.TextureWalkRight : _playerType.TextureWalkLeft;
        } else {
            entity.TextureDataComponent.Visible = true;
            entity.TextureDataComponent.TextureAtlas = _playerType.TextureAtlas;
            entity.TextureDataComponent.Texture = _facingRight ? _playerType.TextureIdleRight : _playerType.TextureIdleLeft;
        }

        // Attack
        if (world.AttackInput.LengthSquared > 0.0f) {
            Vector2 direction = (world.AttackInput - entity.PositionDataComponent.Position).NormalizedOrZero();
            if (direction.LengthSquared > 0.0f && _attackedAt + 1000 < DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()) {
                ProjectileType type = _assetManager.Get<ProjectileType>(new AssetID("fireball"));
                Entity projectile = new Entity(_assetManager, null, new ProjectileDataComponent(type.TargetsEnemies, type.TargetsPlayers, type.Damage, type.Speed), new HitboxDataComponent(new Box2(-0.25f, -0.25f, 0.25f, 0.25f)), [new ProjectileBehaviourComponent(type)]);
                projectile.PositionDataComponent.Position = entity.PositionDataComponent.Position;
                projectile.PositionDataComponent.Velocity = direction * type.Speed;
                world.AddCommand(new AddEntityCommand(projectile));
                _attackedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }
            world.AttackInput = Vector2.Zero;
        }
    }
}