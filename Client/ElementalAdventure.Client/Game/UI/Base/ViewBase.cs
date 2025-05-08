namespace ElementalAdventure.Client.Game.UI.ViewGroup;

using ElementalAdventure.Client.Core.Rendering;
using ElementalAdventure.Client.Game.UI.Interface;

using OpenTK.Mathematics;

public abstract class ViewBase : IView {
    protected Vector2 _size = Vector2.Zero;
    protected Vector3 _position = Vector3.Zero;
    protected IViewGroup? _parent = null;

    public Vector2 Size { get => _size; set => _size = value; }
    public Vector3 Position { get => _position; set => _position = value; }
    public IViewGroup? Parent { get => _parent; set => _parent = value; }

    public void InvalidateLayout() {
        _parent?.InvalidateLayout();
    }

    public abstract void Measure();
    public abstract void Render(IRenderer renderer);
}