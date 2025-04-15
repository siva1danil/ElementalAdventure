using ElementalAdventure.Client.Game.Logic.GameObject;

namespace ElementalAdventure.Client.Game.Logic.Component.Behaviour;

public class PlayerAnimatorBehaviourComponent : IBehavourComponent {
    private bool _facingRight = false;

    public PlayerAnimatorBehaviourComponent() { }

    public void Update(GameWorld world, Entity entity) {
        if (entity.PositionDataComponent.Velocity.X > 0.0f) _facingRight = true;
        else if (entity.PositionDataComponent.Velocity.X < 0.0f) _facingRight = false;

        if (entity.PositionDataComponent.Velocity.LengthSquared > 0.0f) entity.TextureDataComponent.Texture = _facingRight ? entity.EntityType.TextureWalkRight : entity.EntityType.TextureWalkLeft;
        else entity.TextureDataComponent.Texture = _facingRight ? entity.EntityType.TextureIdleRight : entity.EntityType.TextureIdleLeft;
    }
}