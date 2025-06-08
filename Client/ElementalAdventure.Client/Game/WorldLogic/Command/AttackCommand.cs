using ElementalAdventure.Client.Game.SystemLogic;
using ElementalAdventure.Client.Game.WorldLogic.GameObject;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.WorldLogic.Command;

public class AttackCommand : ICommand {
    private Vector2 _target;

    public AttackCommand(Vector2 target) {
        _target = target;
    }

    public void Execute(GameWorld world, ClientContext context) {
        world.AttackInput = _target;
    }
}