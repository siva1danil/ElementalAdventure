using ElementalAdventure.Client.Game.Components.Data;
using ElementalAdventure.Client.Game.WorldLogic.GameObject;

namespace ElementalAdventure.Client.Game.WorldLogic.Component.Behaviour;

public class PlayerBehaviourComponent : IBehavourComponent {
    private readonly PlayerType _playerType;
    private bool _facingRight = true;

    public PlayerBehaviourComponent(PlayerType playerType) {
        _playerType = playerType;
    }

    public void Update(GameWorld world, Entity entity) {
        // Update velocity
        entity.PositionDataComponent.Velocity = world.Input;

        // Tick movement
        entity.PositionDataComponent.Position += entity.PositionDataComponent.Velocity * entity.LivingDataComponent!.MovementSpeed;

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
    }
}