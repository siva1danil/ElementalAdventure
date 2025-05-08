using System.Collections.ObjectModel;

using ElementalAdventure.Client.Core.Rendering;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.UI.Interface;

public class LinearLayout : IViewGroup {
    private readonly List<IView> _views;
    private readonly Dictionary<IView, IViewGroup.ILayoutParams> _layoutParams;

    private Vector2 _size;
    private Vector3 _calculatedPosition;
    private OrientationType _orientation;

    public ReadOnlyCollection<IView> Children => _views.AsReadOnly();
    public Vector2 Size { get => _size; set => _size = value; }
    public Vector3 CalculatedPosition { get => _calculatedPosition; set => _calculatedPosition = value; }
    public OrientationType Orientation { get => _orientation; set => _orientation = value; }

    public LinearLayout() {
        _orientation = OrientationType.Horizontal;
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
        _size = Vector2.Zero;
        foreach (IView view in _views) {
            view.Measure();
            _size.X = _orientation == OrientationType.Horizontal ? _size.X + view.Size.X : Math.Max(_size.X, view.Size.X);
            _size.Y = _orientation == OrientationType.Vertical ? _size.Y + view.Size.Y : Math.Max(_size.Y, view.Size.Y);
        }
    }

    public void Layout(float depth = 0.0f, float step = 0.0f) {
        Vector2 position = _calculatedPosition.Xy;
        foreach (IView view in _views) {
            view.CalculatedPosition = new Vector3(position.X, position.Y, depth);
            if (_orientation == OrientationType.Horizontal) position.X += view.Size.X;
            else position.Y += view.Size.Y;

            if (view is IViewGroup group)
                group.Layout(depth + step, step);
        }
    }

    public void Render(IRenderer renderer) {
        foreach (IView view in _views)
            view.Render(renderer);
    }

    public enum OrientationType { Horizontal, Vertical }
    public class LayoutParams : IViewGroup.ILayoutParams { }
}