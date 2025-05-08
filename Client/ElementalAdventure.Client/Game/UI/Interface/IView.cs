using ElementalAdventure.Client.Core.Rendering;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.UI.Interface;

public interface IView {
    public Vector2 Size { get; set; }
    public Vector3 Position { get; set; }
    public IViewGroup? Parent { get; set; }

    public void InvalidateLayout();
    public void Measure();
    public void Render(IRenderer renderer);
}