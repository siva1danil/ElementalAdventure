using ElementalAdventure.Client.Core.Rendering;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Core.UI;

public abstract class ViewBase : IView {
    protected Vector2 _size = Vector2.Zero;
    protected IViewGroup? _parent = null;

    protected Vector2 _computedSize = Vector2.Zero;
    protected Vector3 _computedPosition = Vector3.Zero;

    public Vector2 Size { get => _size; set => _size = value; }
    public IViewGroup? Parent { get => _parent; set => _parent = value; }

    public Vector2 ComputedSize { get => _computedSize; set => _computedSize = value; }
    public Vector3 ComputedPosition { get => _computedPosition; set => _computedPosition = value; }

    public void InvalidateLayout() {
        _parent?.InvalidateLayout();
    }

    public abstract void Measure(Vector2 available);
    public abstract void Render(IRenderer renderer);
}