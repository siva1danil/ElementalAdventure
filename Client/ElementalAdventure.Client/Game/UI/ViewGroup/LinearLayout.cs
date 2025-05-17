using ElementalAdventure.Client.Core.Rendering;
using ElementalAdventure.Client.Core.UI;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.UI.ViewGroups;

public class LinearLayout : ViewGroupBase {
    private Vector2 _size;
    private OrientationType _orientation;
    private GravityType _gravity;

    public Vector2 Size { get => _size; set => _size = value; }
    public OrientationType Orientation { get => _orientation; set => _orientation = value; }
    public GravityType Gravity { get => _gravity; set => _gravity = value; }

    public LinearLayout() {
        _orientation = OrientationType.Horizontal;
        _gravity = GravityType.Start;
    }

    public override void Measure(Vector2 available) {
        _computedSize = Vector2.Zero;
        foreach (IView view in _views) {
            LayoutParams layoutParams = (LayoutParams)_layoutParams[view];
            view.Measure(new Vector2(_size.X == 0.0f ? available.X : _size.X, _size.Y == 0.0f ? available.Y : _size.Y));
            _computedSize.X = _orientation == OrientationType.Horizontal ? _computedSize.X + view.ComputedSize.X + layoutParams.Margin.Y + layoutParams.Margin.W : Math.Max(_computedSize.X, view.ComputedSize.X + layoutParams.Margin.Y + layoutParams.Margin.W);
            _computedSize.Y = _orientation == OrientationType.Vertical ? _computedSize.Y + view.ComputedSize.Y + layoutParams.Margin.X + layoutParams.Margin.Z : Math.Max(_computedSize.Y, view.ComputedSize.Y + layoutParams.Margin.X + layoutParams.Margin.Z);
        }
    }

    public override void Layout(float depth = 0.0f, float step = 0.0f) {
        Vector2 position = _computedPosition.Xy;
        foreach (IView view in _views) {
            LayoutParams layoutParams = (LayoutParams)_layoutParams[view];
            Vector2 localPosition = _gravity switch {
                GravityType.Start => _orientation == OrientationType.Horizontal
                    ? new Vector2(position.X + layoutParams.Margin.W, position.Y + layoutParams.Margin.X)
                    : new Vector2(position.X + layoutParams.Margin.W, position.Y + layoutParams.Margin.X),
                GravityType.Center => _orientation == OrientationType.Horizontal
                    ? new Vector2(position.X + layoutParams.Margin.W, position.Y + (_computedSize.Y - view.ComputedSize.Y - layoutParams.Margin.X - layoutParams.Margin.Z) / 2.0f + layoutParams.Margin.X)
                    : new Vector2(position.X + (_computedSize.X - view.ComputedSize.X - layoutParams.Margin.Y - layoutParams.Margin.W) / 2.0f + layoutParams.Margin.W, position.Y + layoutParams.Margin.X),
                GravityType.End => _orientation == OrientationType.Horizontal
                    ? new Vector2(position.X + layoutParams.Margin.W, position.Y + _computedSize.Y - view.ComputedSize.Y - layoutParams.Margin.Z)
                    : new Vector2(position.X + _computedSize.X - view.ComputedSize.X - layoutParams.Margin.Y, position.Y + layoutParams.Margin.X),
                _ => throw new ArgumentOutOfRangeException(nameof(Gravity), _gravity, "Invalid gravity type")
            };
            view.ComputedPosition = new Vector3(localPosition.X, localPosition.Y, depth);
            position += _orientation == OrientationType.Horizontal ? new Vector2(layoutParams.Margin.W + view.ComputedSize.X + layoutParams.Margin.Y, 0.0f) : new Vector2(0.0f, layoutParams.Margin.X + view.ComputedSize.Y + layoutParams.Margin.Z);

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
    public class LayoutParams : IViewGroup.ILayoutParams {
        private Vector4 _margin;

        public Vector4 Margin { get => _margin; set => _margin = value; } // top, right, bottom, left
    }
}