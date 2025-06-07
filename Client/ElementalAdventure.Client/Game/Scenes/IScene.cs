using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace ElementalAdventure.Client.Game.Scenes;

public interface IScene : IDisposable {
    void Update(FrameEventArgs args);
    void Render(FrameEventArgs args);
    void Resize(ResizeEventArgs args);
    void KeyDown(KeyboardKeyEventArgs args);
    void KeyUp(KeyboardKeyEventArgs args);
    void MouseDown(MouseButtonEventArgs args, Vector2 position);
    void MouseUp(MouseButtonEventArgs args, Vector2 position);
}