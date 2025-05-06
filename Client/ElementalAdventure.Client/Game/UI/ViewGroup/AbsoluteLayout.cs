using ElementalAdventure.Client.Core.Rendering;
using ElementalAdventure.Client.Game.UI.Interface;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.UI.ViewGroup;

public class AbsoluteLayout : IViewGroup {
    private readonly List<IView> _views;
    private readonly Dictionary<IView, IViewGroup.ILayoutParams> _layoutParams;

    private Vector2 _size;
    private Vector3 _calculatedPosition;

    public Vector2 Size { get => _size; set => _size = value; }
    public Vector3 CalculatedPosition { get => _calculatedPosition; set => _calculatedPosition = value; }

    public AbsoluteLayout() {
        _views = [];
        _layoutParams = [];
    }

    public void Add(IView view, IViewGroup.ILayoutParams layoutParams) {
        if (layoutParams is not LayoutParams)
            throw new ArgumentException($"Expected LayoutParams of type {nameof(LayoutParams)}, but got {layoutParams.GetType().Name}.");
        _views.Add(view);
        _layoutParams[view] = layoutParams;
    }

    public void Remove(IView view) {
        _views.Remove(view);
        _layoutParams.Remove(view);
    }

    public void Clear() {
        _views.Clear();
        _layoutParams.Clear();
    }

    public void Measure() {
        Vector2 a = Vector2.Zero, b = Vector2.Zero;
        foreach (IView view in _views) {
            view.Measure();
            LayoutParams layoutParams = (LayoutParams)_layoutParams[view];
            a.X = Math.Min(a.X, layoutParams.Position.X - layoutParams.Anchor.X * view.Size.X);
            a.Y = Math.Min(a.Y, layoutParams.Position.Y - layoutParams.Anchor.Y * view.Size.Y);
            b.X = Math.Max(b.X, layoutParams.Position.X - layoutParams.Anchor.X * view.Size.X);
            b.Y = Math.Max(b.Y, layoutParams.Position.Y - layoutParams.Anchor.Y * view.Size.Y);
        }
        _size = b - a;
    }

    public void Layout() {
        foreach (IView view in _views) {
            LayoutParams layoutParams = (LayoutParams)_layoutParams[view];
            Vector2 position = layoutParams.Position - layoutParams.Anchor * view.Size;
            view.CalculatedPosition = new Vector3(position.X, position.Y, 0.0f);

            if (view is IViewGroup group)
                group.Layout();
        }
    }

    public void Render(IRenderer renderer) {
        foreach (IView view in _views)
            view.Render(renderer);
    }

    public class LayoutParams : IViewGroup.ILayoutParams {
        private Vector2 _position, _anchor;

        public Vector2 Position { get => _position; set => _position = value; }
        public Vector2 Anchor { get => _anchor; set => _anchor = value; }
    }
}