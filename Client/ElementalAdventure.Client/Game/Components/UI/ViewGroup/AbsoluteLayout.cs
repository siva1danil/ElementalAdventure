using ElementalAdventure.Client.Core.Rendering;
using ElementalAdventure.Client.Core.UI;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.Components.UI.ViewGroups;

public class AbsoluteLayout : ViewGroupBase {
    private Vector2 _size;

    public Vector2 Size { get => _size; set => _size = value; }

    public AbsoluteLayout() { }

    public override void Measure(Vector2 available) {
        Vector2 a = Vector2.Zero, b = Vector2.Zero;
        foreach (IView view in _views) {
            view.Measure(new Vector2(_size.X == 0.0f ? available.X : _size.X, _size.Y == 0.0f ? available.Y : _size.Y));
        }
        _computedSize = available; // TODO: compute size based on children
    }

    public override void Layout(float depth = 0.0f, float step = 0.0f) {
        foreach (IView view in _views) {
            LayoutParams layoutParams = (LayoutParams)_layoutParams[view];
            Vector2 targetPosition = new((layoutParams.Position.X >= 0.0f && layoutParams.Position.X <= 1.0f) ? _computedSize.X * layoutParams.Position.X : layoutParams.Position.X, (layoutParams.Position.Y >= 0.0f && layoutParams.Position.Y <= 1.0f) ? _computedSize.Y * layoutParams.Position.Y : layoutParams.Position.Y);
            Vector2 position = _computedPosition.Xy + targetPosition - layoutParams.Anchor * view.ComputedSize;
            view.ComputedPosition = new Vector3(position.X, position.Y, depth);

            if (view is IViewGroup group)
                group.Layout(depth + step, step);
        }
    }

    public override void Render(IRenderer renderer) {
        foreach (IView view in _views)
            view.Render(renderer);
    }

    public class LayoutParams : IViewGroup.ILayoutParams {
        private Vector2 _position, _anchor;

        public Vector2 Position { get => _position; set => _position = value; }
        public Vector2 Anchor { get => _anchor; set => _anchor = value; }
    }
}