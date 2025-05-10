using ElementalAdventure.Client.Core.Rendering;
using ElementalAdventure.Client.Core.UI;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.UI.ViewGroups;

public class LinearLayout : ViewGroupBase {
    private OrientationType _orientation;

    public OrientationType Orientation { get => _orientation; set => _orientation = value; }

    public LinearLayout() {
        _orientation = OrientationType.Horizontal;
    }

    public override void Measure() {
        _computedSize = Vector2.Zero;
        foreach (IView view in _views) {
            view.Measure();
            _computedSize.X = _orientation == OrientationType.Horizontal ? _computedSize.X + view.ComputedSize.X : Math.Max(_computedSize.X, view.ComputedSize.X);
            _computedSize.Y = _orientation == OrientationType.Vertical ? _computedSize.Y + view.ComputedSize.Y : Math.Max(_computedSize.Y, view.ComputedSize.Y);
        }
    }

    public override void Layout(float depth = 0.0f, float step = 0.0f) {
        Vector2 position = _computedPosition.Xy;
        foreach (IView view in _views) {
            view.ComputedPosition = new Vector3(position.X, position.Y, depth);
            if (_orientation == OrientationType.Horizontal) position.X += view.ComputedSize.X;
            else position.Y += view.ComputedSize.Y;

            if (view is IViewGroup group)
                group.Layout(depth + step, step);
        }
    }

    public override void Render(IRenderer renderer) {
        foreach (IView view in _views)
            view.Render(renderer);
    }

    public enum OrientationType { Horizontal, Vertical }
    public class LayoutParams : IViewGroup.ILayoutParams { }
}