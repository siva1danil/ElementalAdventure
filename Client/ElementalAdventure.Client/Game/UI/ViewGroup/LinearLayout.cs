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
        _size = Vector2.Zero;
        foreach (IView view in _views) {
            view.Measure();
            _size.X = _orientation == OrientationType.Horizontal ? _size.X + view.Size.X : Math.Max(_size.X, view.Size.X);
            _size.Y = _orientation == OrientationType.Vertical ? _size.Y + view.Size.Y : Math.Max(_size.Y, view.Size.Y);
        }
    }

    public override void Layout(float depth = 0.0f, float step = 0.0f) {
        Vector2 position = _position.Xy;
        foreach (IView view in _views) {
            view.Position = new Vector3(position.X, position.Y, depth);
            if (_orientation == OrientationType.Horizontal) position.X += view.Size.X;
            else position.Y += view.Size.Y;

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