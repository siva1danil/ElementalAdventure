using ElementalAdventure.Client.Core.Rendering;
using ElementalAdventure.Client.Core.UI;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.UI.ViewGroups;

public class LinearLayout : ViewGroupBase {
    private OrientationType _orientation;
    private GravityType _gravity;

    public OrientationType Orientation { get => _orientation; set => _orientation = value; }
    public GravityType Gravity { get => _gravity; set => _gravity = value; }

    public LinearLayout() {
        _orientation = OrientationType.Horizontal;
        _gravity = GravityType.Start;
    }

    public override void Measure(Vector2 available) {
        _computedSize = Vector2.Zero;
        foreach (IView view in _views) {
            view.Measure(new Vector2(_size.X == 0.0f ? available.X : _size.X, _size.Y == 0.0f ? available.Y : _size.Y));
            _computedSize.X = _orientation == OrientationType.Horizontal ? _computedSize.X + view.ComputedSize.X : Math.Max(_computedSize.X, view.ComputedSize.X);
            _computedSize.Y = _orientation == OrientationType.Vertical ? _computedSize.Y + view.ComputedSize.Y : Math.Max(_computedSize.Y, view.ComputedSize.Y);
        }
    }

    public override void Layout(float depth = 0.0f, float step = 0.0f) {
        Vector2 position = _computedPosition.Xy;
        foreach (IView view in _views) {
            if (_gravity == GravityType.Center)
                position = _orientation == OrientationType.Horizontal
                    ? new Vector2(position.X, position.Y + (_computedSize.Y - view.ComputedSize.Y) / 2.0f)
                    : new Vector2(position.X + (_computedSize.X - view.ComputedSize.X) / 2.0f, position.Y);
            else if (_gravity == GravityType.End)
                position = _orientation == OrientationType.Horizontal
                    ? new Vector2(position.X, position.Y + _computedSize.Y - view.ComputedSize.Y)
                    : new Vector2(position.X + _computedSize.X - view.ComputedSize.X, position.Y);
            view.ComputedPosition = new Vector3(position.X, position.Y, depth);
            position += _orientation == OrientationType.Horizontal ? new Vector2(view.ComputedSize.X, 0.0f) : new Vector2(0.0f, view.ComputedSize.Y);

            if (view is IViewGroup group)
                group.Layout(depth + step, step);
        }
    }

    public override void Render(IRenderer renderer) {
        foreach (IView view in _views)
            view.Render(renderer);
    }

    public enum OrientationType { Horizontal, Vertical }
    public enum GravityType { Start, Center, End }
    public class LayoutParams : IViewGroup.ILayoutParams { }
}