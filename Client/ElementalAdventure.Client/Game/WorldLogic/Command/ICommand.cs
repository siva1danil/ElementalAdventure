namespace ElementalAdventure.Client.Game.WorldLogic.Command;

public interface ICommand {
    void Execute(GameWorld world);
}