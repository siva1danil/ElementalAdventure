using ElementalAdventure.Client.Core.Rendering;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.UI.Interface;

public interface IView {
    public Vector2 Size { get; set; }
    public Vector3 CalculatedPosition { get; set; }

    public void Measure();
    public void Render(IRenderer renderer);
}