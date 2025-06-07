using ElementalAdventure.Client.Game.WorldLogic.GameObject;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.WorldLogic.Command;

public class AddEntityCommand : ICommand {
    private readonly Entity _entity;

    public AddEntityCommand(Entity entity) {
        _entity = entity;
    }

    public void Execute(GameWorld world) {
        world.Entities.Add(_entity);
    }
}