namespace ElementalAdventure.Client.Scenes;

public interface IScene : IDisposable {
    void Update();
    void Render();
}