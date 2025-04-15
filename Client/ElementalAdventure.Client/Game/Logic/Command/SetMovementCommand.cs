using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.Logic.Command;

public class SetMovementCommand : ICommand {
    private Vector2 _input;

    public SetMovementCommand(bool w, bool a, bool s, bool d) {
        _input = new Vector2((d ? 1 : 0) + (a ? -1 : 0), (s ? -1 : 0) + (w ? 1 : 0));
        if (_input.LengthSquared != 0) _input.Normalize();
    }

    public void Execute(GameWorld world) {
        world.Input = _input;
    }
}