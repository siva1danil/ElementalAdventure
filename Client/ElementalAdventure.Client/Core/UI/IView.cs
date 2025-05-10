using ElementalAdventure.Client.Core.Rendering;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Core.UI;

public interface IView {
    public Vector2 Size { get; set; }
    public IViewGroup? Parent { get; set; }

    public Vector2 ComputedSize { get; set; }
    public Vector3 ComputedPosition { get; set; }

    public void InvalidateLayout();
    public void Measure(Vector2 available);
    public void Render(IRenderer renderer);
}