using ElementalAdventure.Client.Core.Rendering;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.UI.Interface;

public class LinearLayout<T> : IViewGroup<T> where T : notnull {
    private readonly List<IView<T>> _views;
    private readonly Dictionary<IView<T>, IViewGroup<T>.ILayoutParams> _layoutParams;

    private Vector2 _size;
    private Vector3 _calculatedPosition;
    private OrientationType _orientation;

    public Vector2 Size { get => _size; set => _size = value; }
    public Vector3 CalculatedPosition { get => _calculatedPosition; set => _calculatedPosition = value; }
    public OrientationType Orientation { get => _orientation; set => _orientation = value; }

    public LinearLayout() {
        _orientation = OrientationType.Horizontal;
        _views = [];
        _layoutParams = [];
    }

    public void Add(IView<T> view, IViewGroup<T>.ILayoutParams layoutParams) {
        if (layoutParams is not LayoutParams)
            throw new ArgumentException($"Expected LayoutParams of type {nameof(LayoutParams)}, but got {layoutParams.GetType().Name}.");
        _views.Add(view);
        _layoutParams[view] = layoutParams;
    }

    public void Remove(IView<T> view) {
        _views.Remove(view);
        _layoutParams.Remove(view);
    }

    public void Clear() {
        _views.Clear();
        _layoutParams.Clear();
    }

    public void Measure() {
        _size = Vector2.Zero;
        foreach (IView<T> view in _views) {
            view.Measure();
            _size.X = _orientation == OrientationType.Horizontal ? _size.X + view.Size.X : Math.Max(_size.X, view.Size.X);
            _size.Y = _orientation == OrientationType.Vertical ? _size.Y + view.Size.Y : Math.Max(_size.Y, view.Size.Y);
        }
    }

    public void Layout() {
        Vector2 position = _calculatedPosition.Xy;
        foreach (IView<T> view in _views) {
            view.CalculatedPosition = new Vector3(position.X, position.Y, 0.0f);
            if (_orientation == OrientationType.Horizontal) position.X += view.Size.X;
            else position.Y += view.Size.Y;

            if (view is IViewGroup<T> group)
                group.Layout();
        }
    }

    public void Render(IRenderer<T> renderer) {
        foreach (IView<T> view in _views)
            view.Render(renderer);
    }

    public enum OrientationType { Horizontal, Vertical }
    public class LayoutParams : IViewGroup<T>.ILayoutParams { }
}