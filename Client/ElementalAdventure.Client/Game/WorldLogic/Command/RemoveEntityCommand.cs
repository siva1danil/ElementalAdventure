using ElementalAdventure.Client.Game.SystemLogic;
using ElementalAdventure.Client.Game.WorldLogic.GameObject;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.WorldLogic.Command;

public class RemoveEntityCommand : ICommand {
    private readonly Entity _entity;

    public RemoveEntityCommand(Entity entity) {
        _entity = entity;
    }

    public void Execute(GameWorld world, ClientContext context) {
        world.Entities.Remove(_entity);
    }
}