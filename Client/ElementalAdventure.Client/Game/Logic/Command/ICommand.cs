namespace ElementalAdventure.Client.Game.Logic.Command;

public interface ICommand {
    void Execute(GameWorld world);
}