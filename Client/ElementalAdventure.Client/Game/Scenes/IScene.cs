namespace ElementalAdventure.Client.Game.Scenes;

public interface IScene : IDisposable {
    void Update();
    void Render();
}